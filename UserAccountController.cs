using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SimpleController;

[ApiController]
[Route("api/[controller]")]
public class UserAccountController : ControllerBase
{
    private readonly UserRegistrationService _userRegistrationService;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;  // Use ApplicationUser
    private readonly SignInManager<ApplicationUser> _signInManager;  // Inject SignInManager
    private readonly ApplicationDbContext _dbContext;

    public UserAccountController(UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager, // Inject SignInManager
                                  UserRegistrationService userRegistrationService,
                                  IConfiguration configuration,
                                  ApplicationDbContext dbContext)
    {
        _userRegistrationService = userRegistrationService;
        _configuration = configuration;
        _userManager = userManager;
        _signInManager = signInManager;  // Assign SignInManager
        _dbContext = dbContext;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if the email already exists
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return BadRequest(new { code = "DuplicateUserName", description = $"Username '{model.Email}' is already taken." });
        }

        // Create a new IdentityUser
        var user = new ApplicationUser  // Use ApplicationUser instead of IdentityUser
        {
            UserName = model.Email.ToLower(),
            PhoneNumber = model.Phone,
            Email = model.Email.ToLower()
        };

        // Create the user in ASP.NET Identity
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => new { code = e.Code, description = e.Description }));
        }

        // Register the user in the Users table and map to UserClientMap
        var userDto = new UserDto
        {
            FirstName = user.UserName,
            LastName = user.UserName,
            Username = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        try
        {
            await _userRegistrationService.RegisterUserAndMapClientAsync(userDto, user.Id);
        }
        catch (Exception ex)
        {
            await _userManager.DeleteAsync(user);
            return StatusCode(500, new { message = "An error occurred during user registration.", details = ex.Message });
        }

        // Fetch mapping table UserId
        var mappingTableUserId = await GetMappingTableUserIdAsync(user.Id);
        if (mappingTableUserId == Guid.Empty)
        {
            return Unauthorized("No associated mapping record found.");
        }

        // Generate the token with the mapping table UserId
        var token = await GenerateJwtTokenAsync(user, mappingTableUserId);
        return Ok(new { Token = token });
    }

    // Modify GenerateJwtTokenAsync to accept mappingTableUserId
    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user, Guid mappingTableUserId)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("UserId", mappingTableUserId.ToString())  // Add UserId from mapping table
        };

        // Add roles as claims
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Modify GetMappingTableUserIdAsync to get UserId from the mapping table
    private async Task<Guid> GetMappingTableUserIdAsync(Guid aspNetUserId)
    {
        // Assuming your mapping table is named "UserClientMap" and has a property "AspNetUserId"
        var mappingRecord = await _dbContext.UserClientMaps
            .FirstOrDefaultAsync(x => x.AspNetUserId == aspNetUserId);

        return mappingRecord?.UserId ?? Guid.Empty; // Return Guid.Empty if no record is found
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return Unauthorized("Invalid username or password");
        }

        // Fetch mapping table UserId
        var mappingTableUserId = await GetMappingTableUserIdAsync(user.Id);
        if (mappingTableUserId == Guid.Empty)
        {
            return Unauthorized("No associated mapping record found.");
        }

        // Generate the token with the mapping table UserId
        var token = await GenerateJwtTokenAsync(user, mappingTableUserId);
        return Ok(new { Token = token });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // Use SignInManager to log out the user
        await _signInManager.SignOutAsync();  // This is where SignOutAsync is used

        return Ok("Logged out successfully");
    }
}
