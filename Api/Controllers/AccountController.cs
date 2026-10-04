using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Api.Data;
using Api.DTOs;
using Api.Entities;
using Api.Extensions;
using Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    public class AccountController(AppDbContext context, ITokenService tokenService):BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await emailExists(registerDto.Email)) return BadRequest("Email taken");

            using var hmac=new HMACSHA512();
            var user=new AppUser
            {
                DisplayName=registerDto.DisplayName,
                Email=registerDto.Email,
                PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt=hmac.Key
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user.ToDto(tokenService);
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user= await context.Users.SingleOrDefaultAsync(x => x.Email.ToLower() == loginDto.Email);

            if (user == null) return Unauthorized("Invalid email addresss");
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(var i = 0; i< computedHash.Length; i++)
            {
                if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");

            }

            return user.ToDto(tokenService);
        }

        private async Task<Boolean> emailExists(string email)
        {
            return await context.Users.AnyAsync(x => x.Email.ToLower()==email.ToLower());
        }
    }
}