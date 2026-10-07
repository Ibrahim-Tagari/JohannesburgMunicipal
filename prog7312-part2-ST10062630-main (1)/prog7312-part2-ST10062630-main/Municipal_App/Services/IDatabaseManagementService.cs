using Municipal_App.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Municipal_App.Services
{
    public interface IDatabaseManagementService
    {
        Task<bool> InitializeDatabaseAsync();
        Task<bool> SeedDemoDataAsync();
        Task<bool> ClearAllDataAsync();
        Task CreateBackupAsync();
        Task RestoreBackupAsync(string backupPath);
        Task<bool> BackupDatabaseAsync(string backupPath);
        Task<bool> RestoreDatabaseAsync(string backupPath);
        Task<DatabaseInfo> GetDatabaseInfoAsync();
        Task<DatabaseInfo> GetDatabaseInfoDetailedAsync();
        Task<bool> OptimizeDatabaseAsync();
        Task<bool> VacuumDatabaseAsync();
        Task<List<string>> GetAvailableBackupsAsync();
        Task<bool> DeleteOldBackupsAsync(int keepCount = 5);
    }
}
