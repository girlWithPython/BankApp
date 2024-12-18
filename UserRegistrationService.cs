using Microsoft.EntityFrameworkCore;

namespace SimpleController;

public class UserRegistrationService
{
    private readonly ApplicationDbContext _context;

    public UserRegistrationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task RegisterUserAndMapClientAsync(UserDto userDto, Guid aspNetUserId)
    {
        if (userDto == null)
        {
            throw new ArgumentNullException(nameof(userDto), "User DTO cannot be null.");
        }

        // Check if email exists in the Users table
        var existingClient = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == userDto.Email);

        Guid clientUserId;

        if (existingClient != null)
        {
            // Email found: Use existing Client UserId
            clientUserId = existingClient.UserId;
        }
        else
        {
            // Email not found: Add a new record to Users table
            var newClient = new User
            {
                UserId = Guid.NewGuid(), // Generate new GUID for the user
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                PhoneNumber = userDto.PhoneNumber,
                Username = userDto.Username
            };

            _context.Users.Add(newClient);
            await _context.SaveChangesAsync();

            clientUserId = newClient.UserId;
        }

        // Check if a mapping already exists in UserClientMap
        var existingMapping = await _context.UserClientMaps
            .AnyAsync(ucm => ucm.AspNetUserId == aspNetUserId && ucm.UserId == clientUserId);

        if (!existingMapping)
        {
            // Add a new mapping to UserClientMap
            var userClientMap = new UserClientMap
            {
                AspNetUserId = aspNetUserId,
                UserId = clientUserId
            };

            _context.UserClientMaps.Add(userClientMap);
            await _context.SaveChangesAsync();
        }
    }

}