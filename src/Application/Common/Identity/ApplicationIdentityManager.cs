using AutoMapper;
using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Common.Models;
using HikingTrailsApi.Application.Users;
using HikingTrailsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace HikingTrailsApi.Application.Common.Identity
{
    public class ApplicationIdentityManager : IApplicationIdentityManager
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IDateTime _dateTime;
        private readonly JwtSettings _jwtSettings;
        private readonly IMapper _mapper;
        private readonly IImageStorageService _imageStorageService;

        public ApplicationIdentityManager(
            IApplicationDbContext applicationDbContext,
            IDateTime dateTime,
            JwtSettings jwtSettings,
            IMapper mapper,
            IImageStorageService imageStorageService)
        {
            _applicationDbContext = applicationDbContext;
            _dateTime = dateTime;
            _jwtSettings = jwtSettings;
            _mapper = mapper;
            _imageStorageService = imageStorageService;
        }

        public async Task<Result<UserLoginVm>> LogIn(UserLoginDto userLoginDto)
        {
            var userLoginDtoValidator = new UserLoginDtoValidator();
            var validationResult = await userLoginDtoValidator
                .ValidateAsync(userLoginDto);

            if (!validationResult.IsValid)
            {
                return Result<UserLoginVm>.BadRequest(validationResult.Errors.Select(x =>
                    new FieldError(x.PropertyName, x.ErrorMessage)));   //400
            }

            var user = await _applicationDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email.ToLower() == userLoginDto.Email.ToLower());

            if (user == null)
            {
                return Result<UserLoginVm>.Unauthorized("Login", "Nepavyko prisijungti");  //401
            }

            if (user.IsDeleted)
            {
                return Result<UserLoginVm>.Forbidden("User blocked", "Naudotojas yra užblokuotas");    //403
            }

            if (!user.IsEmailConfirmed)
            {
                return Result<UserLoginVm>.Forbidden("Confirm email",
                    "Prieš prisijungiant patvirtinkite naudotojo paskyrą paspaudžiant nuorodą el. pašte");  //403
            }

            var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password);
            if (!isPasswordCorrect)
            {
                return Result<UserLoginVm>.Unauthorized("Login", "Nepavyko prisijungti");  //401
            }

            await _applicationDbContext.Users
                    .Where(x => x.Email.ToLower() == userLoginDto.Email.ToLower())
                    .UpdateAsync(x => new User { LastLoginDate = _dateTime.Now });

            return Result<UserLoginVm>.Success(new UserLoginVm{ Token = GenerateJwtToken(user) });  //200
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
                    new Claim("email", user.Email),
                    new Claim("role", user.Role.ToString()),
                    new Claim("firstName", user.FirstName),
                    new Claim("lastName", user.LastName),
                    new Claim("fullName", $"{user.FirstName} {user.LastName}")
                }),
                Expires = _dateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(securityTokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<Result<UserVm>> RegisterUser(UserRegistrationDto userRegistrationDto)
        {
            var userRegistrationDtoValidator = new UserRegistrationDtoValidator(_applicationDbContext);
            var validationResult = await userRegistrationDtoValidator
                .ValidateAsync(userRegistrationDto);

            if (!validationResult.IsValid)
            {
                return Result<UserVm>.BadRequest(validationResult.Errors.Select(x =>
                    new FieldError(x.PropertyName, x.ErrorMessage)));   //400
            }

            var avatarImage = await _imageStorageService.SaveImage(userRegistrationDto.Avatar);

            var user = new User()
            {
                Email = userRegistrationDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userRegistrationDto.Password),
                FirstName = userRegistrationDto.FirstName,
                LastName = userRegistrationDto.LastName,
                CreationDate = _dateTime.Now,
                IsEmailConfirmed = true //TODO: Create an email confirmation solution
            };

            if (avatarImage != null)
            {
                user.Images = new List<Image>(new[] { avatarImage });
            }

            _applicationDbContext.Users.Add(user);

            await _applicationDbContext.SaveChangesAsync(CancellationToken.None);

            return Result<UserVm>.Created(_mapper.Map<User, UserVm>(user)); //201
        }
    }
}
