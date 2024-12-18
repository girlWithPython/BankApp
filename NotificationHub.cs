namespace SimpleController;

using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;
   
    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    
    public async Task SendNotification(string userId, string message)
    {
        // Sends a notification to a specific client identified by their connection ID
        _logger.LogInformation($"Sending message to clients: {message}");
        await Clients.User(userId).SendAsync("ReceiveNotification", message);
    }
}
