namespace DeviceArchiving.Data.Dto.Devices;


public class CheckDuplicateDto
{
    public string SerialNumber { get; set; } = string.Empty;
    public string LaptopName { get; set; } = string.Empty;
}

public class DuplicateCheckResponse
{
    public List<string> DuplicateSerialNumbers { get; set; } = new List<string>();
    public List<string> DuplicateLaptopNames { get; set; } = new List<string>();
}

public class DeviceUploadDto : CreateDeviceDto
{
    public bool IsUpdate { get; set; } 
}