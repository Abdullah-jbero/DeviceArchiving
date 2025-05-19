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


    public async Task<GetDeviceDto?> GetDeviceByIdAsync(int id)
    {
        return await context.Devices
            .Where(d => d.Id == id)
            .Include(d=>d.User)
            .Include(d=>d.Operations)
            .Select(d => new GetDeviceDto
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
                UserName = d.User.UserName,
                OperationsDtos = d.Operations.Select(o => new OperationDto
                {
                    OperationName = o.OperationName,
                    OldValue = o.OldValue,
                    NewValue = o.NewValue,
                    Comment = o.Comment,
                    CreatedAt = o.CreatedAt,
                    UserName = d.User.UserName
                }).ToList()
            })
            .FirstOrDefaultAsync();
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

        device.UpdatedAt = DateTime.UtcNow;

        context.Devices.Update(device);
        if (operations.Any())
        {
            context.Operations.AddRange(operations);
        }

        await context.SaveChangesAsync();
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
