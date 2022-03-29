using HikingTrailsApi.Domain.Enums;
using System;
using System.Collections.Generic;

namespace HikingTrailsApi.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; } = Role.User;

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsEmailConfirmed { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public List<Event> Events { get; set; }
    }
}
