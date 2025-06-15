using DeviceArchiving.Data.Dto;
using DeviceArchiving.Data.Dto.Devices;
using DeviceArchiving.Data.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DeviceArchiving.Service.DeviceServices;

public class DeviceProcedureService : IDeviceService
{
    private readonly DataAccessLayer _dataAccessLayer;

    public DeviceProcedureService(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(nameof(configuration), "سلسلة الاتصال 'DefaultConnection' غير موجودة.");
        string pathDb = configuration["PathDb"]
         ?? throw new ArgumentNullException(nameof(configuration), "المفتاح 'PathDb' غير موجود في الإعدادات.");

        _dataAccessLayer = new DataAccessLayer(connectionString, pathDb);
    }



    public async Task<BaseResponse<string>> UpdateDeviceAsync(int id, UpdateDeviceDto dto)
    {
        try
        {

            var userId = AppSession.CurrentUserId;

            // Retrieve the current device to track changes
            var device = await GetDeviceByIdAsync(id);
            if (device == null)
                return BaseResponse<string>.Failure("الجهاز غير موجود");

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

            // Update the device
            var updateParameters = new[]
            {
                new SqlParameter("@Id", id),
                new SqlParameter("@Source", (object)dto.Source ?? DBNull.Value),
                new SqlParameter("@BrotherName", (object)dto.BrotherName ?? DBNull.Value),
                new SqlParameter("@LaptopName", (object)dto.LaptopName ?? DBNull.Value),
                new SqlParameter("@SystemPassword", (object)dto.SystemPassword ?? DBNull.Value),
                new SqlParameter("@WindowsPassword", (object)dto.WindowsPassword ?? DBNull.Value),
                new SqlParameter("@HardDrivePassword", (object)dto.HardDrivePassword ?? DBNull.Value),
                new SqlParameter("@FreezePassword", (object)dto.FreezePassword ?? DBNull.Value),
                new SqlParameter("@Code", (object)dto.Code ?? DBNull.Value),
                new SqlParameter("@Type", (object)dto.Type ?? DBNull.Value),
                new SqlParameter("@SerialNumber", (object)dto.SerialNumber ?? DBNull.Value),
                new SqlParameter("@Card", (object)dto.Card ?? DBNull.Value),
                new SqlParameter("@Comment", (object)dto.Comment ?? DBNull.Value),
                new SqlParameter("@ContactNumber", (object)dto.ContactNumber ?? DBNull.Value),
                new SqlParameter("@UpdatedAt", DateTime.Now),
                new SqlParameter("@UserId",userId )
            };
            await _dataAccessLayer.ExecuteNonQueryAsync("sp_UpdateDevice", updateParameters);

            // Add operations if any
            foreach (var operation in operations)
            {
                var operationParameters = new[]
                {
                    new SqlParameter("@DeviceId", operation.DeviceId),
                    new SqlParameter("@OperationName", operation.OperationName),
                    new SqlParameter("@OldValue", (object)operation.OldValue ?? DBNull.Value),
                    new SqlParameter("@NewValue", (object)operation.NewValue ?? DBNull.Value),
                    new SqlParameter("@Comment", (object)operation.Comment ?? DBNull.Value),
                    new SqlParameter("@CreatedAt", operation.CreatedAt),
                    new SqlParameter("@UserId",userId )

                };
                await _dataAccessLayer.ExecuteNonQueryAsync("sp_AddOperation", operationParameters);
            }

            return BaseResponse<string>.SuccessResult("تم تحديث الجهاز بنجاح.");
        }
        catch (SqlException ex) when (ex.Message.Contains("رقم التسلسل أو اسم اللاب توب موجود بالفعل"))
        {
            return BaseResponse<string>.Failure("رقم التسلسل أو اسم اللاب توب موجود بالفعل");
        }
        catch (SqlException ex) when (ex.Message.Contains("الجهاز غير موجود"))
        {
            return BaseResponse<string>.Failure("الجهاز غير موجود");
        }
    }

