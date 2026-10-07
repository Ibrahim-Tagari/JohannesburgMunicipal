using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Municipal_App.Data;
using Municipal_App.Models;

namespace Municipal_App.Services
{
    public class DatabaseManagementService : IDatabaseManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseManagementService> _logger;
        private const string BackupDirectory = "Backups";

        public DatabaseManagementService(ApplicationDbContext context, ILogger<DatabaseManagementService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> InitializeDatabaseAsync()
        {
            try
            {
                await _context.Database.EnsureCreatedAsync();
                _logger.LogInformation("Database initialized successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing database");
                return false;
            }
        }

        public async Task<DatabaseInfo> GetDatabaseInfoAsync()
        {
            try
            {
                var info = new DatabaseInfo
                {
                    DatabaseName = _context.Database.GetDbConnection().Database,
                    DatabaseType = _context.Database.ProviderName ?? "Unknown",
                    Provider = _context.Database.ProviderName ?? "Unknown",
                    IsConnected = await _context.Database.CanConnectAsync(),
                    IsHealthy = true,
                    LastUpdated = DateTime.Now,
                    LastModified = DateTime.Now,
                    Status = "Connected"
                };

                info.UserCount = await _context.Users.CountAsync();
                info.IssueCount = await _context.Issues.CountAsync();
                info.EventCount = await _context.LocalEvents.CountAsync();
                info.NotificationCount = await _context.Notifications.CountAsync();

                info.TotalUsers = info.UserCount;
                info.TotalIssues = info.IssueCount;
                info.TotalEvents = info.EventCount;
                info.TotalNotifications = info.NotificationCount;

                var connection = _context.Database.GetDbConnection();
                if (connection.DataSource != null && File.Exists(connection.DataSource))
                {
                    var fileInfo = new FileInfo(connection.DataSource);
                    info.DatabasePath = fileInfo.FullName;
                    info.Size = fileInfo.Length;
                }

                info.Statistics["TotalRecords"] = info.UserCount + info.IssueCount + info.EventCount + info.NotificationCount;
                info.Statistics["LastUpdated"] = DateTime.Now;

                return info;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting database info");
                return new DatabaseInfo
                {
                    DatabaseName = "Error",
                    DatabaseType = "Unknown",
                    IsHealthy = false,
                    IsConnected = false,
                    Status = "Error: " + ex.Message
                };
            }
        }

        public async Task<DatabaseInfo> GetDatabaseInfoDetailedAsync()
        {
            var info = await GetDatabaseInfoAsync();

            try
            {
                info.Statistics["ActiveUsers"] = await _context.Users.CountAsync(u => u.IsActive);
                info.Statistics["PendingIssues"] = await _context.Issues.CountAsync(i => i.Status == "Pending");
                info.Statistics["UpcomingEvents"] = await _context.LocalEvents.CountAsync(e => e.EventDate > DateTime.Now);
                info.Statistics["UnreadNotifications"] = await _context.Notifications.CountAsync(n => !n.IsRead);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting detailed database info");
            }

            return info;
        }

        public async Task<bool> SeedDemoDataAsync()
        {
            try
            {
                if (await _context.Users.AnyAsync())
                {
                    _logger.LogInformation("Demo data already exists, skipping seed");
                    return true;
                }

                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@municipal.com",
                    FullName = "System Administrator",
                    PasswordHash = HashPassword("admin123"),
                    Role = "Admin",
                    DateCreated = DateTime.Now,
                    IsActive = true
                };

                var regularUser = new User
                {
                    Username = "user",
                    Email = "user@municipal.com",
                    FullName = "John Citizen",
                    PasswordHash = HashPassword("user123"),
                    Role = "User",
                    DateCreated = DateTime.Now,
                    IsActive = true
                };

                _context.Users.AddRange(adminUser, regularUser);
                await _context.SaveChangesAsync();

                var demoEvents = new List<LocalEvent>
                {
                    new LocalEvent
                    {
                        Title = "Community Clean-up Day",
                        Description = "Join us for a community clean-up event in the local park.",
                        Category = "Community",
                        Location = "Central Park",
                        EventDate = DateTime.Now.AddDays(7),
                        CreatedAt = DateTime.Now,
                        Priority = 1
                    },
                    new LocalEvent
                    {
                        Title = "Town Hall Meeting",
                        Description = "Monthly town hall meeting to discuss local issues.",
                        Category = "Government",
                        Location = "City Hall",
                        EventDate = DateTime.Now.AddDays(14),
                        CreatedAt = DateTime.Now,
                        Priority = 2
                    }
                };

                _context.LocalEvents.AddRange(demoEvents);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Demo data seeded successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding demo data");
                return false;
            }
        }

