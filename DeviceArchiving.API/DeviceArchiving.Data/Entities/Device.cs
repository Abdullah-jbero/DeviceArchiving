namespace DeviceArchiving.Data.Entities;
public class Device : IAuditableEntity
{
    public int Id { get; set; }
    public string Source { get; set; } = null!;
    public string BrotherName { get; set; } = null!;
    public string LaptopName { get; set; } = null!;
    public string SystemPassword { get; set; } = null!;
    public string WindowsPassword { get; set; } = null!;
    public string HardDrivePassword { get; set; } = null!;
    public string FreezePassword { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public string? Comment { get; set; }
    public string? ContactNumber { get; set; }
    public string Card { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public List<Operation> Operations { get; set; } = new();
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}

