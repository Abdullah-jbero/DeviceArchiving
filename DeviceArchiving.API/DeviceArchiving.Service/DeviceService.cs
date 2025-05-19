using DeviceArchiving.Data.Contexts;
using DeviceArchiving.Data.Dto.Devices;
using DeviceArchiving.Data.Entities;
using DeviceArchiving.Service;
using Microsoft.EntityFrameworkCore;

namespace DeviceArchiving.Service;
public class DeviceService(DeviceArchivingContext context) : IDeviceService
{
    public async Task AddDeviceAsync(CreateDeviceDto dto)
    {
        var device = new Device
        {
            Source = dto.Source,
            BrotherName = dto.BrotherName,
            LaptopName = dto.LaptopName,
            SystemPassword = dto.SystemPassword,
            WindowsPassword = dto.WindowsPassword,
            HardDrivePassword = dto.HardDrivePassword,
            FreezePassword = dto.FreezePassword,
            Code = dto.Code,
            Type = dto.Type,
            SerialNumber = dto.SerialNumber,
            Card = dto.Card,
            Comment = dto.Comment,
            ContactNumber = dto.ContactNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Devices.Add(device);
        await context.SaveChangesAsync();
    }

    public async Task<List<GetAllDevicesDto>> GetAllDevicesAsync()
    {
        return await context.Devices
            .Where(d => d.IsActive)
            .Include(d => d.User)
            .Select(d => new GetAllDevicesDto
            {
                Id = d.Id,
                Source = d.Source,
                BrotherName = d.BrotherName,
                LaptopName = d.LaptopName,
                SystemPassword = d.SystemPassword,
                WindowsPassword = d.WindowsPassword,
                HardDrivePassword = d.HardDrivePassword,
                FreezePassword = d.FreezePassword,
                Code = d.Code,
                Type = d.Type,
                SerialNumber = d.SerialNumber,
                Comment = d.Comment,
                ContactNumber = d.ContactNumber,
                Card = d.Card,
                UserName = d.User.UserName
            })
            .ToListAsync();
    }


    //public async Task<GetDeviceDto?> GetDeviceByIdAsync(int id)
    //{
    //    return await context.Devices
    //        .Where(d => d.Id == id)
    //        .Include(d=>d.User)
    //        .Include(d=>d.Operations)
    //        .Select(d => new GetDeviceDto
    //        {
    //            Id = d.Id,
    //            Source = d.Source,
    //            BrotherName = d.BrotherName,
    //            LaptopName = d.LaptopName,
    //            SystemPassword = d.SystemPassword,
    //            WindowsPassword = d.WindowsPassword,
    //            HardDrivePassword = d.HardDrivePassword,
    //            FreezePassword = d.FreezePassword,
    //            Code = d.Code,
    //            Type = d.Type,
    //            SerialNumber = d.SerialNumber,
    //            Comment = d.Comment,
    //            ContactNumber = d.ContactNumber,
    //            Card = d.Card,
    //            UserName = d.User.UserName,
    //            OperationsDtos = d.Operations.Select(o => new OperationDto
    //            {
    //                OperationName = o.OperationName,
    //                OldValue = o.OldValue,
    //                NewValue = o.NewValue,
    //                Comment = o.Comment,
    //                CreatedAt = o.CreatedAt,
    //                UserName = d.User.UserName
    //            }).ToList()
    //        })
    //        .FirstOrDefaultAsync();
    //}


    public async Task<GetDeviceDto?> GetDeviceByIdAsync(int id)
    {
        // Fetch the device with its user
        var device = await context.Devices
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (device == null)
        {
            return null;
        }

        // Fetch operations separately
        var operations = await context.Operations
            .Where(o => o.DeviceId == id)
            .Select(o => new OperationDto
            {
                OperationName = o.OperationName,
                OldValue = o.OldValue,
                NewValue = o.NewValue,
                Comment = o.Comment,
                CreatedAt = o.CreatedAt,
                UserName = device.User.UserName // Use the already-fetched UserName
            })
            .ToListAsync();

        // Construct the DTO
        return new GetDeviceDto
        {
            Id = device.Id,
            Source = device.Source,
            BrotherName = device.BrotherName,
            LaptopName = device.LaptopName,
            SystemPassword = device.SystemPassword,
            WindowsPassword = device.WindowsPassword,
            HardDrivePassword = device.HardDrivePassword,
            FreezePassword = device.FreezePassword,
            Code = device.Code,
            Type = device.Type,
            SerialNumber = device.SerialNumber,
            Comment = device.Comment,
            ContactNumber = device.ContactNumber,
            Card = device.Card,
            UserName = device.User.UserName,
            OperationsDtos = operations
        };
    }


    public async Task DeleteDeviceAsync(int id)
    {
        var device = await context.Devices.FindAsync(id);
        if (device is null)
            throw new KeyNotFoundException($"الجهاز بالمعرف {id} غير موجود.");

        device.IsActive = false;
        device.UpdatedAt = DateTime.Now;
        context.Devices.Update(device);
        await context.SaveChangesAsync();
    }

    public async Task UpdateDeviceAsync(int id, UpdateDeviceDto dto)
    {
        var device = await context.Devices.FindAsync(id);
        if (device == null)
            throw new KeyNotFoundException("الجهاز غير موجود");

  
        var operations = new List<Operation>();

        TrackChange(operations, device, d => d.Source, dto.Source, id, "تحديث الجهة");
        TrackChange(operations, device, d => d.BrotherName, dto.BrotherName, id, "تحديث اسم الأخ");
        TrackChange(operations, device, d => d.LaptopName, dto.LaptopName, id, "تحديث اسم اللابتوب");
        TrackChange(operations, device, d => d.SystemPassword, dto.SystemPassword, id, "تحديث كلمة سر النظام");
        TrackChange(operations, device, d => d.WindowsPassword, dto.WindowsPassword, id, "تحديث كلمة سر الويندوز");
        TrackChange(operations, device, d => d.HardDrivePassword, dto.HardDrivePassword, id, "تحديث كلمة سر الهارد");
        TrackChange(operations, device, d => d.FreezePassword, dto.FreezePassword, id, "تحديث كلمة التجميد");
        TrackChange(operations, device, d => d.Code, dto.Code, id, "تحديث الكود");
        TrackChange(operations, device, d => d.Type, dto.Type, id, "تحديث النوع");
        TrackChange(operations, device, d => d.SerialNumber, dto.SerialNumber, id, "تحديث رقم السيريال");
        TrackChange(operations, device, d => d.Card, dto.Card, id, "تحديث الكرت");
        TrackChange(operations, device, d => d.Comment, dto.Comment, id, "تحديث الملاحظة");
        TrackChange(operations, device, d => d.ContactNumber, dto.ContactNumber, id, "تحديث رقم التواصل");
        

        device.UpdatedAt = DateTime.UtcNow;

        context.Devices.Update(device);
        if (operations.Any())
        {
            context.Operations.AddRange(operations);
        }

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Log the error and throw a more user-friendly exception
            throw new Exception("حدث خطأ أثناء تحديث البيانات. يرجى المحاولة مرة أخرى.", ex);
        }
    }
    private void TrackChange(List<Operation> operations, Device device, Func<Device, string?> selector, string? newValue, int deviceId, string operationName)
    {
        var oldValue = selector(device);
        if (oldValue != newValue)
        {
            operations.Add(new Operation
            {
                DeviceId = deviceId,
                OperationName = operationName,
                OldValue = oldValue,
                NewValue = newValue,
                CreatedAt = DateTime.Now
            });

            // Set new value using reflection
            var property = typeof(Device).GetProperties()
                .FirstOrDefault(p => p.GetMethod?.Invoke(device, null)?.ToString() == oldValue);

            if (property != null && property.CanWrite)
                property.SetValue(device, newValue);
        }
    }


}
