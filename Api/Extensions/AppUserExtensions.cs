using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs;
using Api.Entities;
using Api.Interfaces;

namespace Api.Extensions
{
    public static class AppUserExtensions
    {
        public static UserDto ToDto(this AppUser user, ITokenService tokenService)
        {
            return new UserDto
            {
                Id=user.Id,
                DisplayName=user.DisplayName,
                Email=user.Email,
                Token=tokenService.CreateToken(user)

            };
        }
    }
}