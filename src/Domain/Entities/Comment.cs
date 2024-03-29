﻿using System;
using System.Collections.Generic;

namespace HikingTrailsApi.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Body { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime CreationDate { get; set; }
        public DateTime? EditDate { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid PostId { get; set; }
        public Post Post { get; set; }

        public Guid? ReplyToId { get; set; }
        public Comment ReplyTo { get; set; }
        public List<Comment> Replies { get; set; }

        public int Rating { get; set; }
        public List<Rating> Ratings { get; set; }

        public List<Image> Images { get; set; }
    }
}
