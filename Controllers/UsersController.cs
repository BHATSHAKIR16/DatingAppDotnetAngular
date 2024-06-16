using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] //api/Users
    public class UsersController : ControllerBase
    {
        //create this field so that datacontext is available throughtout the class and not only in the constructor
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

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
    }
}
