using Microsoft.AspNetCore.Mvc;

namespace SimpleController.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserClientMapController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
        
    public UserClientMapController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
        
    [HttpGet]
    public IActionResult GetAllRecords()
    {
        return Ok(_dbContext.Users);
    }
}