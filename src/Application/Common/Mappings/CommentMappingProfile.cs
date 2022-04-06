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

            CreateMap<Comment, FormattedCommentVm>()
                .ForMember(x => x.UserFullName, opt =>
                    opt.MapFrom(y => y.User.FirstName + " " + y.User.LastName))
                .ForMember(x => x.Replies, opt =>
                    opt.Ignore());
        }
    }
}
