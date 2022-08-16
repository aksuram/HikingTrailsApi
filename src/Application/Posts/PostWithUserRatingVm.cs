using HikingTrailsApi.Application.Ratings;
using HikingTrailsApi.Application.Users;
using System;

namespace HikingTrailsApi.Application.Posts
{
    public class PostWithUserRatingVm
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? EditDate { get; set; }

        public UserAvatarVm User { get; set; }
        public RatingVm UserRating { get; set; }

        public int Rating { get; set; }
    }
}
