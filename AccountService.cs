namespace SimpleController;

public class AccountService : IAccountService
{
    private readonly ApplicationDbContext _context; // Your database context

    public AccountService(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Account> GetAccountsByUserId(string userId)
    {
        // Convert userId (string) to Guid
        if (Guid.TryParse(userId, out Guid userGuid))
        {
            // Fetch accounts based on userId (Guid) from the database
            return _context.Accounts.Where(a => a.UserId == userGuid).ToList();
        }
        else
        {
            // If userId is invalid (not a valid Guid), return empty or handle as needed
            return new List<Account>();
        }
    }

}
