using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TDSSWebApplication.Common;
using TDSSWebApplication.IOC;
using TDSSWebApplication.IServices;
using TDSSWebApplication.Models;
using TDSSWebApplication.Services;
using Serilog; 

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Set minimum log level
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    //.Filter.ByExcluding(log => log.Properties.ContainsKey("Password"))
    .CreateLogger();

// Replace the default .NET Core logger with Serilog
builder.Host.UseSerilog();

// Ensure configuration is set up
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var secretKey = builder.Configuration["Jwt:Key"];
    var validissuer = builder.Configuration["Jwt:Issuer"];
    var validaudience = builder.Configuration["Jwt:Audience"];

    // Check if the secretKey is null and throw a descriptive error if it is
    if (string.IsNullOrEmpty(secretKey))
    {
        throw new ArgumentNullException(nameof(secretKey), "JWT Secret key is missing in configuration");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = validissuer,
        ValidAudience = validaudience

    };
});


builder.Services.AddAuthorization(); 
// Add Services
builder.Services.RegisterServices();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<LinenManagementContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("LinenDBConnection"));
});

var app = builder.Build();

// Register the Global Error Handler Middleware
app.UseMiddleware<GlobalErrorHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();


app.MapControllers();

try
{
    Log.Information("Starting up the API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
