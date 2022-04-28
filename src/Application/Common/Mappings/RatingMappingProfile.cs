using AutoMapper;
using HikingTrailsApi.Application.Ratings;
using HikingTrailsApi.Domain.Entities;

namespace HikingTrailsApi.Application.Common.Mappings
{

    public class RatingMappingProfile : Profile
    {
        public RatingMappingProfile()
        {
            CreateMap<Rating, RatingVm>();
        }
    }
}
