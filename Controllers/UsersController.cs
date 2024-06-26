using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        //create this field so that datacontext is available throughtout the class and not only in the constructor
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> getUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getUser(int id) 
        {
            var user = await _context.Users.FindAsync(id);
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> register(string username, string password)
        {
            using var hmac = new HMACSHA512();

            var users = new AppUser()
            {
                UserName = username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(users);
            await _context.SaveChangesAsync();
            return Ok(users);
        }
    }
}
