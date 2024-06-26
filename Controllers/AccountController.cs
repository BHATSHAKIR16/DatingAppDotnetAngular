using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly DataContext _context;
        private readonly ItokenService _tokenService;

        public AccountController(DataContext context, ItokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> register(RegisterDto register)
        {
            if (await checkIfExists(register.Username))
            {
                return BadRequest("user already exists");
            }
            using var hmac = new HMACSHA512() ;

            var user = new AppUser()
            {
                UserName = register.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            //check if the user exists if not return unauthorised
            var user =await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username);
            if (user == null)
            {
                return Unauthorized("invalid userName");
            }
            //generate the same hash , we do so by providing the same salt that the user has
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            //check if the passwordhash is matching with the hash present in db
           for(int i = 0; i< computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("invalid Password");
                }
            }
            return Ok(new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            });
        }
        private async Task<bool> checkIfExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
