using HikingTrailsApi.Application.Common.Models;
using HikingTrailsApi.Application.Users;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Common.Interfaces
{
    public interface IApplicationIdentityManager
    {
        public Task<Result<UserVm>> RegisterUser(UserRegistrationDto userRegistrationDto);
        public Task<Result<UserLoginVm>> LogIn(UserLoginDto userLoginDto);
    }
}
