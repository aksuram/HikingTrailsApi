using AutoMapper;
using HikingTrailsApi.Application.Users;
using HikingTrailsApi.Domain.Entities;
using System.Linq;

namespace HikingTrailsApi.Application.Common.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserVm>();

            CreateMap<User, UserAvatarVm>()
                .ForMember(x => x.FullName, opt =>
                    opt.MapFrom(y => y.FirstName + " " + y.LastName))

                .ForMember(x => x.Avatar, opt =>
                    opt.MapFrom(y => y.Images.Count == 0 ? null : y.Images.FirstOrDefault().Path));
        }
    }
}
