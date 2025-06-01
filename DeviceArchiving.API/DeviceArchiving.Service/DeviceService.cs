using DeviceArchiving.Data.Contexts;
using DeviceArchiving.Data.Dto;
using DeviceArchiving.Data.Dto.Devices;
using DeviceArchiving.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceArchiving.Service;

public class DeviceService : IDeviceService
{

    private readonly IDbContextFactory<DeviceArchivingContext> _contextFactory;

    public DeviceService(IDbContextFactory<DeviceArchivingContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<BaseResponse<string>> AddDeviceAsync(CreateDeviceDto dto)
    {

        await using var context = await _contextFactory.CreateDbContextAsync();
        var existingDevice = await context.Devices
            .AnyAsync(d => d.SerialNumber == dto.SerialNumber || d.LaptopName == dto.LaptopName);

        if (existingDevice)
        {
            return BaseResponse<string>.Failure("رقم التسلسل أو اسم اللاب توب موجود بالفعل");
        }

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
            CreatedAt = DateTime.Now
        };

        context.Devices.Add(device);
        await context.SaveChangesAsync();

        return BaseResponse<string>.SuccessResult("Device added successfully.");
    }

    public async Task<BaseResponse<string>> UpdateDeviceAsync(int id, UpdateDeviceDto dto)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var device = await context.Devices.Include(d => d.Operations).FirstOrDefaultAsync(d => d.Id == id);
        if (device == null)
            return BaseResponse<string>.Failure("الجهاز غير موجود");

        // Check for existing devices with the same SerialNumber || LaptopName, excluding the current device
        var existingDevice = await context.Devices.AnyAsync(d => (d.SerialNumber == dto.SerialNumber || d.LaptopName == dto.LaptopName) && d.Id != id);
        if (existingDevice)
            return BaseResponse<string>.Failure("رقم التسلسل أو اسم اللاب توب موجود بالفعل");
    

        var operations = new List<Operation>();
        TrackChange(operations, device, d => d.Source, dto.Source, id, "تحديث الجهة");
        TrackChange(operations, device, d => d.BrotherName, dto.BrotherName, id, "تحديث اسم الأخ");
        TrackChange(operations, device, d => d.LaptopName, dto.LaptopName, id, "تحديث اسم اللاب توب");
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

        device.UpdatedAt = DateTime.Now;
        context.Devices.Update(device);
        if (operations.Any())
            context.Operations.AddRange(operations);