        public async Task<bool> ClearAllDataAsync()
        {
            try
            {
                _context.PushSubscriptions.RemoveRange(_context.PushSubscriptions);
                _context.Notifications.RemoveRange(_context.Notifications);
                _context.UserSearchPatterns.RemoveRange(_context.UserSearchPatterns);
                _context.Issues.RemoveRange(_context.Issues);
                _context.LocalEvents.RemoveRange(_context.LocalEvents);
                _context.Users.RemoveRange(_context.Users);

                await _context.SaveChangesAsync();

                _logger.LogInformation("All data cleared successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing data");
                return false;
            }
        }

        public async Task<bool> OptimizeDatabaseAsync()
        {
            try
            {
                if (_context.Database.IsSqlite())
                {
                    await _context.Database.ExecuteSqlRawAsync("VACUUM;");
                }

                _logger.LogInformation("Database optimized successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error optimizing database");
                return false;
            }
        }

        public async Task<bool> VacuumDatabaseAsync()
        {
            return await OptimizeDatabaseAsync();
        }

        public async Task CreateBackupAsync()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupPath = Path.Combine(BackupDirectory, $"backup_{timestamp}.db");
            await BackupDatabaseAsync(backupPath);
        }

        public async Task RestoreBackupAsync(string backupPath)
        {
            await RestoreDatabaseAsync(backupPath);
        }

        public async Task<bool> BackupDatabaseAsync(string backupPath)
        {
            try
            {
                if (!Directory.Exists(BackupDirectory))
                {
                    Directory.CreateDirectory(BackupDirectory);
                }

                var connection = _context.Database.GetDbConnection();
                if (connection.DataSource != null && File.Exists(connection.DataSource))
                {
                    await Task.Run(() => File.Copy(connection.DataSource, backupPath, true));
                    _logger.LogInformation("Database backed up to {BackupPath}", backupPath);
                    return true;
                }

                _logger.LogWarning("Database source file not found");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error backing up database");
                return false;
            }
        }

        public async Task<bool> RestoreDatabaseAsync(string backupPath)
        {
            try
            {
                if (!File.Exists(backupPath))
                {
                    _logger.LogWarning("Backup file not found: {BackupPath}", backupPath);
                    return false;
                }

                var connection = _context.Database.GetDbConnection();
                if (connection.DataSource != null)
                {
                    await connection.CloseAsync();
                    await Task.Run(() => File.Copy(backupPath, connection.DataSource, true));
                    _logger.LogInformation("Database restored from {BackupPath}", backupPath);
                    return true;
                }

                _logger.LogWarning("Database source path not available");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring database");
                return false;
            }
        }

        public async Task<List<string>> GetAvailableBackupsAsync()
        {
            try
            {
                if (!Directory.Exists(BackupDirectory))
                {
                    return new List<string>();
                }

                return await Task.Run(() => Directory.GetFiles(BackupDirectory, "*.db")
                    .OrderByDescending(f => f)
                    .ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available backups");
                return new List<string>();
            }
        }

        public async Task<bool> DeleteOldBackupsAsync(int keepCount = 5)
        {
            try
            {
                var backups = await GetAvailableBackupsAsync();
                var backupsToDelete = backups.Skip(keepCount);

                foreach (var backup in backupsToDelete)
                {
                    File.Delete(backup);
                }

                _logger.LogInformation("Deleted {Count} old backups", backupsToDelete.Count());
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting old backups");
                return false;
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
