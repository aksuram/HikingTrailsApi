﻿using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HikingTrailsApi.WebApi.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IApplicationIdentityManager _applicationIdentityManager;

        public IdentityController(IApplicationIdentityManager applicationIdentityManager)
        {
            _applicationIdentityManager = applicationIdentityManager;
        }

        //[AllowAnonymous]
        //[HttpPost("api/login")]
        //public async Task<ActionResult> Login([FromBody] UserLoginRequest userLoginRequest)
        //{
        //    string token = null;
        //    try
        //    {
        //        token = await _applicationIdentityManager.ApiLogIn(userLoginRequest.Username, userLoginRequest.Password);
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest("Nepavyko prisijungti");
        //    }

        //    if (token == null)
        //    {
        //        return BadRequest("Nepavyko prisijungti");
        //    }

        //    return Ok(token);
        //}

        //[AllowAnonymous]
        [HttpPost("api/register")]
        public async Task<ActionResult> Register([FromBody] UserRegistrationDto userRegistrationDto)
        {
            var result = await _applicationIdentityManager.RegisterUser(userRegistrationDto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
