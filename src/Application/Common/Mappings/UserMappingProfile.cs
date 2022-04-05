using AutoMapper;
using HikingTrailsApi.Application.Users;
using HikingTrailsApi.Domain.Entities;

namespace HikingTrailsApi.Application.Common.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserVm>();
        }
    }
}
