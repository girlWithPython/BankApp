using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace SimpleController.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
        
    public UserController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers(string search)
    {
        var currentUserEmail = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;    
        if (string.IsNullOrEmpty(search))
        {
            return BadRequest("Search parameter cannot be empty.");
        }

        var users = await _dbContext.Users
            .Where(u => u.Email != currentUserEmail && EF.Functions.Like(u.Email, $"%{search}%"))
            .Select(u => new { u.UserId, u.Email })  // Return UserId along with Email
            .ToListAsync();

        return Ok(users);
    }

    [HttpPost]
    public IActionResult AddUser(UserDto userDto)
    {
        var user = new User
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Username = userDto.Username,
            Email = userDto.Email,
            PhoneNumber = userDto.PhoneNumber
                
               
        };
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut]
    [Route("{userId:guid}")]
    public IActionResult UpdateUser(Guid userId, UpdateUserDto userDto)
    {
        var user = _dbContext.Users.Find(userId);
        if (user == null)
        {
            return NotFound();
        }
        user.FirstName = userDto.FirstName ?? throw new ArgumentNullException(nameof(userDto.FirstName));
        user.LastName = userDto.LastName ?? string.Empty;
        user.Email = userDto.Email ?? string.Empty;
        user.PhoneNumber = userDto.PhoneNumber ?? string.Empty;
            
        _dbContext.SaveChanges();
        return Ok(user);
    }

    [HttpDelete]
    [Route("{userId:guid}")]
    public IActionResult DeleteUser(Guid userId)
    {
        var user = _dbContext.Users.Find(userId);
        if (user == null)
        {
            return NotFound();
        }
        _dbContext.Users.Remove(user);
        _dbContext.SaveChanges();
        return NoContent();
    }
}