        try
        {
            await context.SaveChangesAsync();
            return BaseResponse<string>.SuccessResult("تم تحديث الجهاز بنجاح.");
        }
        catch (DbUpdateException ex)
        {
            return BaseResponse<string>.Failure("حدث خطأ أثناء تحديث البيانات. يرجى المحاولة مرة أخرى.");
        }
    }

    public async Task<BaseResponse<DuplicateCheckResponse>> CheckDuplicatesAsync(List<CheckDuplicateDto> items)
    {
        try
        {
            var serialNumbers = items.Select(i => i.SerialNumber).Where(sn => !string.IsNullOrEmpty(sn)).ToList();
            var laptopNames = items.Select(i => i.LaptopName).Where(ln => !string.IsNullOrEmpty(ln)).ToList();

            await using var context = await _contextFactory.CreateDbContextAsync();
            var existingSerials = await context.Devices
                .Where(d => serialNumbers.Contains(d.SerialNumber))
                .Select(d => d.SerialNumber)
                .ToListAsync();

            var existingLaptopNames = await context.Devices
                .Where(d => laptopNames.Contains(d.LaptopName))
                .Select(d => d.LaptopName)
                .ToListAsync();

            var response = new DuplicateCheckResponse
            {
                DuplicateSerialNumbers = existingSerials,
                DuplicateLaptopNames = existingLaptopNames
            };

            if (!existingSerials.Any() && !existingLaptopNames.Any())
                return BaseResponse<DuplicateCheckResponse>.SuccessResult(response, "لا توجد أرقام تسلسلية أو أسماء لاب توب مكررة في قاعدة البيانات");

            return BaseResponse<DuplicateCheckResponse>.SuccessResult(response, "تم العثور على تكرارات");
        }
        catch (Exception ex)
        {
            return BaseResponse<DuplicateCheckResponse>.Failure($"خطأ أثناء التحقق من التكرارات: {ex.Message}");
        }
    }

    public async Task<BaseResponse<int>> ProcessDevicesAsync(List<DeviceUploadDto> dtos)
    {
        try
        {
            // Validate required fields
            if (dtos.Any(dto => string.IsNullOrEmpty(dto.SerialNumber) || string.IsNullOrEmpty(dto.LaptopName)))
                return BaseResponse<int>.Failure("رقم التسلسل واسم اللاب توب مطلوبان لجميع الأجهزة");

            // Check for duplicates within input
            var duplicateSerials = dtos.GroupBy(d => d.SerialNumber)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            var duplicateLaptopNames = dtos.GroupBy(d => d.LaptopName)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicateSerials.Any() || duplicateLaptopNames.Any())
                return BaseResponse<int>.Failure($"تم العثور على تكرارات: أرقام تسلسلية ({string.Join(", ", duplicateSerials)}), أسماء لاب توب ({string.Join(", ", duplicateLaptopNames)})");

            int processedCount = 0;
            const int batchSize = 100;

            // Process updates
            await using var context = await _contextFactory.CreateDbContextAsync();
            var updateDevices = dtos.Where(d => d.IsUpdate).ToList();
            if (updateDevices.Any())
            {
                for (int i = 0; i < updateDevices.Count; i += batchSize)
                {
                    var batch = updateDevices.Skip(i).Take(batchSize).ToList();
                    foreach (var dto in batch)
                    {
                        var device = await context.Devices
                            .FirstOrDefaultAsync(d => d.SerialNumber == dto.SerialNumber || d.LaptopName == dto.LaptopName);
                        if (device != null)
                        {
                            var operations = new List<Operation>();
                            TrackChange(operations, device, d => d.Source, dto.Source, device.Id, "تحديث الجهة");
                            TrackChange(operations, device, d => d.BrotherName, dto.BrotherName, device.Id, "تحديث اسم الأخ");
                            TrackChange(operations, device, d => d.LaptopName, dto.LaptopName, device.Id, "تحديث اسم اللاب توب");
                            TrackChange(operations, device, d => d.SystemPassword, dto.SystemPassword, device.Id, "تحديث كلمة سر النظام");
                            TrackChange(operations, device, d => d.WindowsPassword, dto.WindowsPassword, device.Id, "تحديث كلمة سر الويندوز");
                            TrackChange(operations, device, d => d.HardDrivePassword, dto.HardDrivePassword, device.Id, "تحديث كلمة سر الهارد");
                            TrackChange(operations, device, d => d.FreezePassword, dto.FreezePassword, device.Id, "تحديث كلمة التجميد");
                            TrackChange(operations, device, d => d.Code, dto.Code, device.Id, "تحديث الكود");
                            TrackChange(operations, device, d => d.Type, dto.Type, device.Id, "تحديث النوع");
                            TrackChange(operations, device, d => d.SerialNumber, dto.SerialNumber, device.Id, "تحديث رقم السيريال");
                            TrackChange(operations, device, d => d.Card, dto.Card, device.Id, "تحديث الكرت");
                            TrackChange(operations, device, d => d.Comment, dto.Comment, device.Id, "تحديث الملاحظة");
                            TrackChange(operations, device, d => d.ContactNumber, dto.ContactNumber, device.Id, "تحديث رقم التواصل");

                            device.UpdatedAt = DateTime.Now;
                            context.Devices.Update(device);
                            if (operations.Any())
                                context.Operations.AddRange(operations);

                            processedCount++;
                        }
                    }
                    await context.SaveChangesAsync();
                }
            }

            // Process additions
            var createDevices = dtos.Where(d => !d.IsUpdate).ToList();
            if (createDevices.Any())
            {
                var devices = createDevices.Select(dto => new Device
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
                    CreatedAt = DateTime.Now
                }).ToList();

                for (int i = 0; i < devices.Count; i += batchSize)
                {
                    var batch = devices.Skip(i).Take(batchSize).ToList();
                    await context.Devices.AddRangeAsync(batch);
                    await context.SaveChangesAsync();
                    processedCount += batch.Count;
                }
            }

            return BaseResponse<int>.SuccessResult(processedCount, $"تم معالجة {processedCount} جهاز بنجاح");
        }
        catch (Exception ex)
        {
            return BaseResponse<int>.Failure($"خطأ أثناء معالجة الأجهزة: {ex.Message}");
        }
    }

    public async Task<List<GetAllDevicesDto>> GetAllDevicesAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
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
                UserName =d.User.UserName,
                CreatedAt = d.CreatedAt,

            })
            .ToListAsync();
    }

    public async Task<GetDeviceDto?> GetDeviceByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var device = await context.Devices
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (device == null)
            return null;

        var operations = await context.Operations
            .Where(o => o.DeviceId == id)
            .Select(o => new OperationDto
            {
                OperationName = o.OperationName,
                OldValue = o.OldValue,
                NewValue = o.NewValue,
                Comment = o.Comment,
                CreatedAt = o.CreatedAt,
                UserName = o.User.UserName,
            })
            .ToListAsync();

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
        await using var context = await _contextFactory.CreateDbContextAsync();
        var device = await context.Devices
            .Include(d => d.Operations)
            .FirstOrDefaultAsync(d => d.Id == id);
        if (device == null)
            throw new KeyNotFoundException($"الجهاز بالمعرف {id} غير موجود.");

        device.IsActive = false;
        device.UpdatedAt = DateTime.Now;

        device.Operations.Add(new Operation
        {
            DeviceId = id,
            OperationName = "حذف جهاز",
            CreatedAt = DateTime.Now
        });

        context.Devices.Update(device);
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

            // Update the device property directly
            var property = typeof(Device).GetProperties()
             .FirstOrDefault(p => p.GetMethod?.Invoke(device, null)?.ToString() == oldValue);
            if (property != null && property.CanWrite)
                property.SetValue(device, newValue);
        }
    }
}