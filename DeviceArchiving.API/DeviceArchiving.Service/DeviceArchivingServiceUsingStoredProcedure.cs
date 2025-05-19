using DeviceArchiving.Data.Contexts;
using DeviceArchiving.Data.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceArchiving.Service;

//public class DeviceArchivingServiceUsingStoredProcedure : IDeviceService
//{
//    private readonly DeviceArchivingContext _context;

//    public DeviceArchivingServiceUsingStoredProcedure(DeviceArchivingContext context)
//    {
//        _context = context;
//    }

//    public async Task AddDevice(Device device)
//    {
//        await _context.Database.ExecuteSqlRawAsync(
//              "EXEC sp_AddDevice @Source, @BrotherName, @LaptopName, @SystemPassword, @WindowsPassword, @HardDrivePassword, @FreezePassword, @Code, @Type, @SerialNumber, @Card, @IsActive",
//              new SqlParameter("@Source", device.Source),
//              new SqlParameter("@BrotherName", device.BrotherName ?? (object)DBNull.Value),
//              new SqlParameter("@LaptopName", device.LaptopName ?? (object)DBNull.Value),
//              new SqlParameter("@SystemPassword", device.SystemPassword ?? (object)DBNull.Value),
//              new SqlParameter("@WindowsPassword", device.WindowsPassword ?? (object)DBNull.Value),
//              new SqlParameter("@HardDrivePassword", device.HardDrivePassword ?? (object)DBNull.Value),
//              new SqlParameter("@FreezePassword", device.FreezePassword ?? (object)DBNull.Value),
//              new SqlParameter("@Code", device.Code ?? (object)DBNull.Value),
//              new SqlParameter("@Type", device.Type ?? (object)DBNull.Value),
//              new SqlParameter("@SerialNumber", device.SerialNumber ?? (object)DBNull.Value),
//              new SqlParameter("@Card", device.Card ?? (object)DBNull.Value),
//              new SqlParameter("@IsActive", device.IsActive)
//          );
//    }

//    public async Task<List<Device>> GetAllDevices()
//    {
//        return await _context.Devices
//            .FromSqlRaw("EXEC sp_GetAllDevices")
//            .ToListAsync();
//    }

//    public async Task<Device?> GetDeviceById(int id)
//    {

//        var device = await _context.Devices
//            .FromSqlRaw("EXEC sp_GetDeviceById @Id", new SqlParameter("@Id", id))
//            .FirstOrDefaultAsync();

//        return device;

//    }

//    public async Task DeleteDevice(int id)
//    {
//        await _context.Database.ExecuteSqlRawAsync(
//             "EXEC sp_DeleteDevice @Id",
//             new SqlParameter("@Id", id)
//         );
//    }

//    public async Task UpdateDevice(Device device)
//    {
//        try
//        {
//            await _context.Database.ExecuteSqlRawAsync(
//                 "EXEC sp_UpdateDevice @Id, @Source, @BrotherName, @LaptopName, @SystemPassword, @WindowsPassword, @HardDrivePassword, @FreezePassword, @Code, @Type, @SerialNumber, @Card",
//                 new SqlParameter("@Id", device.Id),
//                 new SqlParameter("@Source", device.Source),
//                 new SqlParameter("@BrotherName", device.BrotherName ?? (object)DBNull.Value),
//                 new SqlParameter("@LaptopName", device.LaptopName ?? (object)DBNull.Value),
//                 new SqlParameter("@SystemPassword", device.SystemPassword ?? (object)DBNull.Value),
//                 new SqlParameter("@WindowsPassword", device.WindowsPassword ?? (object)DBNull.Value),
//                 new SqlParameter("@HardDrivePassword", device.HardDrivePassword ?? (object)DBNull.Value),
//                 new SqlParameter("@FreezePassword", device.FreezePassword ?? (object)DBNull.Value),
//                 new SqlParameter("@Code", device.Code ?? (object)DBNull.Value),
//                 new SqlParameter("@Type", device.Type ?? (object)DBNull.Value),
//                 new SqlParameter("@SerialNumber", device.SerialNumber ?? (object)DBNull.Value),
//                 new SqlParameter("@Card", device.Card ?? (object)DBNull.Value)
//             );
//        }
//        catch (SqlException ex) when (ex.Number == 50000)
//        {
//            throw new Exception("الجهاز غير موجود", ex);
//        }
//    }



//    public async Task<Device> GetDevice(int id)
//    {

//        var device = await _context.Devices
//            .FirstOrDefaultAsync(d => d.Id == id);

//        if (device == null)
//        {
//            throw new KeyNotFoundException($"Device with ID {id} not found.");
//        }

//        return device;

//    }
//}