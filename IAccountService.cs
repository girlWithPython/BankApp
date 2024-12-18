namespace SimpleController;

public interface IAccountService
{
    List<Account> GetAccountsByUserId(string userId);
}
