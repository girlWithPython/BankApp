namespace SimpleController;

public class BankAccountType
{
    public int BankAccountTypeId { get; set; } // Primary Key
    public string BankAccountTypeName { get; set; } = string.Empty; // Account type name (e.g., Savings, Checking)
}