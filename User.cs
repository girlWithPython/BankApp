using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpleController;

public class User
{
    public Guid UserId { get; init; }
        
    [MaxLength(30)]
    public required string FirstName { get; set; }
        
    [MaxLength(30)]
    public required string LastName { get; set; }
        
    [MaxLength(20)]
    public required string Username { get; set; }
        
    [MaxLength(20)]
    public required string Email { get; set; }
        
    [MaxLength(15)]
    public required string PhoneNumber { get; set; }
        
    public DateTime CreatedOn { get; init; }
        
    public ICollection<Account> Accounts { get; set; }
}