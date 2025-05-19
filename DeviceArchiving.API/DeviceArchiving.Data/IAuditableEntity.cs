namespace DeviceArchiving.Data;

public interface IAuditableEntity
{
    int UserId { get; set; }
}