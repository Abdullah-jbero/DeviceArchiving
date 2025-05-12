using DeviceArchiving.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Windows.Forms;

namespace DeviceArchiving
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ≈⁄œ«œ DbContext
            var options = new DbContextOptionsBuilder<DeviceArchivingContext>()
                .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=DeviceArchiving;Trusted_Connection=True;")
                .Options;

            using var context = new DeviceArchivingContext(options);
            context.Database.EnsureCreated();

            // ≈‰‘«¡ Œœ„… IDeviceService
            var deviceService = new DeviceArchivingService(context);

            // Õﬁ‰ «·Œœ„… ›Ì «·‰„Ê–Ã Main
            Application.Run(new Main(deviceService));
        }
    }
}