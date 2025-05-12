namespace DeviceArchiving.Data.Entities;
public class Device
{
    public int Id { get; set; }
    public string Source { get; set; } = string.Empty;
    public string BrotherName { get; set; } = string.Empty;
    public string LaptopName { get; set; } = string.Empty;
    public string SystemPassword { get; set; } = string.Empty;
    public string WindowsPassword { get; set; } = string.Empty;
    public string HardDrivePassword { get; set; } = string.Empty;
    public string FreezePassword { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Card { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public List<Operation> Operation { get; set; } = [];
}