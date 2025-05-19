namespace DeviceArchiving.Data.Dto;

public class CreateOperation 
{
    public int DeviceId { get; set; }
    public string OperationName { get; set; } = null!;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Comment { get; set; }



}
