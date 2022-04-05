using HikingTrailsApi.Application.Users;
using HikingTrailsApi.Domain.Entities;

namespace HikingTrailsApi.Application.Common.Mappings
{
    public static class UserMappings
    {
        public static UserVm ToUserVm(User user)
        {
            return new UserVm
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsDeleted = user.IsDeleted,
                CreationDate = user.CreationDate,
                LastLoginDate = user.LastLoginDate
            };
        }
    }
}
