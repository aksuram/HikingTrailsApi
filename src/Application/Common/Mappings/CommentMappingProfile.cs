using AutoMapper;
using HikingTrailsApi.Application.Comments;
using HikingTrailsApi.Domain.Entities;

namespace HikingTrailsApi.Application.Common.Mappings
{
    public class CommentMappingProfile : Profile
    {
        public CommentMappingProfile()
        {
            CreateMap<Comment, CommentVm>()
                .ForMember(x => x.UserFullName, opt =>
                    opt.MapFrom(y => y.User.FirstName + " " + y.User.LastName));

            CreateMap<Comment, CommentWithUserRatingVm>()
                .ForMember(x => x.User, opt =>
                    opt.MapFrom(y => y.User))
                .ForMember(x => x.Replies, opt =>
                    opt.Ignore())
                .ForMember(x => x.UserRating, opt =>
                    opt.Ignore());
        }
    }
}
