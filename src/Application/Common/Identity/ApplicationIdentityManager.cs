using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Models;
using HikingTrailsApi.Application.Users;
using HikingTrailsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
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
        private readonly IApplicationDbContext _applicationDbContext;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDateTime _dateTime;
        private readonly JwtSettings _jwtSettings;

        //private readonly Dictionary<Role, string> _roles = TranslationHelper.Roles;

        public ApplicationIdentityManager(
            IApplicationDbContext applicationDbContext,
            //IHttpContextAccessor httpContextAccessor,
            IDateTime dateTime,
            JwtSettings jwtSettings)
        {
            _applicationDbContext = applicationDbContext;
            //_httpContextAccessor = httpContextAccessor;
            _dateTime = dateTime;
            _jwtSettings = jwtSettings;
        }

        //public async Task LogOut()
        //{
        //    if (!Guid.TryParse(_httpContextAccessor?.HttpContext?.User?
        //        .FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        //    {
        //        return;
        //    }

        //    User user;
        //    try
        //    {
        //        user = await _userManager.FindByIdAsync(userId.ToString());
        //        if (user == null) { return; }

        //        await _signInManager.SignOutAsync();
        //    }
        //    catch (Exception)
        //    {
        //        return;
        //    }

        //    //Log the event
        //    using var applicationDbContext = _applicationDbContextFactory.CreateDbContext();
        //    try
        //    {
        //        applicationDbContext.Events.Add(new Event()
        //        {
        //            Description = $"Atsijungė nuo sistemos",
        //            UserId = user.Id,
        //            CreationDate = _dateTime.Now
        //        });

        //        await applicationDbContext.SaveChangesAsync(CancellationToken.None);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new DatabaseException("Nepavyko išsaugoti duomenų duomenų bazėje", e);
        //    }
        //}

        public async Task<Result<UserLoginVm>> LogIn(UserLoginDto userLoginDto)
        {
            if (userLoginDto == null) return Result<UserLoginVm>.BadRequest();   //400

            //TODO: Iskelti validation?
            var userLoginDtoValidator = new UserLoginDtoValidator();
            var validationResult = userLoginDtoValidator.Validate(userLoginDto);

            if (!validationResult.IsValid)
            {
                return Result<UserLoginVm>.BadRequest(validationResult.Errors.Select(x =>
                    new FieldError(x.PropertyName, x.ErrorMessage)));   //400
            }

            var user = await _applicationDbContext.Users
                .FirstOrDefaultAsync(x => x.Email.ToLower() == userLoginDto.Email.ToLower());

            if (user == null)
            {
                return Result<UserLoginVm>.Unauthorized("Login", "Nepavyko prisijungti");  //401
            }

            if (user.IsDeleted)
            {
                return Result<UserLoginVm>.Forbidden("User blocked", "Vartotojas yra užblokuotas");    //403
            }

            if (!user.IsEmailConfirmed)
            {
                return Result<UserLoginVm>.Forbidden("Confirm email", "Prieš prisijungiant patvirtinkite naudotojo paskyrą paspaudžiant nuorodą el. pašte"); //403
            }

            var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password);
            if (!isPasswordCorrect)
            {
                return Result<UserLoginVm>.Unauthorized("Login", "Nepavyko prisijungti");  //401
            }

            user.LastLoginDate = _dateTime.Now;
            await _applicationDbContext.SaveChangesAsync(CancellationToken.None);

            return Result<UserLoginVm>
                .Success(new UserLoginVm
                {
                    Token = GenerateJwtToken(user)
                }); //200
        }

        private string GenerateJwtToken(User user)
        {
            var secret = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                //TODO: add claims which could be used client side
                //TODO: change token expiration date
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("id", user.Id.ToString()),
                    new Claim("role", user.Role.ToString())
                }),
                Expires = _dateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(securityTokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<Result> RegisterUser(UserRegistrationDto userRegistrationDto)
        {
            if (userRegistrationDto == null) return Result.BadRequest();    //400

            //TODO: Iskelti validation?
            var userRegistrationDtoValidator = new UserRegistrationDtoValidator(_applicationDbContext);
            var validationResult = userRegistrationDtoValidator.Validate(userRegistrationDto);

            if (!validationResult.IsValid)
            {
                return Result.BadRequest(validationResult.Errors.Select(x =>
                    new FieldError(x.PropertyName, x.ErrorMessage)));   //400
            }

            var user = new User()
            {
                Email = userRegistrationDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userRegistrationDto.Password),
                FirstName = userRegistrationDto.FirstName,
                LastName = userRegistrationDto.LastName,
                CreationDate = _dateTime.Now
            };

            _applicationDbContext.Users.Add(user);

            _applicationDbContext.Events.Add(
                new Event()
                {
                    Description = $"{user.FirstName} {user.LastName} užsiregistravo prie sistemos",
                    User = user,
                    CreationDate = _dateTime.Now
                }
            );

            await _applicationDbContext.SaveChangesAsync(CancellationToken.None);

            //TODO: Change to 201?
            return Result.Success();    //200
        }

        //public async Task DeleteUser(Guid id)
        //{
        //    if (!Guid.TryParse(_httpContextAccessor?.HttpContext?.User?
        //        .FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        //    {
        //        throw new NotFoundException("Nepavyko surasti prisijungusio naudotojo duomenų");
        //    }

        //    User user;
        //    try
        //    {
        //        user = await _userManager.FindByIdAsync(id.ToString());

        //        if (user == null)
        //        {
        //            throw new NotFoundException("Nepavyko surasti trinamo naudotojo duomenų");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new NotFoundException("Nepavyko surasti trinamo naudotojo duomenų", e);
        //    }

        //    user.IsDeleted = true;

        //    if (!(await _userManager.UpdateAsync(user)).Succeeded)
        //    {
        //        throw new IdentityException("Nepavyko ištrinti sistemos naudotojo");
        //    }

        //    //Log the event
        //    using var applicationDbContext = _applicationDbContextFactory.CreateDbContext();
        //    try
        //    {
        //        applicationDbContext.Events.Add(new Event()
        //        {
        //            Description = $"Ištrynė sistemos naudotoją {user.FirstName} {user.LastName}",
        //            UserId = userId,
        //            CreationDate = _dateTime.Now
        //        });

        //        await applicationDbContext.SaveChangesAsync(CancellationToken.None);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new DatabaseException("Nepavyko išsaugoti duomenų duomenų bazėje", e);
        //    }
        //}

        //public async Task UpdateUser(UserEditDto userEditDto)
        //{
        //    if (!Guid.TryParse(_httpContextAccessor?.HttpContext?.User?
        //        .FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        //    {
        //        throw new NotFoundException("Nepavyko surasti prisijungusio naudotojo duomenų");
        //    }

        //    User user;
        //    try
        //    {
        //        user = await _userManager.FindByIdAsync(userEditDto.Id.ToString());

        //        if (user == null)
        //        {
        //            throw new NotFoundException("Nepavyko surasti redaguojamo naudotojo duomenų");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new NotFoundException("Nepavyko surasti redaguojamo naudotojo duomenų", e);
        //    }

        //    user.UserName = userEditDto.UserName;
        //    user.FirstName = userEditDto.FirstName;
        //    user.LastName = userEditDto.LastName;
        //    user.Role = userEditDto.Role;
        //    user.IsDeleted = userEditDto.IsDeleted;

        //    if (!(await _userManager.UpdateAsync(user)).Succeeded)
        //    {
        //        throw new IdentityException("Nepavyko redaguoti sistemos naudotojo");
        //    }

        //    //Log the event
        //    using var applicationDbContext = _applicationDbContextFactory.CreateDbContext();
        //    try
        //    {
        //        applicationDbContext.Events.Add(new Event()
        //        {
        //            Description = $"Redagavo sistemos naudotoją {user.FirstName} {user.LastName}",
        //            UserId = userId,
        //            CreationDate = _dateTime.Now
        //        });

        //        await applicationDbContext.SaveChangesAsync(CancellationToken.None);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new DatabaseException("Nepavyko išsaugoti duomenų duomenų bazėje", e);
        //    }
        //}
    }
}
