using DeviceArchiving.Data.Contexts;
using DeviceArchiving.Data.Entities;
using DeviceArchiving.Service;
using Microsoft.EntityFrameworkCore;

namespace DeviceArchiving.Service;
public class DeviceService(DeviceArchivingContext context) : IDeviceService
{
    public void AddDevice(Device device)
    {
        context.Devices.Add(device);
        context.SaveChanges();
    }


    public IEnumerable<Device> GetAllDevices()
    {
        return context.Devices.ToList();
    }

    public Device? GetDeviceById(int id)
    {
        var device = context.Devices.FirstOrDefault(d => d.Id == id);
        return device;
    }

    public void DeleteDevice(int id)
    {
        var device = GetDeviceById(id);
        if (device != null)
        {
            device.IsActive = false;
            context.Devices.Update(device);
            context.SaveChanges();
        }

    }

    public void UpdateDevice(Device device)
    {
        var existingDevice = context.Devices.Find(device.Id);
        if (existingDevice == null) throw new Exception("الجهاز غير موجود");

        var operations = new List<Operation>();

        if (existingDevice.Source != device.Source)
        {
            operations.Add(new Operation
            {
                DeviceId = device.Id,
                OperationName = "تحديث الجهة",
                oldValue = existingDevice.Source,
                newValue = device.Source,
                CreatedAt = DateTime.UtcNow
            });
            existingDevice.Source = device.Source;
        }

        if (existingDevice.BrotherName != device.BrotherName)
        {
            operations.Add(new Operation
            {
                DeviceId = device.Id,
                OperationName = "تحديث اسم الأخ",
                oldValue = existingDevice.BrotherName,
                newValue = device.BrotherName,
                CreatedAt = DateTime.UtcNow
            });
            existingDevice.BrotherName = device.BrotherName;
        }

        if (existingDevice.LaptopName != device.LaptopName)
        {
            operations.Add(new Operation
            {
                DeviceId = device.Id,
                OperationName = "تحديث اسم اللابتوب",
                oldValue = existingDevice.LaptopName,
                newValue = device.LaptopName,
                CreatedAt = DateTime.UtcNow
            });
            existingDevice.LaptopName = device.LaptopName;
        }

        if (existingDevice.SystemPassword != device.SystemPassword)
        {
            operations.Add(new Operation
            {
                DeviceId = device.Id,
                OperationName = "تحديث كلمة سر النظام",
                oldValue = existingDevice.SystemPassword,
                newValue = device.SystemPassword,
                CreatedAt = DateTime.UtcNow
            });
            existingDevice.SystemPassword = device.SystemPassword;
        }

        if (existingDevice.WindowsPassword != device.WindowsPassword)
        {
            operations.Add(new Operation
            {
                DeviceId = device.Id,
                OperationName = "تحديث كلمة سر الويندوز",
                oldValue = existingDevice.WindowsPassword,
                newValue = device.WindowsPassword,
                CreatedAt = DateTime.UtcNow
            });
            existingDevice.WindowsPassword = device.WindowsPassword;
        }

        if (existingDevice.HardDrivePassword != device.HardDrivePassword)
        {
            operations.Add(new Operation
            {
                DeviceId = device.Id,
                OperationName = "تحديث كلمة سر الهارد",
                oldValue = existingDevice.HardDrivePassword,
                newValue = device.HardDrivePassword,
                CreatedAt = DateTime.UtcNow
            });
            existingDevice.HardDrivePassword = device.HardDrivePassword;
        }

        if (existingDevice.FreezePassword != device.FreezePassword)
        {
            operations.Add(new Operation
            {
                DeviceId = device.Id,
                OperationName = "تحديث كلمة التجميد",
                oldValue = existingDevice.FreezePassword,
                newValue = device.FreezePassword,
                CreatedAt = DateTime.UtcNow
            });
            existingDevice.FreezePassword = device.FreezePassword;
        }

        if (existingDevice.Code != device.Code)
        {
            operations.Add(new Operation
            {
                DeviceId = device.Id,
                OperationName = "تحديث الكود",
                oldValue = existingDevice.Code,
                newValue = device.Code,
                CreatedAt = DateTime.UtcNow
            });
            existingDevice.Code = device.Code;
        }

        if (existingDevice.Type != device.Type)
        {
            operations.Add(new Operation
            {
                DeviceId = device.Id,
                OperationName = "تحديث النوع",
                oldValue = existingDevice.Type,
                newValue = device.Type,
                CreatedAt = DateTime.UtcNow
            });
            existingDevice.Type = device.Type;
        }

        if (existingDevice.SerialNumber != device.SerialNumber)
        {
            operations.Add(new Operation
            {
                DeviceId = device.Id,
                OperationName = "تحديث رقم السيريال",
                oldValue = existingDevice.SerialNumber,
                newValue = device.SerialNumber,
                CreatedAt = DateTime.UtcNow
            });
            existingDevice.SerialNumber = device.SerialNumber;
        }

        if (existingDevice.Card != device.Card)
        {
            operations.Add(new Operation
            {
                DeviceId = device.Id,
                OperationName = "تحديث الكرت",
                oldValue = existingDevice.Card,
                newValue = device.Card,
                CreatedAt = DateTime.UtcNow
            });
            existingDevice.Card = device.Card;
        }

        // save the changes 
        context.Devices.Update(existingDevice);
        context.Operations.AddRange(operations);
        context.SaveChanges();
    }
}