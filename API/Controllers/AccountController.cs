﻿using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController:BaseAPIController
    {
        private readonly ITokenService _tokenService;
        private readonly DataContext _context;

        public AccountController(DataContext context, ITokenService tokenService) {


            _tokenService = tokenService;
            _context = context;

        }

        [HttpPost("register")] //  POST api/account/register
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if(await UserExists(registerDTO.Username)) { 
                return BadRequest("Username is taken");
            
            }
            using var hmac = new HMACSHA512();

            var user = new AppUser {
                UserName = registerDTO.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return new UserDTO { Username= user.UserName, Token = _tokenService.CreateToken(user) };
        }

        [HttpPost("login")]

        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _context
                .Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.Username);

            if(user == null) { return Unauthorized("Invalid Username"); }

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            if (!user.PasswordHash.SequenceEqual(computedHash))
            {
                return Unauthorized("Invalid Password");
            }

            return new UserDTO { Username = user.UserName, Token = _tokenService.CreateToken(user) };
        }
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
