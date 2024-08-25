using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/roles")]
    public class RolesController : ControllerBase
    {
        private readonly IUserService _userService;

        public RolesController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Endpoint to creat a role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("create")]
        [ProducesResponseType(typeof(SuccessResponse<bool>), 201)]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var response = await _userService.CreateRoleAsync(roleName);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to delete role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<bool>), 201)]
        public async Task<IActionResult> DeleteRole([FromRoute] Guid id)
        {
            var response = await _userService.DeleteRoleAsync(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all roles
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(typeof(SuccessResponse<UserLoginResponse>), 200)]
        public async Task<IActionResult> GetAllRoles()
        {
            var response = await _userService.GetAllRolesAsync();

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to add users to a role
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("add-users-to-role")]
        [ProducesResponseType(typeof(SuccessResponse<UpdateUserResponse>), 200)]
        public async Task<IActionResult> AddUsersToRole([FromBody] AddUserToRoleDto model)
        {
            var response = await _userService.AddUserToRoleAsync(model);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to remove users to a role
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("remove-users-from-role")]
        [ProducesResponseType(typeof(SuccessResponse<UpdateUserResponse>), 200)]
        public async Task<IActionResult> RemoveUsersFromRole([FromBody] RemoveUserFromRoleDto model)
        {
            var response = await _userService.RemoveUserFromRoleAsync(model);
            return Ok(response);
        }


        /// <summary>
        /// Endpoint to get a role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetRoleById))]
        [ProducesResponseType(typeof(SuccessResponse<RoleDto>), 200)]
        public async Task<IActionResult> GetRoleById([FromRoute] Guid id)
        {
            var response = await _userService.GetRoleAsync(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get a role
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}/roles", Name = nameof(GetUserRoles))]
        [ProducesResponseType(typeof(SuccessResponse<RoleDto>), 200)]
        public async Task<IActionResult> GetUserRoles([FromRoute] Guid userId)
        {
            var response = await _userService.GetUserRolesAsync(userId);
            return Ok(response);
        }
    }
}
