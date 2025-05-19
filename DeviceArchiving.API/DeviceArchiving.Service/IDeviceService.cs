using DeviceArchiving.Data.Dto.Devices;
using DeviceArchiving.Data.Entities;

namespace DeviceArchiving.Service;

public interface IDeviceService
{
    Task AddDeviceAsync(CreateDeviceDto dto);
    Task<List<GetAllDevicesDto>> GetAllDevicesAsync();
    Task<GetDeviceDto?> GetDeviceByIdAsync(int id);
    Task UpdateDeviceAsync(int id, UpdateDeviceDto dto);
    Task DeleteDeviceAsync(int id);
}



