namespace SimpleController;

public class AccountDto
{
    public required Guid UserId { get; set; }
    public required int BankAccountTypeId { get; set; }
    public required decimal Balance { get; set; }
        
           
}