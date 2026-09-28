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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("User ID must be greater than 0.");

                var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
                return user is null ? NotFound() : Ok(UserResponse.From(user));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving the user: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while processing your request: {ex.Message}");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(UserRequest request, CancellationToken ct)
        {
            if (request == null)
                return BadRequest("Request body cannot be empty.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var user = new User();
                Apply(user, request);

                db.Users.Add(user);
                await db.SaveChangesAsync(ct);

                return CreatedAtAction(nameof(GetById), new { id = user.Id }, UserResponse.From(user));
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database error while creating user: {dbEx.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while saving the user to the database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the user: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while processing your request: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, UserRequest request, CancellationToken ct)
        {
            if (id <= 0)
                return BadRequest("User ID must be greater than 0.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (request == null)
                return BadRequest("Invalid user data.");

            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user is null)
                return NotFound();

            Apply(user, request);
            await db.SaveChangesAsync(ct);

            return Ok(UserResponse.From(user));
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            if (id <= 0)
                return BadRequest("User ID must be greater than 0.");

            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user is null)
                return NotFound();

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
