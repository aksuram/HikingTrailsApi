﻿using System;

namespace HikingTrailsApi.Domain.Entities
{
    public class Rating
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool IsPositive { get; set; }

        public DateTime CreationDate { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid? PostId { get; set; }
        public Post Post { get; set; }
        public Guid? CommentId { get; set; }
        public Comment Comment { get; set; }
    }
}
