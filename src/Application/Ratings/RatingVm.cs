using System;

namespace HikingTrailsApi.Application.Ratings
{
    public class RatingVm
    {
        public Guid Id { get; set; }
        public bool IsPositive { get; set; }

        //TODO: Remove if unused
        //public DateTime CreationDate { get; set; }

        //public Guid UserId { get; set; }
        //public string UserFullName { get; set; }

        //public Guid? PostId { get; set; }
        //public Guid? CommentId { get; set; }
    }
}
