using Microsoft.AspNetCore.Mvc;

namespace SimpleController;

[Route("api/[controller]")]
[ApiController]

public class BankAccountTypeController : ControllerBase
{


    private readonly ApplicationDbContext _dbContext;
        
    public BankAccountTypeController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult GetBankAccountTypes()
    {
        return Ok(_dbContext.BankAccountType);
    }

    [HttpGet]
    [Route("{BankAccountTypeId:int}")]
    public IActionResult GetBankAccountTypeById(BankAccountType bankAccountTypeId)
    {
        var account = _dbContext.BankAccountType.Find(bankAccountTypeId);
        if (account == null)
        {
            return NotFound();
        }
        return Ok(account);
    }

    [HttpPost]
    public IActionResult AddBankAccountType(BankAccountTypeDto bankAccountTypeDto)
    {
        var bankAccountType = new BankAccountType
        {
            BankAccountTypeName = bankAccountTypeDto.BankAccountTypeName
        };

        _dbContext.BankAccountType.Add(bankAccountType); // Correct DbSet
        _dbContext.SaveChanges();
        return StatusCode(StatusCodes.Status201Created);
    }


        
    [HttpDelete("{bankAccountTypeId:int}")]
    public IActionResult DeleteBankAccountType(int bankAccountTypeId)
    {
        // Find the BankAccountType by ID
        var bankAccountType = _dbContext.BankAccountType.Find(bankAccountTypeId);

        if (bankAccountType == null)
        {
            // Return 404 if the BankAccountType does not exist
            return NotFound($"BankAccountType with ID {bankAccountTypeId} not found.");
        }

        // Remove the BankAccountType from the DbContext
        _dbContext.BankAccountType.Remove(bankAccountType);
        _dbContext.SaveChanges();

        // Return 204 No Content after successful deletion
        return NoContent();
    }

    
}