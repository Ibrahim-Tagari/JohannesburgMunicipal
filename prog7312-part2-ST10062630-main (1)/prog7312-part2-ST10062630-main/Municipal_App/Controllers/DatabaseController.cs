using Microsoft.AspNetCore.Mvc;
using Municipal_App.Services;
using System.Threading.Tasks;

namespace Municipal_App.Controllers
{
    public class DatabaseController : Controller
    {
        private readonly IDatabaseManagementService _databaseService;

        public DatabaseController(IDatabaseManagementService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<IActionResult> Index()
        {
            var info = await _databaseService.GetDatabaseInfoAsync();
            return View(info);
        }

        [HttpPost]
        public async Task<IActionResult> SeedDemoData()
        {
            var result = await _databaseService.SeedDemoDataAsync();

            if (result)
            {
                TempData["SuccessMessage"] = "Demo data seeded successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to seed demo data.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ClearAllData()
        {
            var result = await _databaseService.ClearAllDataAsync();

            if (result)
            {
                TempData["SuccessMessage"] = "All data cleared successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to clear data.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> OptimizeDatabase()
        {
            var result = await _databaseService.OptimizeDatabaseAsync();

            if (result)
            {
                TempData["SuccessMessage"] = "Database optimized successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to optimize database.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBackup()
        {
            await _databaseService.CreateBackupAsync();
            TempData["SuccessMessage"] = "Backup created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetBackups()
        {
            var backups = await _databaseService.GetAvailableBackupsAsync();
            return Json(backups);
        }
    }
}
