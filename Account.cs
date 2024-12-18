using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SimpleController;

public class Account
{
   public Guid AccountId { get; init; }
    
   public Guid UserId { get; set; }
   public User User { get; init; }
   
   public int AccountNumber { get; set; }
   
   public int BankAccountTypeId { get; set; } // Foreign Key to AccountType
   public BankAccountType BankAccountType { get; set; } = null!;
   
   [Precision(10,2)]
   public decimal Balance { get; set; }
   
   public byte[] RowVersion { get; set; }
   public DateTime CreatedOn { get; set; }
   
   public DateTime LastChanged { get; set; }
}