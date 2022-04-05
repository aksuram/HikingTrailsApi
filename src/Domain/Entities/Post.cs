using System;
using System.Collections.Generic;

namespace HikingTrailsApi.Domain.Entities
{
    public class Post
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Body { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime? EditDate { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public List<Comment> Comments { get; set; }

        //TODO: Rating and Categories?
    }
}
