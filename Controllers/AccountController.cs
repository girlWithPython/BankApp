using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace SimpleController.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]

public class AccountController : ControllerBase
{

        private readonly IAccountService _accountService;
        private readonly ApplicationDbContext _dbContext;
        
        public AccountController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _accountService = new AccountService(_dbContext);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult>  GetAccounts()
        {
            // Get the logged-in user's UserId from the claims
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (userId == null)
                return Unauthorized();

            // Fetch accounts for the logged-in user
            var accounts = await _dbContext.Accounts
                .Where(a => a.UserId == Guid.Parse(userId)) // Filter by UserId
                .ToListAsync();

            return Ok(accounts);
        }

        [HttpPost("getByUserId")]
        public IActionResult GetAccountsByUserId([FromBody] UserIdRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest("UserId is required.");
            }

            var accounts = _accountService.GetAccountsByUserId(request.UserId);
            if (accounts == null || accounts.Count == 0)
            {
                return NotFound("No accounts found for this user.");
            }

            return Ok(accounts);
        }

        public class UserIdRequest
        {
            public string UserId { get; set; }
        }

        [HttpPost]
        public IActionResult AddAccount(AccountDto accountDto)
        {
            var account = new Account
            {
                UserId = accountDto.UserId,
                BankAccountTypeId = accountDto.BankAccountTypeId,
                Balance = accountDto.Balance
                
               
            };
            _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet]
        [Authorize]
        [Route("{accountTypeId}")]
        public async Task<IActionResult> GetAccountsByType([FromRoute] int accountTypeId)
        {
            // Get the logged-in user's UserId from the claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "User is not authorized" });
            }

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid UserId in claims" });
            }

            // Fetch accounts for the logged-in user and filter by accountType
            var accounts = await _dbContext.Accounts
                .Where(a => a.UserId == userId && a.BankAccountTypeId == accountTypeId)
                .ToListAsync();

            if (!accounts.Any())
            {
                return NotFound(new { message = "No accounts found for the given type" });
            }

            return Ok(accounts);
        }
        
        [HttpDelete]
        [Route("{accountsId:guid}")]
        public IActionResult DeleteAccount(Guid accountId)
        {
            var account = _dbContext.Accounts.Find(accountId);
            if (account == null)
            {
                return NotFound();
            }
            _dbContext.Accounts.Remove(account);
            _dbContext.SaveChanges();
            return NoContent();
        }
    
}