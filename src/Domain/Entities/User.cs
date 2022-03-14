using HikingTrailsApi.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace HikingTrailsApi.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public Role Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime CreationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public List<Event> Events { get; set; }
    }
}
