using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.Identities;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Errors.Model;

namespace Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<UserActivity> _userActivityRepository;
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IRepository<Token> _tokenRepository;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly RoleManager<Role> _roleManager;
        private readonly IBusinessService _businessService;

        public UserService(
            IUserRepository userRepository, 
            IRepository<UserActivity> userActivityRepository,
            IJwtAuthenticationManager jwtAuthenticationManager,
            UserManager<User> userManager,
            IMapper mapper,
            IRepository<Token> tokenRepository,
            IConfiguration configuration,
            IMailService mailService,
            IEmailTemplateService emailTemplateService,
            RoleManager<Role> roleManager,
            IBusinessService businessService)
        {
            _userRepository = userRepository;
            _userActivityRepository = userActivityRepository;
            _jwtAuthenticationManager = jwtAuthenticationManager;
            _mapper = mapper;
            _userManager = userManager;
            _tokenRepository = tokenRepository;
            _configuration = configuration;
            _mailService = mailService;
            _emailTemplateService = emailTemplateService;
            _roleManager = roleManager;
            _businessService = businessService;
        }

        public async Task<SuccessResponse<CreateUserResponse>> CreateUser(CreateUserDTO model, List<string> roles = null)
        {
            // ReSharper disable once HeapView.ClosureAllocation
            var email = model.Email.Trim().ToLower();
            var isEmailExist = await _userRepository.ExistsAsync(x => x.Email == email);

            if (isEmailExist)
                throw new RestException(HttpStatusCode.BadRequest, message: ResponseMessages.DuplicateEmail);

            if (string.IsNullOrEmpty(model.Password))
                throw new RestException(HttpStatusCode.BadGateway, message: ResponseMessages.PasswordCannotBeEmpty);
            
            //Verify correctness if business id if businessid not null

            if(model.BusinessId is not null)
            {
                var business = await _businessService.GetBusinessByBusinessId((Guid)model.BusinessId);
                if (business is null)
                    throw new RestException(HttpStatusCode.BadRequest, message: ResponseMessages.BusinessNotFound);
            }

            var user = _mapper.Map<User>(model);
            if (roles is null)
            {
                roles = new List<string>();
                var role = await _roleManager.FindByNameAsync(ERole.USER.ToString());
                if (role is not null)
                    roles.Add(role.Name);
            }

            await _userManager.CreateAsync(user, model.Password);
            
            await AddUserToRoles(user, roles);

            var userActivity = new UserActivity
            {
                EventType = "User created",
                UserId = user.Id,
                ObjectClass = "USER",
                Details = "signed up",
                ObjectId = user.Id
            };

            await _userActivityRepository.AddAsync(userActivity);

            var token = CustomToken.GenerateOtp();
            
            //send comfirmation token using a fake smtp fake
            var tokenEntity = new Token
            {
                UserId = user.Id,
                TokenType = TokenTypeEnum.Email_Confirmation,
                OTPToken = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsValid = true
            };
            await _tokenRepository.AddAsync(tokenEntity);

            const string title = "Confirm Email Address";
            var message = _emailTemplateService.GetConfirmEmailTemplate(token, user.Email, user.FirstName, title);
            await _mailService.SendSingleMail(user.Email, message, title);

             await _userActivityRepository.SaveChangesAsync();

            var userResponse = _mapper.Map<CreateUserResponse>(user);

            return new SuccessResponse<CreateUserResponse>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = userResponse
            };
        }

        public async Task<SuccessResponse<object>> CompleteUserRegistration(VerifyTokenDTO token)
        {
            var verifyToken = await VerifyToken(token);
            if (!verifyToken.Success)
                throw new RestException(HttpStatusCode.BadRequest, message: ResponseMessages.InvalidToken);

            var tokenEntity = await _tokenRepository.FirstOrDefault(x => x.OTPToken == token.Token);
            if(tokenEntity.ExpiresAt < DateTime.UtcNow)
                throw new RestException(HttpStatusCode.BadRequest, message: ResponseMessages.ExpiredToken);
            
            var user = await _userRepository.FirstOrDefault(x => x.Id == tokenEntity.UserId);
            if (user.EmailConfirmed == true)
                throw new RestException(HttpStatusCode.BadRequest, message: ResponseMessages.EmailAlreadyComfirmed);
            
            user.EmailConfirmed = true;
            user.Verified = true;
            _userRepository.Update(user);

            //no need to remove except we decide to use cache
            //_tokenRepository.Remove(tokenEntity);
            await _userRepository.SaveChangesAsync();

            return new SuccessResponse<object>
            {
                Message = ResponseMessages.Successful
            };
        }

        public async Task<SuccessResponse<UserLoginResponse>> Login(UserLoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.WrongEmailOrPassword);

            // ReSharper disable once HeapView.BoxingAllocation
            if (!user.EmailConfirmed || user?.Status?.ToUpper() != EUserStatus.ACTIVE.ToString() || !user.Verified)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.WrongEmailOrPassword);
            
            var isUserValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isUserValid)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.WrongEmailOrPassword);

            user.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var userActivity = new UserActivity
            {
                EventType = "User Login",
                UserId = user.Id,
                ObjectClass = "USER",
                Details = "logged in",
                ObjectId = user.Id
            };

            var roles = await _userManager.GetRolesAsync(user);
            await _userActivityRepository.AddAsync(userActivity);
            await _userActivityRepository.SaveChangesAsync();

            var userViewModel = _mapper.Map<UserLoginResponse>(user);

            var tokenResponse = _jwtAuthenticationManager.Authenticate(user, roles);
            userViewModel.AccessToken = tokenResponse.AccessToken;
            userViewModel.ExpiresIn = tokenResponse.ExpiresIn;
            userViewModel.RefreshToken = _jwtAuthenticationManager.GenerateRefreshToken(user.Id);

            return new SuccessResponse<UserLoginResponse>
            {
                Message = ResponseMessages.LoginSuccessResponse,
                Data = userViewModel
            };
        }

        public async Task<SuccessResponse<bool>> ValidateRefreshToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new RestException(HttpStatusCode.BadRequest, "Invalid parameter");

            var validate = _jwtAuthenticationManager.ValidateRefreshToken(token);
            if(!validate)
                throw new RestException(HttpStatusCode.BadRequest, "Invalid parameter");

            return new SuccessResponse<bool>
            {
                Message = "Successfully validated",
                Success = true,
                Data = validate
            };
        }

        public async Task<SuccessResponse<bool>> DeleteUser(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
                throw new NotFoundException("User not found exception");

            await _userManager.DeleteAsync(user);
            return new SuccessResponse<bool>()
            {
                Data = true
            };
        }


        public async Task<SuccessResponse<UpdateUserResponse>> UpdateUser(Guid id, UpdateUserDto model)
        {
            if(model == null || id == Guid.Empty)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.Failed);

            var user = await _userRepository.GetByIdAsync(id);
            if (user is null)
            {
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.Failed);
            };

            user.UserName = model.UserName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.UpdatedAt = DateTime.Now;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
            
            var updatedUserResponse = _mapper.Map<UpdateUserResponse>(user);
            return new SuccessResponse<UpdateUserResponse>
            {
                Message = ResponseMessages.UpdateResponse,
                Data = updatedUserResponse
            };
        }

        public async Task<SuccessResponse<UserByIdResponse>> GetUserById(Guid userId)
        {
            var user = await _userRepository.SingleOrDefaultNoTracking(x => x.Id == userId);

            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var userResponse = _mapper.Map<UserByIdResponse>(user);
            var roles = await _userManager.GetRolesAsync(user);
            userResponse.Roles = new List<string>();

            foreach ( var role in roles)
                userResponse.Roles.Add(role);

            return new SuccessResponse<UserByIdResponse>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data    = userResponse
            };
        }

        public async Task<PagedResponse<IEnumerable<UserResponse>>> GetUsers(ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            var userQuery = _userRepository.GetUsersQuery(parameter.Search);
            var userResponses = userQuery.ProjectTo<UserResponse>(_mapper.ConfigurationProvider);

            var users = await PagedList<UserResponse>.CreateAsync(userResponses, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<UserResponse>.CreateResourcePageUrl(parameter, name, users, urlHelper);

            var response = new PagedResponse<IEnumerable<UserResponse>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = users,
                Meta = new Helpers.Meta
                {
                    Pagination = page
                }
            };
            return response;
        }

        public async Task<PagedResponse<IEnumerable<UserResponse>>> GetUsersByBusinessId(Guid businessId, ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            var business = await _businessService.GetBusinessByBusinessId(businessId);
            if (business is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.BusinessNotFound);

            var businessUserQuery = _userRepository.GetBusinessUsersQuery(businessId, parameter.Search);
            var businessUserResponses = businessUserQuery.ProjectTo<UserResponse>(_mapper.ConfigurationProvider);

            var users = await PagedList<UserResponse>.CreateAsync(businessUserResponses, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<UserResponse>.CreateResourcePageUrl(parameter, name, users, urlHelper);

            var response = new PagedResponse<IEnumerable<UserResponse>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = users,
                Meta = new Helpers.Meta
                {
                    Pagination = page
                }
            };
            return response;
        }

        public async Task<SuccessResponse<RefreshTokenResponse>> GetRefreshToken(RefreshTokenDTO model)
        {
            var userId = _jwtAuthenticationManager.GetUserIdFromAccessToken(model.AccessToken);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var isRefreshTokenValid = _jwtAuthenticationManager.ValidateRefreshToken(model.RefreshToken);
            if (!isRefreshTokenValid)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.InvalidToken);

            var tokenResponse = _jwtAuthenticationManager.Authenticate(user);

            var newRefreshToken = _jwtAuthenticationManager.GenerateRefreshToken(user.Id);

            var tokenViewModel = new RefreshTokenResponse
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = tokenResponse.ExpiresIn
            };

            return  new SuccessResponse<RefreshTokenResponse>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = tokenViewModel
            };

        }
        public async Task<SuccessResponse<string>> ResetPassword(ResetPasswordDTO model)
        {
            var user = await _userRepository.FirstOrDefault(x => x.Email == model.Email);
            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            if (!user.EmailConfirmed)
                throw new RestException(HttpStatusCode.BadRequest, "Email address not confirm. Please confirm your email address");

            var token = new Token
            {
                UserId = user.Id,
                TokenType = TokenTypeEnum.Password_Reset,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsValid = true,
                OTPToken = CustomToken.GenerateToken()
            };
            await _tokenRepository.AddAsync(token);

            string title = "Password Reset";
            var message = _emailTemplateService.GetConfirmEmailTemplate(token.OTPToken, user.Email, user.FirstName, title);
            await _mailService.SendSingleMail(user.Email, message, title);

            var userActivity = new UserActivity
            {
                EventType = "Password Reset Request",
                UserId = user.Id,
                ObjectClass = "USER",
                Details = "requested for password reset",
                ObjectId = user.Id
            };
            
            await _userActivityRepository.AddAsync(userActivity);

            await _tokenRepository.SaveChangesAsync();

            //Send email to queue
            return new SuccessResponse<string>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data =  "Password reset code successfully sent to your email address",
                Success = true
            };
        }
        public async Task<SuccessResponse<object>> VerifyToken(VerifyTokenDTO model)
        {
            var token = await _tokenRepository.FirstOrDefault(x => x.OTPToken == model.Token);
            if (token == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.InvalidToken);

            if (!token.IsValid)
                throw new RestException(HttpStatusCode.BadRequest, "Sorry, token has been invalidated.");

            var tokenLifeSpan = double.Parse(_configuration["TOKEN_LIFESPAN"]);

            var isValid = CustomToken.IsTokenValid(token, tokenLifeSpan);
            if(!isValid)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.InvalidToken);       

            return new SuccessResponse<object>
            {
                Message = ResponseMessages.Successful,
            };
        }
        public async Task<SuccessResponse<object>> SetPassword(SetPasswordDTO model)
        {
            var token = await _tokenRepository.FirstOrDefault(x => x.OTPToken == model.Token);
            if (token == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.InvalidToken);

            if (!token.IsValid)
                throw new RestException(HttpStatusCode.BadRequest, "Sorry, token has been invalidated.");

            if (!token.TokenType.Equals(TokenTypeEnum.Password_Reset))
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.InvalidToken);

            var tokenLifeSpan = double.Parse(_configuration["TOKEN_LIFESPAN"]);

            var isValid = CustomToken.IsTokenValid(token, tokenLifeSpan);
            if (!isValid)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.InvalidToken);

            token.TokenType = TokenTypeEnum.Password_Reset;
            token.UpdatedAt = DateTime.UtcNow;
            token.IsValid = false;
            _tokenRepository.Update(token);
            //_tokenRepository.Remove(token);

            var user = await _userRepository.GetByIdAsync(token.UserId);
            user.UpdatedAt = DateTime.UtcNow;
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
            user.Verified = true;
            user.EmailConfirmed = true;
            _userRepository.Update(user);

            var userActivity = new UserActivity
            {
                EventType = "Set Password",
                UserId = user.Id,
                Details = "set password",
                ObjectClass = "USER",
                ObjectId = user.Id
            };
            await _userActivityRepository.AddAsync(userActivity);           

            await _tokenRepository.SaveChangesAsync();

            return new SuccessResponse<object>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
            };
        }
        
        #region Private Functions
        private async Task AddUserToRoles(User user, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                if(!await _userManager.IsInRoleAsync(user, role))
                    await _userManager.AddToRoleAsync(user, role);
            }
        }
        #endregion
    }
}
