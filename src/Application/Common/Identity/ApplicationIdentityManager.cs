using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Domain.Entities;
using HikingTrailsApi.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Common.Identity
{
    public class ApplicationIdentityManager : IApplicationIdentityManager
    {
        private readonly IApplicationDbContextFactory<IApplicationDbContext> _applicationDbContextFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IDateTime _dateTime;
        private readonly JwtSettings _jwtSettings;

        private readonly Dictionary<Role, string> _roles = TranslationHelper.Roles;

        public ApplicationIdentityManager(
            IApplicationDbContextFactory<IApplicationDbContext> applicationDbContextFactory,
            IHttpContextAccessor httpContextAccessor,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IDateTime dateTime,
            JwtSettings jwtSettings)
        {
            _applicationDbContextFactory = applicationDbContextFactory;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
            _dateTime = dateTime;
            _jwtSettings = jwtSettings;
        }

        public async Task<string> ApiLogIn(string username, string password)
        {
            User user;

            try
            {
                user = await _userManager.FindByNameAsync(username);
            }
            catch (Exception)
            {
                throw new LogInException();
            }

            if (user == null || user.IsDeleted) { throw new LogInException(); }

            var signInResult = await _signInManager.PasswordSignInAsync(
                username ?? "", password ?? "", false, false);

            if (!signInResult.Succeeded) { throw new LogInException(); }

            //Log the event
            using var applicationDbContext = _applicationDbContextFactory.CreateDbContext();
            try
            {
                applicationDbContext.Events.Add(new Event()
                {
                    Description = $"Prisijungė prie sistemos",
                    UserId = user.Id,
                    CreationDate = _dateTime.Now
                });

                await applicationDbContext.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception e)
            {
                throw new DatabaseException("Nepavyko išsaugoti duomenų duomenų bazėje", e);
            }

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var secret = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                //TODO: add claims which could be used client side
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("id", user.Id.ToString()),
                    new Claim("roles", user.Role.ToString())
                }),
                Expires = _dateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(securityTokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task LogIn(string username, string password)
        {
            User user;

            try
            {
                user = await _userManager.FindByNameAsync(username);
            }
            catch (Exception)
            {
                throw new LogInException();
            }

            if (user == null || user.IsDeleted) { throw new LogInException(); }

            var signInResult = await _signInManager.PasswordSignInAsync(
                username ?? "", password ?? "", false, false);

            if (!signInResult.Succeeded) { throw new LogInException(); }

            //Log the event
            using var applicationDbContext = _applicationDbContextFactory.CreateDbContext();
            try
            {
                applicationDbContext.Events.Add(new Event()
                {
                    Description = $"Prisijungė prie sistemos",
                    UserId = user.Id,
                    CreationDate = _dateTime.Now
                });

                await applicationDbContext.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception e)
            {
                throw new DatabaseException("Nepavyko išsaugoti duomenų duomenų bazėje", e);
            }
        }

        public async Task LogOut()
        {
            if (!Guid.TryParse(_httpContextAccessor?.HttpContext?.User?
                .FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            {
                return;
            }

            User user;
            try
            {
                user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null) { return; }

                await _signInManager.SignOutAsync();
            }
            catch (Exception)
            {
                return;
            }

            //Log the event
            using var applicationDbContext = _applicationDbContextFactory.CreateDbContext();
            try
            {
                applicationDbContext.Events.Add(new Event()
                {
                    Description = $"Atsijungė nuo sistemos",
                    UserId = user.Id,
                    CreationDate = _dateTime.Now
                });

                await applicationDbContext.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception e)
            {
                throw new DatabaseException("Nepavyko išsaugoti duomenų duomenų bazėje", e);
            }
        }

        public async Task RegisterUser(UserDto userDto)
        {
            if (!Guid.TryParse(_httpContextAccessor?.HttpContext?.User?
                .FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            {
                throw new NotFoundException("Nepavyko surasti prisijungusio naudotojo duomenų");
            }

            var user = new User()
            {
                UserName = userDto.UserName,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Role = userDto.Role,
                CreationDate = _dateTime.Now
            };

            if (!(await _userManager.CreateAsync(user, userDto.Password)).Succeeded)
            {
                throw new IdentityException("Nepavyko sukurti sistemos naudotojo");
            }

            if (!(await _userManager.AddToRoleAsync(user, userDto.Role.ToString())).Succeeded)
            {
                throw new IdentityException("Nepavyko sistemos naudotojui sukurti rolės");
            }

            //Log the event
            using var applicationDbContext = _applicationDbContextFactory.CreateDbContext();
            try
            {
                applicationDbContext.Events.Add(new Event()
                {
                    Description = $"Užregistravo naudotoją {user.FirstName} {user.LastName} su pareigybe {_roles[userDto.Role]}",
                    UserId = userId,
                    CreationDate = _dateTime.Now
                });

                await applicationDbContext.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception e)
            {
                throw new DatabaseException("Nepavyko išsaugoti duomenų duomenų bazėje", e);
            }
        }

        public async Task DeleteUser(Guid id)
        {
            if (!Guid.TryParse(_httpContextAccessor?.HttpContext?.User?
                .FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            {
                throw new NotFoundException("Nepavyko surasti prisijungusio naudotojo duomenų");
            }

            User user;
            try
            {
                user = await _userManager.FindByIdAsync(id.ToString());

                if (user == null)
                {
                    throw new NotFoundException("Nepavyko surasti trinamo naudotojo duomenų");
                }
            }
            catch (Exception e)
            {
                throw new NotFoundException("Nepavyko surasti trinamo naudotojo duomenų", e);
            }

            user.IsDeleted = true;

            if (!(await _userManager.UpdateAsync(user)).Succeeded)
            {
                throw new IdentityException("Nepavyko ištrinti sistemos naudotojo");
            }

            //Log the event
            using var applicationDbContext = _applicationDbContextFactory.CreateDbContext();
            try
            {
                applicationDbContext.Events.Add(new Event()
                {
                    Description = $"Ištrynė sistemos naudotoją {user.FirstName} {user.LastName}",
                    UserId = userId,
                    CreationDate = _dateTime.Now
                });

                await applicationDbContext.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception e)
            {
                throw new DatabaseException("Nepavyko išsaugoti duomenų duomenų bazėje", e);
            }
        }

        public async Task UpdateUser(UserEditDto userEditDto)
        {
            if (!Guid.TryParse(_httpContextAccessor?.HttpContext?.User?
                .FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            {
                throw new NotFoundException("Nepavyko surasti prisijungusio naudotojo duomenų");
            }

            User user;
            try
            {
                user = await _userManager.FindByIdAsync(userEditDto.Id.ToString());

                if (user == null)
                {
                    throw new NotFoundException("Nepavyko surasti redaguojamo naudotojo duomenų");
                }
            }
            catch (Exception e)
            {
                throw new NotFoundException("Nepavyko surasti redaguojamo naudotojo duomenų", e);
            }

            user.UserName = userEditDto.UserName;
            user.FirstName = userEditDto.FirstName;
            user.LastName = userEditDto.LastName;
            user.Role = userEditDto.Role;
            user.IsDeleted = userEditDto.IsDeleted;

            if (!(await _userManager.UpdateAsync(user)).Succeeded)
            {
                throw new IdentityException("Nepavyko redaguoti sistemos naudotojo");
            }

            //Log the event
            using var applicationDbContext = _applicationDbContextFactory.CreateDbContext();
            try
            {
                applicationDbContext.Events.Add(new Event()
                {
                    Description = $"Redagavo sistemos naudotoją {user.FirstName} {user.LastName}",
                    UserId = userId,
                    CreationDate = _dateTime.Now
                });

                await applicationDbContext.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception e)
            {
                throw new DatabaseException("Nepavyko išsaugoti duomenų duomenų bazėje", e);
            }
        }
    }
}
