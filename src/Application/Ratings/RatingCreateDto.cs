using System;

namespace HikingTrailsApi.Application.Ratings
{
    public class RatingCreateDto
    {
        public bool IsPositive { get; set; }
        public Guid? PostId { get; set; }
        public Guid? CommentId { get; set; }
    }
}
