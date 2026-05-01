using BCrypt.Net;
using Serilog;

namespace StudentMS.UI.Helpers
{
    /// <summary>
    /// Development helper — generates a BCrypt hash for the default admin password
    /// and logs it so you can paste it into the database seed script.
    ///
    /// Only runs in DEBUG builds. Remove or disable once the DB is seeded.
    /// </summary>
    public static class DevSeedHelper
    {
#if DEBUG
        public static void LogAdminHash()
        {
            const string defaultPassword = "Admin@123";
            string hash = BCrypt.Net.BCrypt.HashPassword(defaultPassword, workFactor: 12);

            Log.Warning("=== DEV SEED HELPER ===");
            Log.Warning("Run this SQL to seed the admin user:");
            Log.Warning("DELETE FROM Users WHERE Username = 'admin';");
            Log.Warning("INSERT INTO Users (Username, PasswordHash, Role, IsActive)");
            Log.Warning("VALUES ('admin', '{Hash}', 'Admin', 1);", hash);
            Log.Warning("=== END DEV SEED HELPER ===");

            // Also write to Debug output so it's visible in VS Output window
            System.Diagnostics.Debug.WriteLine($"\n>>> ADMIN HASH for 'Admin@123': {hash}\n");
        }
#endif
    }
}
