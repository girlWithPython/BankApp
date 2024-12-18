using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SimpleController;

[Route("api/[controller]")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly TransactionService _transactionService;

    public TransactionController(TransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    // POST: api/Transaction
    [HttpPost]
    public async Task<IActionResult> TransferFunds([FromBody] TransferRequest request)
    {
        if (request == null || request.Amount <= 0)
        {
            return BadRequest("Invalid request payload.");
        }

        var result = await _transactionService.TransferFundsAsync(
            request.SenderAccountId,
            request.RecipientAccountId,
            request.Amount
        );

        if (result.Success)
        {
            return Ok(result.Message);
        }
        else
        {
            return BadRequest(result.Message);
        }
    }
}

// Request model
public class TransferRequest
{
    public Guid SenderAccountId { get; set; }
    public Guid RecipientAccountId { get; set; }
    public decimal Amount { get; set; }
}