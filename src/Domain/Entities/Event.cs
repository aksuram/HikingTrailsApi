using System;

namespace HikingTrailsApi.Domain.Entities
{
    public class Event
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Description { get; set; }

        public DateTime CreationDate { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
