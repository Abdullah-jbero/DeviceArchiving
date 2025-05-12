using DeviceArchiving.Data.Contexts;
using DeviceArchiving.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeviceArchiving.Service;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class DeviceArchivingServiceUsingStoredProcedure : IDeviceService
{
    private readonly DeviceArchivingContext _context;

    public DeviceArchivingServiceUsingStoredProcedure(DeviceArchivingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void AddDevice(Device device)
    {
        if (device == null) throw new ArgumentNullException(nameof(device));

        // Call the stored procedure for adding a device
        _context.Database.ExecuteSqlRaw(
            "EXEC sp_AddDevice @Source, @BrotherName, @LaptopName, @SystemPassword, @WindowsPassword, @HardDrivePassword, @FreezePassword, @Code, @Type, @SerialNumber, @Card, @IsActive",
            new SqlParameter("@Source", device.Source ?? (object)DBNull.Value),
            new SqlParameter("@BrotherName", device.BrotherName ?? (object)DBNull.Value),
            new SqlParameter("@LaptopName", device.LaptopName ?? (object)DBNull.Value),
            new SqlParameter("@SystemPassword", device.SystemPassword ?? (object)DBNull.Value),
            new SqlParameter("@WindowsPassword", device.WindowsPassword ?? (object)DBNull.Value),
            new SqlParameter("@HardDrivePassword", device.HardDrivePassword ?? (object)DBNull.Value),
            new SqlParameter("@FreezePassword", device.FreezePassword ?? (object)DBNull.Value),
            new SqlParameter("@Code", device.Code ?? (object)DBNull.Value),
            new SqlParameter("@Type", device.Type ?? (object)DBNull.Value),
            new SqlParameter("@SerialNumber", device.SerialNumber ?? (object)DBNull.Value),
            new SqlParameter("@Card", device.Card ?? (object)DBNull.Value),
            new SqlParameter("@IsActive", device.IsActive)
        );
    }

    public IEnumerable<Device> GetAllDevices()
    {
        // Call the stored procedure for retrieving all devices
        var devices = _context.Devices
            .FromSqlRaw("EXEC sp_GetAllDevices")
            .ToList();

        return devices;
    }

    public void UpdateDevice(Device device)
    {
        if (device == null) throw new ArgumentNullException(nameof(device));

        // Call the stored procedure for updating a device
        _context.Database.ExecuteSqlRaw(
            "EXEC sp_UpdateDevice @Id, @Source, @BrotherName, @LaptopName, @SystemPassword, @WindowsPassword, @HardDrivePassword, @FreezePassword, @Code, @Type, @SerialNumber, @Card, @IsActive, @UpdatedAt",
            new SqlParameter("@Id", device.Id),
            new SqlParameter("@Source", device.Source ?? (object)DBNull.Value),
            new SqlParameter("@BrotherName", device.BrotherName ?? (object)DBNull.Value),
            new SqlParameter("@LaptopName", device.LaptopName ?? (object)DBNull.Value),
            new SqlParameter("@SystemPassword", device.SystemPassword ?? (object)DBNull.Value),
            new SqlParameter("@WindowsPassword", device.WindowsPassword ?? (object)DBNull.Value),
            new SqlParameter("@HardDrivePassword", device.HardDrivePassword ?? (object)DBNull.Value),
            new SqlParameter("@FreezePassword", device.FreezePassword ?? (object)DBNull.Value),
            new SqlParameter("@Code", device.Code ?? (object)DBNull.Value),
            new SqlParameter("@Type", device.Type ?? (object)DBNull.Value),
            new SqlParameter("@SerialNumber", device.SerialNumber ?? (object)DBNull.Value),
            new SqlParameter("@Card", device.Card ?? (object)DBNull.Value),
            new SqlParameter("@IsActive", device.IsActive),
            new SqlParameter("@UpdatedAt", DateTime.UtcNow)
        );
    }

    public void DeleteDevice(int id)
    {
        // Call the stored procedure for deleting a device
        _context.Database.ExecuteSqlRaw(
            "EXEC sp_DeleteDevice @Id",
            new SqlParameter("@Id", id)
        );
    }
}