    public async Task<BaseResponse<DuplicateCheckResponse>> CheckDuplicatesAsync(List<CheckDuplicateDto> items)
    {
        var serialNumbers = items.Select(i => i.SerialNumber).Where(sn => !string.IsNullOrEmpty(sn)).ToList();
        var laptopNames = items.Select(i => i.LaptopName).Where(ln => !string.IsNullOrEmpty(ln)).ToList();

        var parameters = new[]
        {
            new SqlParameter("@SerialNumbers", string.Join(",", serialNumbers)),
            new SqlParameter("@LaptopNames", string.Join(",", laptopNames))
        };

        var duplicates = await _dataAccessLayer.ExecuteQueryAsync("sp_CheckDuplicates", parameters, reader => new
        {
            Value = reader.GetString(0)
        });

        var response = new DuplicateCheckResponse
        {
            DuplicateSerialNumbers = duplicates.Where(d => serialNumbers.Contains(d.Value)).Select(d => d.Value).ToList(),
            DuplicateLaptopNames = duplicates.Where(d => laptopNames.Contains(d.Value)).Select(d => d.Value).ToList()
        };

        if (!response.DuplicateSerialNumbers.Any() && !response.DuplicateLaptopNames.Any())
            return BaseResponse<DuplicateCheckResponse>.SuccessResult(response, "لا توجد أرقام تسلسلية أو أسماء لاب توب مكررة في قاعدة البيانات");

        return BaseResponse<DuplicateCheckResponse>.SuccessResult(response, "تم العثور على تكرارات");
    }


    private async Task<GetDeviceDto?> GetDeviceBySerialOrLaptopAsync(string? serialNumber, string? laptopName)
    {
        var parameters = new[]
        {
            new SqlParameter("@SerialNumber", (object)serialNumber ?? DBNull.Value),
            new SqlParameter("@LaptopName", (object)laptopName ?? DBNull.Value)
        };

        var devices = await _dataAccessLayer.ExecuteQueryAsync("sp_GetDeviceBySerialOrLaptop", parameters, reader => new GetDeviceDto
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Source = reader.IsDBNull(reader.GetOrdinal("Source")) ? null : reader.GetString(reader.GetOrdinal("Source")),
            BrotherName = reader.IsDBNull(reader.GetOrdinal("BrotherName")) ? null : reader.GetString(reader.GetOrdinal("BrotherName")),
            LaptopName = reader.IsDBNull(reader.GetOrdinal("LaptopName")) ? null : reader.GetString(reader.GetOrdinal("LaptopName")),
            SystemPassword = reader.IsDBNull(reader.GetOrdinal("SystemPassword")) ? null : reader.GetString(reader.GetOrdinal("SystemPassword")),
            WindowsPassword = reader.IsDBNull(reader.GetOrdinal("WindowsPassword")) ? null : reader.GetString(reader.GetOrdinal("WindowsPassword")),
            HardDrivePassword = reader.IsDBNull(reader.GetOrdinal("HardDrivePassword")) ? null : reader.GetString(reader.GetOrdinal("HardDrivePassword")),
            FreezePassword = reader.IsDBNull(reader.GetOrdinal("FreezePassword")) ? null : reader.GetString(reader.GetOrdinal("FreezePassword")),
            Code = reader.IsDBNull(reader.GetOrdinal("Code")) ? null : reader.GetString(reader.GetOrdinal("Code")),
            Type = reader.IsDBNull(reader.GetOrdinal("Type")) ? null : reader.GetString(reader.GetOrdinal("Type")),
            SerialNumber = reader.IsDBNull(reader.GetOrdinal("SerialNumber")) ? null : reader.GetString(reader.GetOrdinal("SerialNumber")),
            Card = reader.IsDBNull(reader.GetOrdinal("Card")) ? null : reader.GetString(reader.GetOrdinal("Card")),
            Comment = reader.IsDBNull(reader.GetOrdinal("Comment")) ? null : reader.GetString(reader.GetOrdinal("Comment")),
            ContactNumber = reader.IsDBNull(reader.GetOrdinal("ContactNumber")) ? null : reader.GetString(reader.GetOrdinal("ContactNumber")),
            UserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? null : reader.GetString(reader.GetOrdinal("UserName"))
        });

