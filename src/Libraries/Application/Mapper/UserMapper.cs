using Application.DTOs;
using Application.DTOs.BusinessDtos;
using AutoMapper;
using Domain.Entities.Identities;
using Domain.Enums;

namespace Application.Mapper
{
    public class UserMapper: Profile
    {
        public UserMapper()
        {
            CreateMap<User, CreateUserResponse>();

            CreateMap<CreateUserDTO, User>().AfterMap((src, dest) =>
            {
                dest.Email = src.Email.Trim().ToLower();
                dest.UserName = src.Email.Trim().ToLower();
                dest.NormalizedUserName = src.Email.ToUpper();
                dest.NormalizedEmail = src.Email.ToUpper();
                dest.Status = EUserStatus.ACTIVE.ToString();
            });

            CreateMap<CreateBusinessDto, CreateUserDTO>().AfterMap((src, dest) =>
            {
                dest.Email = src.BusinessAdminEmail;
                dest.FirstName = src.AdminFirstName;
                dest.LastName = src.AdminLastName;
                dest.PhoneNumber = src.AdminPhoneNumber;
                dest.UserName = src.BusinessAdminEmail;
            });

            CreateMap<User, UserLoginResponse>().ReverseMap();

            CreateMap<User, UserByIdResponse>();
            CreateMap<User, UpdateUserResponse>();
            CreateMap<User, UserResponse>().AfterMap((src, dest) =>
            {
                dest.BusinessName = src.Business.Name;
            });
            CreateMap<UpdateUserDto, User>();
        }
    }
}
