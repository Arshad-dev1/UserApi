using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserApi.DB;
using UserApi.DB.Models;
using UserApi.Models;

namespace UserApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(UserDbContext db) : ControllerBase
    {

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            try
            {
                var users = await db.Users
                    .AsNoTracking()
                    .OrderByDescending(u => u.Id)
                    .Select(u => new UserResponse(u.Id, u.Name, u.Age, u.City, u.State, u.Pincode))
                    .ToListAsync(ct);
                return Ok(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving users: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
            return user is null ? NotFound() : Ok(UserResponse.From(user));
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(UserRequest request, CancellationToken ct)
        {
            var user = new User();
            Apply(user, request);

            db.Users.Add(user);
            await db.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, UserResponse.From(user));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, UserRequest request, CancellationToken ct)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user is null) return NotFound();

            Apply(user, request);
            await db.SaveChangesAsync(ct);

            return Ok(UserResponse.From(user));
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user is null) return NotFound();

            db.Users.Remove(user);
            await db.SaveChangesAsync(ct);

            return NoContent();
        }
        private static void Apply(User user, UserRequest r)
        {
            user.Name = r.Name.Trim();
            user.Age = r.Age;
            user.City = r.City.Trim();
            user.State = r.State.Trim();
            user.Pincode = r.Pincode.Trim();
        }


    }
}
