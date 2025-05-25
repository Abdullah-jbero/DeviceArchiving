using DeviceArchiving.Data.Dto;
using DeviceArchiving.Data.Dto.Devices;
using DeviceArchiving.Data.Entities;

namespace DeviceArchiving.Service;

public interface IDeviceService
{
    Task<BaseResponse<string>> AddDeviceAsync(CreateDeviceDto dto);

    Task<List<GetAllDevicesDto>> GetAllDevicesAsync();
    Task<GetDeviceDto?> GetDeviceByIdAsync(int id);
    Task<BaseResponse<string>> UpdateDeviceAsync(int id, UpdateDeviceDto dto);
    Task DeleteDeviceAsync(int id);
    Task<BaseResponse<DuplicateCheckResponse>> CheckDuplicatesAsync(List<CheckDuplicateDto> items);
    Task<BaseResponse<int>> ProcessDevicesAsync(List<DeviceUploadDto> dtos);
}



