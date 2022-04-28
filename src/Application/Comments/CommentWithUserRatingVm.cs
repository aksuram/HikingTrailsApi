using HikingTrailsApi.Application.Ratings;
using HikingTrailsApi.Application.Users;
using System;
using System.Collections.Generic;

namespace HikingTrailsApi.Application.Comments
{
    public class CommentWithUserRatingVm
    {
        public Guid Id { get; set; }
        public string Body { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? EditDate { get; set; }

        public UserAvatarVm User { get; set; }
        public RatingVm UserRating { get; set; }

        public int Rating { get; set; }

        public Guid PostId { get; set; }
        public Guid? ReplyToId { get; set; }

        public List<CommentWithUserRatingVm> Replies { get; set; }
    }
}
