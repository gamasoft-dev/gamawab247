using Application.DTOs;
using Application.Helpers;
using Domain.Common;
using Domain.Entities.Identities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public partial class UserService
    {
        public async Task<SuccessResponse<bool>> CreateRoleAsync(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "Role already exist");
            var role = new Role
            {
                Name = roleName,
            };
            await _roleManager.CreateAsync(role);

            return  new SuccessResponse<bool> { Message = ResponseMessages.CreationSuccessResponse,Data = true};
        }

        public async Task<SuccessResponse<bool>> DeleteRoleAsync(Guid roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString())
                ?? throw new RestException(System.Net.HttpStatusCode.BadRequest, "Role not found");

            await _roleManager.DeleteAsync(role);
            return new SuccessResponse<bool> { Message = "Role deleted successfully", Data = true };
        }

        public async Task<SuccessResponse<RoleDto>> GetRoleAsync(Guid roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString())
                ?? throw new RestException(System.Net.HttpStatusCode.BadRequest, "Role not found");

            await _roleManager.DeleteAsync(role);
            return new SuccessResponse<RoleDto> { Message = "Role deleted successfully", Data = _mapper.Map<RoleDto>(role) };
        }

        public async Task<SuccessResponse<List<RoleDto>>> GetAllRolesAsync()
        {
           var roles = await _roleManager.Roles.ToListAsync();
            return new SuccessResponse<List<RoleDto>>
            {
                Data = _mapper.Map<List<RoleDto>>(roles),
                Message = ResponseMessages.RetrievalSuccessResponse,
            };
        }

        public async Task<SuccessResponse<bool>> AddUserToRoleAsync(AddUserToRoleDto model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId.ToString())
              ?? throw new RestException(System.Net.HttpStatusCode.BadRequest, "Role not found");
            var userRoles = new List<UserRole>();
            foreach (var userId in model.UserIds)
            {
                var user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new RestException(System.Net.HttpStatusCode.BadRequest, "User not found");

                if (!await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoles.Add(new UserRole
                    {
                        RoleId = role.Id,
                        UserId = userId,
                    });
                }
            }
            await _userRole.AddRangeAsync(userRoles);
            await _userRole.SaveChangesAsync();

            return new SuccessResponse<bool> { Message = ResponseMessages.CreationSuccessResponse, Data = true };
        }

        public async Task<SuccessResponse<bool>> RemoveUserFromRoleAsync(RemoveUserFromRoleDto model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId.ToString())
              ?? throw new RestException(System.Net.HttpStatusCode.BadRequest, "Role not found");
            var userRoles = new List<UserRole>();
            foreach (var userId in model.UserIds)
            {
                var user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new RestException(System.Net.HttpStatusCode.BadRequest, "User not found");

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoles.Add(new UserRole
                    {
                        RoleId = role.Id,
                        UserId = userId,
                    });
                }
            }
             _userRole.RemoveRange(userRoles);
            await _userRole.SaveChangesAsync();

            return new SuccessResponse<bool> { Message = ResponseMessages.CreationSuccessResponse, Data = true };
        }

        public async Task<SuccessResponse<IList<string>>> GetUserRolesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new RestException(System.Net.HttpStatusCode.NotFound, "User not found");

            var userRoles = await _userManager.GetRolesAsync(user);
            return new SuccessResponse<IList<string>> { Data = userRoles, Message = ResponseMessages.RetrievalSuccessResponse };
        }
    }
}
