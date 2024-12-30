using AutoMapper;
using BitsionTest.API.Models.Domain.Contracts;
using BitsionTest.API.Models.Domain.Entities;

namespace BitsionTest.API.Infrastructure.Mapping
{
    public class MappingProfile: Profile    // creamos reglas de mapeado a entidades
    {
        public MappingProfile() {
            CreateMap<ApplicationUser, UserResponse>();
            CreateMap<ApplicationUser, CurrentUserResponse>();
            CreateMap<UserRegisterRequest, ApplicationUser>();
        }
    }
}
