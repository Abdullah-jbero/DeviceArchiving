namespace DeviceArchiving.Data.Entities;
public class Operation : IAuditableEntity
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public string OperationName { get; set; } = null!;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public User User { get; set; } = null!;
    public int UserId { get; set; }


}
