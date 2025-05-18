namespace DeviceArchiving.Data.Entities;
public class Device
{
    public int Id { get; set; }
    public string Source { get; set; } =  null!;
    public string BrotherName { get; set; } =  null!;
    public string LaptopName { get; set; } =  null!;
    public string SystemPassword { get; set; } =  null!;
    public string WindowsPassword { get; set; } =  null!;
    public string HardDrivePassword { get; set; } =  null!;
    public string FreezePassword { get; set; } =  null!;
    public string Code { get; set; } =  null!;
    public string Type { get; set; } =  null!;
    public string SerialNumber { get; set; } =  null!;
    public string Card { get; set; } =  null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public List<Operation> Operation { get; set; } = [];
    public User User { get; set; } = new();

}