        return devices.FirstOrDefault();
    }

    public async Task<BaseResponse<string>> AddDeviceAsync(CreateDeviceDto dto)
    {
        try
        {
            var userId = AppSession.CurrentUserId;
            if (userId == 0)
                return BaseResponse<string>.Failure("معرف المستخدم غير صالح");

            var parameters = new[]
            {
                new SqlParameter("@Source", (object)dto.Source ?? DBNull.Value),
                new SqlParameter("@BrotherName", (object)dto.BrotherName ?? DBNull.Value),
                new SqlParameter("@LaptopName", (object)dto.LaptopName ?? DBNull.Value),
                new SqlParameter("@SystemPassword", (object)dto.SystemPassword ?? DBNull.Value),
                new SqlParameter("@WindowsPassword", (object)dto.WindowsPassword ?? DBNull.Value),
                new SqlParameter("@HardDrivePassword", (object)dto.HardDrivePassword ?? DBNull.Value),
                new SqlParameter("@FreezePassword", (object)dto.FreezePassword ?? DBNull.Value),
                new SqlParameter("@Code", (object)dto.Code ?? DBNull.Value),
                new SqlParameter("@Type", (object)dto.Type ?? DBNull.Value),
                new SqlParameter("@SerialNumber", (object)dto.SerialNumber ?? DBNull.Value),
                new SqlParameter("@Card", (object)dto.Card ?? DBNull.Value),
                new SqlParameter("@Comment", (object)dto.Comment ?? DBNull.Value),
                new SqlParameter("@ContactNumber", (object)dto.ContactNumber ?? DBNull.Value),
                new SqlParameter("@IsActive", true),
                new SqlParameter("@CreatedAt", DateTime.Now),
                new SqlParameter("@UserId", userId),
            };

            // Define the mapping function to extract the message
            Func<SqlDataReader, string> mapFunction = reader => reader.GetString(0);

            // Execute the stored procedure and get the message
            var results = await _dataAccessLayer.ExecuteQueryAsync("sp_AddDevice", parameters, mapFunction);

            // Check if any result was returned
            if (results.Any())
            {
                return BaseResponse<string>.SuccessResult(results.First());
            }

            return BaseResponse<string>.Failure("لم يتم إرجاع أي رسالة من الإجراء المخزن.");
        }
        catch (SqlException ex) when (ex.Message.Contains("الجهاز موجود بالفعل في النظام"))
        {
            return BaseResponse<string>.Failure("رقم التسلسل أو اسم اللاب توب موجود بالفعل");
        }
        catch (Exception ex)
        {
            return BaseResponse<string>.Failure($"حدث خطأ: {ex.Message}");
        }
    }
    public async Task<BaseResponse<int>> ProcessDevicesAsync(List<DeviceUploadDto> dtos)
    {
        if (dtos.Any(dto => string.IsNullOrEmpty(dto.SerialNumber) || string.IsNullOrEmpty(dto.LaptopName)))
            return BaseResponse<int>.Failure("رقم التسلسل واسم اللاب توب مطلوبان لجميع الأجهزة");

        var userId = AppSession.CurrentUserId;
        if (userId == 0)
            return BaseResponse<int>.Failure("معرف المستخدم غير صالح");

        int processedCount = 0;
        const int batchSize = 100;

        var updateDevices = dtos.Where(d => d.IsUpdate).ToList();
        if (updateDevices.Any())
        {
            for (int i = 0; i < updateDevices.Count; i += batchSize)
            {
                var batch = updateDevices.Skip(i).Take(batchSize).ToList();
                foreach (var dto in batch)
                {
                    var device = await GetDeviceBySerialOrLaptopAsync(dto.SerialNumber, dto.LaptopName);
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

                        var updateParameters = new[]
                        {
                            new SqlParameter("@Id", device.Id),
                            new SqlParameter("@Source", (object)dto.Source ?? DBNull.Value),
                            new SqlParameter("@BrotherName", (object)dto.BrotherName ?? DBNull.Value),
                            new SqlParameter("@LaptopName", (object)dto.LaptopName ?? DBNull.Value),
                            new SqlParameter("@SystemPassword", (object)dto.SystemPassword ?? DBNull.Value),
                            new SqlParameter("@WindowsPassword", (object)dto.WindowsPassword ?? DBNull.Value),
                            new SqlParameter("@HardDrivePassword", (object)dto.HardDrivePassword ?? DBNull.Value),
                            new SqlParameter("@FreezePassword", (object)dto.FreezePassword ?? DBNull.Value),
                            new SqlParameter("@Code", (object)dto.Code ?? DBNull.Value),
                            new SqlParameter("@Type", (object)dto.Type ?? DBNull.Value),
                            new SqlParameter("@SerialNumber", (object)dto.SerialNumber ?? DBNull.Value),
                            new SqlParameter("@Card", (object)dto.Card ?? DBNull.Value),
                            new SqlParameter("@Comment", (object)dto.Comment ?? DBNull.Value),
                            new SqlParameter("@ContactNumber", (object)dto.ContactNumber ?? DBNull.Value),
                            new SqlParameter("@UpdatedAt", DateTime.Now),
                            new SqlParameter("@UserId", userId)
                        };

                        await _dataAccessLayer.ExecuteNonQueryAsync("sp_UpdateDevice", updateParameters);

                        foreach (var operation in operations)
                        {
                            var operationParameters = new[]
                            {
                                new SqlParameter("@DeviceId", operation.DeviceId),
                                new SqlParameter("@OperationName", operation.OperationName),
                                new SqlParameter("@OldValue", (object)operation.OldValue ?? DBNull.Value),
                                new SqlParameter("@NewValue", (object)operation.NewValue ?? DBNull.Value),
                                new SqlParameter("@Comment", (object)operation.Comment ?? DBNull.Value),
                                new SqlParameter("@CreatedAt", operation.CreatedAt),
                                new SqlParameter("@UserId", userId)
                            };
                            await _dataAccessLayer.ExecuteNonQueryAsync("sp_AddOperation", operationParameters);
                        }

                        processedCount++;
                    }
                }
            }
        }

        var createDevices = dtos.Where(d => !d.IsUpdate).ToList();
        if (createDevices.Any())
        {
            for (int i = 0; i < createDevices.Count; i += batchSize)
            {
                var batch = createDevices.Skip(i).Take(batchSize).ToList();
                foreach (var dto in batch)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@Source", (object)dto.Source ?? DBNull.Value),
                        new SqlParameter("@BrotherName", (object)dto.BrotherName ?? DBNull.Value),
                        new SqlParameter("@LaptopName", (object)dto.LaptopName ?? DBNull.Value),
                        new SqlParameter("@SystemPassword", (object)dto.SystemPassword ?? DBNull.Value),
                        new SqlParameter("@WindowsPassword", (object)dto.WindowsPassword ?? DBNull.Value),
                        new SqlParameter("@HardDrivePassword", (object)dto.HardDrivePassword ?? DBNull.Value),
                        new SqlParameter("@FreezePassword", (object)dto.FreezePassword ?? DBNull.Value),
                        new SqlParameter("@Code", (object)dto.Code ?? DBNull.Value),
                        new SqlParameter("@Type", (object)dto.Type ?? DBNull.Value),
                        new SqlParameter("@SerialNumber", (object)dto.SerialNumber ?? DBNull.Value),
                        new SqlParameter("@Card", (object)dto.Card ?? DBNull.Value),
                        new SqlParameter("@Comment", (object)dto.Comment ?? DBNull.Value),
                        new SqlParameter("@ContactNumber", (object)dto.ContactNumber ?? DBNull.Value),
                        new SqlParameter("@IsActive", true),
                        new SqlParameter("@CreatedAt",(object)dto.CreatedAt ?? DateTime.Now),
                        new SqlParameter("@UserId", userId),
                    };

                    await _dataAccessLayer.ExecuteNonQueryAsync(storedProcedureName: "sp_AddDevice", parameters);
                    processedCount++;
                }
            }
        }

        return BaseResponse<int>.SuccessResult(processedCount, $"تم معالجة {processedCount} جهاز بنجاح");
    }

    public async Task<List<GetAllDevicesDto>> GetAllDevicesAsync()
    {
        return await _dataAccessLayer.ExecuteQueryAsync("sp_GetAllDevices", null, reader => new GetAllDevicesDto
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Source = reader.IsDBNull(reader.GetOrdinal("Source")) ? null : reader.GetString(reader.GetOrdinal("Source")),
            BrotherName = reader.IsDBNull(reader.GetOrdinal("BrotherName")) ? null : reader.GetString(reader.GetOrdinal("BrotherName")),
            LaptopName = reader.IsDBNull(reader.GetOrdinal("LaptopName")) ? null : reader.GetString(reader.GetOrdinal("LaptopName")),
            SystemPassword = reader.IsDBNull(reader.GetOrdinal("SystemPassword")) ? null : reader.GetString(reader.GetOrdinal("SystemPassword")),
            WindowsPassword = reader.IsDBNull(reader.GetOrdinal("WindowsPassword")) ? null : reader.GetString(reader.GetOrdinal("WindowsPassword")),
            HardDrivePassword = reader.IsDBNull(reader.GetOrdinal("HardDrivePassword")) ? null : reader.GetString(reader.GetOrdinal("HardDrivePassword")),
            FreezePassword = reader.IsDBNull(reader.GetOrdinal("FreezePassword")) ? null : reader.GetString(reader.GetOrdinal("FreezePassword")),
            Code = reader.IsDBNull(reader.GetOrdinal("Code")) ? null : reader.GetString(reader.GetOrdinal("Code")),
            Type = reader.IsDBNull(reader.GetOrdinal("Type")) ? null : reader.GetString(reader.GetOrdinal("Type")),
            SerialNumber = reader.IsDBNull(reader.GetOrdinal("SerialNumber")) ? null : reader.GetString(reader.GetOrdinal("SerialNumber")),
            Card = reader.IsDBNull(reader.GetOrdinal("Card")) ? null : reader.GetString(reader.GetOrdinal("Card")),
            Comment = reader.IsDBNull(reader.GetOrdinal("Comment")) ? null : reader.GetString(reader.GetOrdinal("Comment")),
            ContactNumber = reader.IsDBNull(reader.GetOrdinal("ContactNumber")) ? null : reader.GetString(reader.GetOrdinal("ContactNumber")),
            UserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? null : reader.GetString(reader.GetOrdinal("UserName")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
        });
    }

    public async Task<GetDeviceDto?> GetDeviceByIdAsync(int id)
    {
        try
        {
            // Create a new SqlParameter instance for the first call
            var deviceParams = new[] { new SqlParameter("@Id", id) };

            // Fetch device info
            var devices = await _dataAccessLayer.ExecuteQueryAsync(
                "sp_GetDeviceById_Device",
                deviceParams,
                reader => new
                {
                    Device = new GetDeviceDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Source = reader.IsDBNull(reader.GetOrdinal("Source")) ? null : reader.GetString(reader.GetOrdinal("Source")),
                        BrotherName = reader.IsDBNull(reader.GetOrdinal("BrotherName")) ? null : reader.GetString(reader.GetOrdinal("BrotherName")),
                        LaptopName = reader.IsDBNull(reader.GetOrdinal("LaptopName")) ? null : reader.GetString(reader.GetOrdinal("LaptopName")),
                        SystemPassword = reader.IsDBNull(reader.GetOrdinal("SystemPassword")) ? null : reader.GetString(reader.GetOrdinal("SystemPassword")),
                        WindowsPassword = reader.IsDBNull(reader.GetOrdinal("WindowsPassword")) ? null : reader.GetString(reader.GetOrdinal("WindowsPassword")),
                        HardDrivePassword = reader.IsDBNull(reader.GetOrdinal("HardDrivePassword")) ? null : reader.GetString(reader.GetOrdinal("HardDrivePassword")),
                        FreezePassword = reader.IsDBNull(reader.GetOrdinal("FreezePassword")) ? null : reader.GetString(reader.GetOrdinal("FreezePassword")),
                        Code = reader.IsDBNull(reader.GetOrdinal("Code")) ? null : reader.GetString(reader.GetOrdinal("Code")),
                        Type = reader.IsDBNull(reader.GetOrdinal("Type")) ? null : reader.GetString(reader.GetOrdinal("Type")),
                        SerialNumber = reader.IsDBNull(reader.GetOrdinal("SerialNumber")) ? null : reader.GetString(reader.GetOrdinal("SerialNumber")),
                        Card = reader.IsDBNull(reader.GetOrdinal("Card")) ? null : reader.GetString(reader.GetOrdinal("Card")),
                        Comment = reader.IsDBNull(reader.GetOrdinal("Comment")) ? null : reader.GetString(reader.GetOrdinal("Comment")),
                        ContactNumber = reader.IsDBNull(reader.GetOrdinal("ContactNumber")) ? null : reader.GetString(reader.GetOrdinal("ContactNumber")),
                        UserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? null : reader.GetString(reader.GetOrdinal("UserName"))
                    }
                });

            if (!devices.Any())
                return null;

            var device = devices.First().Device;

            // ❌ Do NOT reuse the same SqlParameter instance.
            // ✅ Create a new one for the second call
            var operationParams = new[] { new SqlParameter("@Id", id) };

            // Fetch operations
            var operations = await _dataAccessLayer.ExecuteQueryAsync(
                "sp_GetDeviceById_Operations",
                operationParams,
                reader => new OperationDto
                {
                    OperationName = reader.IsDBNull(reader.GetOrdinal("OperationName")) ? null : reader.GetString(reader.GetOrdinal("OperationName")),
                    OldValue = reader.IsDBNull(reader.GetOrdinal("OldValue")) ? null : reader.GetString(reader.GetOrdinal("OldValue")),
                    NewValue = reader.IsDBNull(reader.GetOrdinal("NewValue")) ? null : reader.GetString(reader.GetOrdinal("NewValue")),
                    Comment = reader.IsDBNull(reader.GetOrdinal("Comment")) ? null : reader.GetString(reader.GetOrdinal("Comment")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? null : reader.GetString(reader.GetOrdinal("UserName"))
                });

            device.OperationsDtos = operations.ToList();

            return device;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }




    public async Task DeleteDeviceAsync(int id)
    {
        var userId = AppSession.CurrentUserId;

        try
        {
            var parameters = new[]
            {
              new SqlParameter("@Id", id),
              new SqlParameter("@UpdatedAt", DateTime.Now),
              new SqlParameter("@UserId",userId )
            };

            await _dataAccessLayer.ExecuteNonQueryAsync("sp_DeleteDevice", parameters);
        }
        catch (SqlException ex) when (ex.Message.Contains("الجهاز بالمعرف غير موجود"))
        {
            throw new KeyNotFoundException($"الجهاز بالمعرف {id} غير موجود.");
        }
    }

    private void TrackChange(List<Operation> operations, GetDeviceDto device, Func<GetDeviceDto, string?> selector, string? newValue, int deviceId, string operationName)
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
        }
    }


}