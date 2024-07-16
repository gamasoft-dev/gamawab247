using System;

namespace Application.DTOs
{
    public record AddUserToRoleDto
    {
        public Guid[] UserIds { get; set; }
        public Guid RoleId { get; set; }
    }

    public record RemoveUserFromRoleDto : AddUserToRoleDto;

    public record RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
