using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleController;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(u => u.CreatedOn)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Account>()
            .HasOne(a => a.User)
            .WithMany(u => u.Accounts)
            .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<Account>()
            .Property(a => a.CreatedOn)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Account>()
            .Property(a => a.RowVersion)
            .IsRowVersion();

        modelBuilder.Entity<Account>(entity =>
        {
            entity.Property(a => a.CreatedOn)
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();

            // Configure AccountNumber as auto-increment with seed and increment
            entity.Property(a => a.AccountNumber)
                .UseIdentityColumn(260000, 1); // Starts at 260000, increment by 1

            // Make AccountNumber a unique key
            entity.HasIndex(a => a.AccountNumber)
                .IsUnique();
                
            // Relationship with AccountType
            entity.HasOne(a => a.BankAccountType)
                .WithMany()
                .HasForeignKey(a => a.BankAccountTypeId)
                .IsRequired();

            // Relationship with User
            entity.HasOne(a => a.User)
                .WithMany(u => u.Accounts)
                .HasForeignKey(a => a.UserId)
                .IsRequired();
        });

        modelBuilder.Entity<UserClientMap>()
            .HasKey(ucm => new { ucm.AspNetUserId, ucm.UserId });

        modelBuilder.Entity<UserClientMap>()
            .HasOne(ucm => ucm.AspNetUser)
            .WithMany()  // Assuming no navigation in ApplicationUser for this relationship
            .HasForeignKey(ucm => ucm.AspNetUserId)
            .IsRequired();

        modelBuilder.Entity<UserClientMap>()
            .HasOne(ucm => ucm.User)
            .WithMany()  // Assuming no navigation in User for this relationship
            .HasForeignKey(ucm => ucm.UserId)
            .IsRequired();
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.TransactionId);

            entity.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne<Account>()
                .WithMany()
                .HasForeignKey(t => t.SenderAccountId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

            entity.HasOne<Account>()
                .WithMany()
                .HasForeignKey(t => t.RecipientAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    public DbSet<User> Users { get; init; }
    public DbSet<Account> Accounts { get; init; }
    public DbSet<BankAccountType> BankAccountType { get; init; }
    public DbSet<UserClientMap> UserClientMaps { get; init; }
    
    public DbSet<Transaction> Transactions { get; set; }
}