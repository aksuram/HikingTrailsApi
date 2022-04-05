using HikingTrailsApi.Application.Common.Models;
using HikingTrailsApi.Application.Users;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Common.Interfaces
{
    public interface IApplicationIdentityManager
    {
        public Task<Result<UserLoginVm>> LogIn(UserLoginDto userLoginDto);
        //public Task LogOut();
        public Task<Result<UserVm>> RegisterUser(UserRegistrationDto userRegistrationDto);
        //public Task DeleteUser(Guid userId);
        //public Task UpdateUser(UserEditDto userEditDto);
    }
}
