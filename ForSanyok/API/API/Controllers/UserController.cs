using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext _db;
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger, UserContext users)
        {
            _logger = logger;
            _db = users;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var user = await _db.Users.ToListAsync();
            return Ok(user);
        }

        [HttpGet("{firstName}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByFirstName(string firstName)
        {
            var user = await _db.Users.Where(x => x.FirstName == firstName).ToListAsync();
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<User>>> CreateNewUser(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IEnumerable<User>>> EditUser([FromBody] User user, [FromRoute] int id)
        {
            var userTemplate = _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            user.Id = id;
            _db.Update(user);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<IEnumerable<User>>> DeleteUser(int id)
        {
            var userTemplate = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (userTemplate == null) return NotFound();
            _db.Users.Remove(userTemplate);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
