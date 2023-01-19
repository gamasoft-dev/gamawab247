using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Interfaces
{
    public interface IUserService: IAutoDependencyService
    {
        Task<SuccessResponse<CreateUserResponse>> CreateUser(CreateUserDTO model, List<string> roles = null);
        Task<SuccessResponse<object>> CompleteUserRegistration(VerifyTokenDTO token);
        Task<SuccessResponse<UserLoginResponse>> Login(UserLoginDTO model);
        Task<SuccessResponse<UpdateUserResponse>> UpdateUser(Guid id, UpdateUserDto model);
        Task<SuccessResponse<UserByIdResponse>> GetUserById(Guid userId);
        Task<PagedResponse<IEnumerable<UserResponse>>> GetUsers(ResourceParameter parameter, string name, IUrlHelper urlHelper);
        Task<PagedResponse<IEnumerable<UserResponse>>> GetUsersByBusinessId(Guid businessId, ResourceParameter parameter, string name, IUrlHelper urlHelper);
        Task<SuccessResponse<RefreshTokenResponse>> GetRefreshToken(RefreshTokenDTO model);
        Task<SuccessResponse<string>> ResetPassword(ResetPasswordDTO model);
        Task<SuccessResponse<object>> VerifyToken(VerifyTokenDTO model);
        Task<SuccessResponse<object>> SetPassword(SetPasswordDTO model);
        Task<SuccessResponse<bool>> ValidateRefreshToken(string token);
        Task<SuccessResponse<bool>> DeleteUser(Guid userId);
    }
}
