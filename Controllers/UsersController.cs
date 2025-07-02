using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using System.Collections.Generic;
using System.Linq;
using UserManagementAPI.Data;

namespace UserManagementAPI.Controllers
{
    /// <summary>
    /// Controller for managing user records in the User Management API.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
#if DEBUG
            System.Diagnostics.Debug.WriteLine("UsersController initialized in DEBUG mode.");
#endif
        }

        /// <summary>
        /// Retrieves all users in the registry.
        /// </summary>
        /// <param name="skip">Number of users to skip (for pagination).</param>
        /// <param name="take">Number of users to return (for pagination).</param>
        /// <returns>List of users with pagination.</returns>
        [HttpGet]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client)]
        public ActionResult<IEnumerable<User>> GetAllUsers([FromQuery] int skip = 0, [FromQuery] int take = 20)
        {
#if DEBUG
            if (skip < 0 || take < 0)
            {
                System.Diagnostics.Debugger.Break();
            }
#endif
            if (take > 100) take = 100; // Limit max page size
            var users = _context.Users.Skip(skip).Take(take).ToList();
            _logger.LogDebug("Returned {Count} users (skip={Skip}, take={Take})", users.Count, skip, take);
            return Ok(users);
        }

        /// <summary>
        /// Retrieves a user by their unique ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>The user with the specified ID, or 404 if not found.</returns>
        [HttpGet("{id}")]
        public ActionResult<User> GetUserById(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found.", id);
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
                return NotFound();
            }
            return Ok(user);
        }

        /// <summary>
        /// Creates a new user in the registry.
        /// </summary>
        /// <param name="user">The user object to create.</param>
        /// <returns>The created user with assigned ID.</returns>
        [HttpPost]
        public ActionResult<User> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Invalid user data: {@Errors}", errors);
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
                return BadRequest(new { message = "Invalid user data.", errors });
            }
            // Check for duplicate email
            var emailToCheck = user.Email != null ? user.Email.ToLower() : null;
            if (_context.Users.Any(u => u.Email != null && u.Email.ToLower() == emailToCheck))
            {
                _logger.LogWarning("Duplicate email attempted: {Email}", user.Email);
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
                return Conflict(new { message = "A user with this email already exists." });
            }
            _context.Users.Add(user);
            _context.SaveChanges();
            _logger.LogInformation("User created: {@User}", user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        /// <summary>
        /// Updates an existing user by ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="updatedUser">The updated user object.</param>
        /// <returns>No content if successful, or 404 if not found.</returns>
        [HttpPut("{id}")]
        public ActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Invalid user update data: {@Errors}", errors);
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
                return BadRequest(new { message = "Invalid user data.", errors });
            }
            var user = _context.Users.Find(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found for update.", id);
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
                return NotFound();
            }
            // Check for duplicate email (if changed)
            var updatedEmailToCheck = updatedUser.Email != null ? updatedUser.Email.ToLower() : null;
            if (!string.Equals(user.Email, updatedUser.Email, System.StringComparison.OrdinalIgnoreCase)
                && _context.Users.Any(u => u.Email != null && u.Email.ToLower() == updatedEmailToCheck))
            {
                _logger.LogWarning("Duplicate email attempted on update: {Email}", updatedUser.Email);
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
                return Conflict(new { message = "A user with this email already exists." });
            }
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.Department = updatedUser.Department;
            _context.SaveChanges();
            _logger.LogInformation("User updated: {@User}", user);
            return NoContent();
        }

        /// <summary>
        /// Deletes a user by their unique ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>No content if successful, or 404 if not found.</returns>
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found for deletion.", id);
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
                return NotFound();
            }
            _context.Users.Remove(user);
            _context.SaveChanges();
            _logger.LogInformation("User deleted: {@User}", user);
            return NoContent();
        }
    }
}
