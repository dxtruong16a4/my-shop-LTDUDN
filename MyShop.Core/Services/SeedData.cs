using MyShop.Core.Data;
using MyShop.Core.Entities;
using MyShop.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MyShop.Core.Services
{
    /// <summary>
    /// Service to seed initial data (admin user) into the database
    /// </summary>
    public class SeedData
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHelper _passwordHelper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SeedData> _logger;

        public SeedData(AppDbContext context, IPasswordHelper passwordHelper, IConfiguration configuration, ILogger<SeedData> logger)
        {
            _context = context;
            _passwordHelper = passwordHelper;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Seeds the database with default admin user if it doesn't exist
        /// </summary>
        public async Task SeedAdminAsync()
        {
            try
            {
                // Ensure database is created
                await _context.Database.EnsureCreatedAsync();
                await _context.Database.MigrateAsync();

                // Check if admin user already exists
                var adminUsername = _configuration["DefaultAdmin:Username"];
                var adminExists = await _context.Set<User>().AnyAsync(u => u.Username == adminUsername);

                if (adminExists)
                {
                    _logger.LogInformation("Admin user '{AdminUsername}' already exists. Skipping seed.", adminUsername);
                    return;
                }

                // Read admin credentials from configuration
                var adminEmail = _configuration["DefaultAdmin:Email"];
                var adminPassword = _configuration["DefaultAdmin:Password"];

                if (string.IsNullOrWhiteSpace(adminUsername) || string.IsNullOrWhiteSpace(adminPassword) || string.IsNullOrWhiteSpace(adminEmail))
                {
                    _logger.LogWarning("Default admin configuration is incomplete. Skipping admin creation.");
                    return;
                }

                // Create admin user
                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = adminUsername,
                    Email = adminEmail,
                    PasswordHash = _passwordHelper.HashPassword(adminPassword),
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Set<User>().Add(adminUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin user '{AdminUsername}' created successfully.", adminUsername);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database with admin user.");
                throw;
            }
        }
    }
}
