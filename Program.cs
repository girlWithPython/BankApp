using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.WebSockets;

using SimpleController;

var builder = WebApplication.CreateBuilder(args);
// Register the AccountService to the DI container
builder.Services.AddScoped<IAccountService, AccountService>();
// Add JWT authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);
builder.Services.AddLogging(logging =>
{
    logging.SetMinimumLevel(LogLevel.Debug);
    logging.AddConsole();
});
builder.Services.AddSignalR();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = "YourIssuer",
            ValidAudience = "YourAudience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKey1234567890123456HopeLongEnough")),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine($"Token validated for user: {context.Principal.Identity.Name}");
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();
// Add services to the container.
builder.Services.AddScoped<UserRegistrationService>();

builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Setup Entity Framework and Identity
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure CORS to allow requests from your frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", 
        corsBuilder => corsBuilder.WithOrigins("http://localhost:3000") // Frontend URL
            .AllowAnyHeader()
            .AllowCredentials()
            .AllowAnyMethod());
});
builder.Services.AddScoped<TransactionService>();
// Add controllers
builder.Services.AddControllers();

// Build the application
var app = builder.Build();
// Enable WebSocket support globally
app.UseWebSockets();  // This is for enabling WebSockets globally across the app

app.UseRouting();
// Use CORS policy
app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Logger.LogInformation("Application started");
app.UseAuthentication(); // Add Authentication middleware
app.UseAuthorization();  // Add Authorization middleware
/*app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); 
});*/
app.MapHub<NotificationHub>("/notificationHub"); // Map SignalR hub

app.MapControllers(); // Map controller routes

app.Run();
