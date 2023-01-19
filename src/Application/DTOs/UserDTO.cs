using System;

namespace Application.DTOs
{
    public class CreateUserDTO
    { 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        internal string UserName { get; set; }
        public Guid? BusinessId { get; set; }
    }
    public class UserLoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RefreshTokenDTO
    {
        public string AccessToken  { get; set; }
        public string RefreshToken { get; set; }
    }
    public class ResetPasswordDTO
    {
        public string Email { get; set; }
    }
    public class VerifyTokenDTO
    {
        public string Token { get; set; }
    }
    public class SetPasswordDTO
    {
        public string Password { get; set; }
        public string Token { get; set; }
    }

    public class UpdateUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
