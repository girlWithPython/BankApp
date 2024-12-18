namespace SimpleController;

public class Transaction
{
    public Guid TransactionId { get; set; } = Guid.NewGuid(); // Unique transaction identifier
    public Guid SenderAccountId { get; set; } // FK to Sender Account
    public Guid RecipientAccountId { get; set; } // FK to Recipient Account
    public decimal Amount { get; set; } // Transaction amount
    public string Status { get; set; } = "Pending"; // Status of the transaction
    public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Creation timestamp
}