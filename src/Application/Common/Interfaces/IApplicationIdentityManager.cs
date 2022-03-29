using HikingTrailsApi.Application.Models;
using HikingTrailsApi.Application.Users;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Common.Interfaces
{
    public interface IApplicationIdentityManager
    {
        //public Task LogIn(string username, string password);
        //public Task<string> ApiLogIn(string username, string password);
        //public Task LogOut();
        public Task<Result> RegisterUser(UserRegistrationDto userRegistrationDto);
        //public Task DeleteUser(Guid userId);
        //public Task UpdateUser(UserEditDto userEditDto);
    }
}
