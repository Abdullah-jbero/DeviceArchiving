namespace DeviceArchiving.Data.Dto.Devices;

public class GetDeviceDto : GetAllDevicesDto
{
    public List<OperationDto> OperationsDtos { get; set; } = new();

}
public class OperationDto
{
    public string OperationName { get; set; } = null!;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserName { get; set; } = null!;
}