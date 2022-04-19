using AutoMapper;
using HikingTrailsApi.Application.Posts;
using HikingTrailsApi.Domain.Entities;

namespace HikingTrailsApi.Application.Common.Mappings
{
    public class PostMappingProfile : Profile
    {
        public PostMappingProfile()
        {
            CreateMap<Post, PostVm>()
                .ForMember(x => x.UserFullName, opt =>
                    opt.MapFrom(y => y.User.FirstName + " " + y.User.LastName));

            CreateMap<Post, PostWithUserRatingVm>()
                .ForMember(x => x.UserFullName, opt =>
                    opt.MapFrom(y => y.User.FirstName + " " + y.User.LastName))
                .ForMember(x => x.UserRating, opt =>
                    opt.Ignore());
        }
    }
}
