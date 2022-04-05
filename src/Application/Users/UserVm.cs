using HikingTrailsApi.Domain.Enums;
using System;

namespace HikingTrailsApi.Application.Users
{
    public class UserVm
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime CreationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
