using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Entities;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserServiceContext _context;

        public UsersController(UserServiceContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            return await _context.Users
                .Select(u => new UserDTO(u))
                .ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new UserDTO(user));
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UserUpdateModel userUdpate)
        {
            if (id != UserService.Entities.User.ComputeId(userUdpate.Username, userUdpate.Email))
                return BadRequest();

            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            
            user.Username = userUdpate.Username;
            user.Email = userUdpate.Email;
            user.SetPassword(userUdpate.Pass);

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                if (!await UserExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> CreateUser(UserUpdateModel userPayload)
        {
            var s = await this.UserExists(UserService.Entities.User.ComputeId(userPayload.Username, userPayload.Email));
            if (s is not bool)
                return Conflict(s);
            if (s)
                return Conflict("Account already exists");

            var user = new User(userPayload.Username, userPayload.Email, userPayload.Pass);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(UserLoginModel userLogin)
        {

            var user = await _context.Users.Where(u=> u.Username == userLogin.Login || u.Email == userLogin.Login).FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            if (user.CheckPassword(userLogin.Pass))
                return Ok(new UserDTO(user));
            
            return NotFound();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<dynamic> UserExists(string id)
        {
            var usernameExists = await _context.Users.AnyAsync(e => e.Id.Substring(0, 64) == id.Substring(0, 64));
            var emailExists = await _context.Users.AnyAsync(e => e.Id.Substring(64, 64) == id.Substring(64, 64));

            if (usernameExists && emailExists)
                return true;
            if (usernameExists)
                return "Username already taken";
            if (emailExists)
                return "Email already exists";
            return false;
        }
    }
}