using DeviceArchiving.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DeviceArchiving.Data.Contexts;

public class DeviceArchivingContextFactory : IDesignTimeDbContextFactory<DeviceArchivingContext>
{
    public DeviceArchivingContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DeviceArchivingContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=DeviceArchiving;Trusted_Connection=True;");

        return new DeviceArchivingContext(optionsBuilder.Options);
    }
}