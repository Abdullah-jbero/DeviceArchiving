namespace DeviceArchiving.Data.Entities;
public class Operation
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public string OperationName { get; set; } = null!;
    public string? oldValue { get; set; }
    public string? newValue { get; set; }
    public DateTime CreatedAt { get; set; }


}