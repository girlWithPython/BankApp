using Microsoft.AspNetCore.Identity;

namespace SimpleController;

public class UserClientMap
{
    public Guid AspNetUserId { get; set; } // Guid type
    public Guid UserId { get; set; }

    public ApplicationUser AspNetUser { get; set; }
    public User User { get; set; }
}