using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Endpoint to register a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(SuccessResponse<CreateUserResponse>), 201)]
        public async Task<IActionResult> RegisterUser(CreateUserDTO model)
        {
            var response = await _userService.CreateUser(model);

            return CreatedAtAction(nameof(GetUserById), new {id = response.Data.Id}, response);
        }

        /// <summary>
        /// Endpoint to verify a user email address
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("complete-registration")]
        [ProducesResponseType(typeof(SuccessResponse<object>), 201)]
        public async Task<IActionResult> ComfirmUserEmailAddres(VerifyTokenDTO model)
        {
            var response = await _userService.CompleteUserRegistration(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to login a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(SuccessResponse<UserLoginResponse>), 200)]
        public async Task<IActionResult> LoginUser(UserLoginDTO model)
        {
            var response = await _userService.Login(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to update a user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<UpdateUserResponse>), 200)]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto model)
        {
            var response = await _userService.UpdateUser(id, model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get a user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetUserById))]
        [ProducesResponseType(typeof(SuccessResponse<UserByIdResponse>), 200)]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var response = await _userService.GetUserById(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get a paginated list of users
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetUsers))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<UserResponse>>), 200)]
        public async Task<IActionResult> GetUsers([FromQuery] ResourceParameter parameter)
        {
            var response = await _userService.GetUsers(parameter, nameof(UserResponse), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to login a user
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("validate-refresh-token")]
        [ProducesResponseType(typeof(SuccessResponse<bool>), 200)]
        public async Task<IActionResult> RefreshToken(string token)
        {
            var response = await _userService.ValidateRefreshToken(token);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to generate a new access and refresh token
        /// </summary>
        /// <param name="mdoel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("refresh-token")]
        [ProducesResponseType(typeof(SuccessResponse<RefreshTokenResponse>), 200)]
        public async Task<IActionResult> RefreshToken(RefreshTokenDTO mdoel)
        {
            var response = await _userService.GetRefreshToken(mdoel);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to initializes password reset
        /// </summary>
        /// <param name="mdoel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("reset-password")]
        [ProducesResponseType(typeof(SuccessResponse<VerifyTokenDTO>), 200)]
        public async Task<IActionResult> ForgotPassword(ResetPasswordDTO mdoel)
        {
            var response = await _userService.ResetPassword(mdoel);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to verify token
        /// </summary>
        /// <param name="mdoel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("verify-otp-token")]
        [ProducesResponseType(typeof(SuccessResponse<object>), 200)]
        public async Task<IActionResult> VerifyToken(VerifyTokenDTO mdoel)
        {
            var response = await _userService.VerifyToken(mdoel);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to set password
        /// </summary>
        /// <param name="mdoel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("password-reset")]
        [ProducesResponseType(typeof(SuccessResponse<object>), 200)]
        public async Task<IActionResult> SetPassword(SetPasswordDTO mdoel)
        {
            var response = await _userService.SetPassword(mdoel);

            return Ok(response);
        }
    }
}
