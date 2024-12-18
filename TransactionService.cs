using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SimpleController;
public class TransactionService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHubContext<NotificationHub> _hubContext;

    public TransactionService(ApplicationDbContext dbContext, IHubContext<NotificationHub> hubContext)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
    }

    public async Task<(bool Success, string Message)> TransferFundsAsync(Guid senderAccountId, Guid recipientAccountId, decimal amount)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var senderAccount = await _dbContext.Accounts
                .SingleOrDefaultAsync(a => a.AccountId == senderAccountId);

            var recipientAccount = await _dbContext.Accounts
                .SingleOrDefaultAsync(a => a.AccountId == recipientAccountId);

            if (senderAccount == null || recipientAccount == null)
            {
                return (false, "Invalid account information.");
            }

            if (senderAccount.Balance < amount)
            {
                return (false, "Insufficient funds.");
            }

            // Deduct from sender
            senderAccount.Balance -= amount;
            senderAccount.LastChanged = DateTime.UtcNow;

            // Add to recipient
            recipientAccount.Balance += amount;
            recipientAccount.LastChanged = DateTime.UtcNow;

            // Record transaction
            var transactionRecord = new Transaction
            {
                SenderAccountId = senderAccountId,
                RecipientAccountId = recipientAccountId,
                Amount = amount,
                Status = "Completed",
                Timestamp = DateTime.UtcNow
            };
            // Retrieve the AspNetUserId using the UserId from the senderAccount
            var senderAspNetUserId = await _dbContext.UserClientMaps
                .Where(map => map.UserId == senderAccount.UserId)
                .Select(map => map.AspNetUserId)
                .FirstOrDefaultAsync();

            _dbContext.Transactions.Add(transactionRecord);

            // Save changes and commit
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            // Notify Sender
            await _hubContext.Clients.User(senderAspNetUserId.ToString())
                .SendAsync("ReceiveNotification", $"Transaction of {amount:C} to {recipientAccountId} was successful.");

            // Notify Recipient
            await _hubContext.Clients.User(recipientAccount.UserId.ToString())
                .SendAsync("ReceiveNotification", $"You have received {amount:C} from {senderAccountId}.");

            return (true, "Transaction successful.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Transaction failed: {ex.Message}");
        }
    }
}
