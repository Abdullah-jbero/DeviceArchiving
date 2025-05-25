using DeviceArchiving.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace DeviceArchiving.Data.Dto.Devices;

public class CreateDeviceDto
{
    public string Source { get; set; } = null!;
    public string BrotherName { get; set; } = null!;
    [Required(ErrorMessage = "اسم اللاب توب مطلوب")]
    public string LaptopName { get; set; } = null!;
    public string SystemPassword { get; set; } = null!;
    public string WindowsPassword { get; set; } = null!;
    public string HardDrivePassword { get; set; } = null!;
    public string FreezePassword { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Type { get; set; } = null!;
    [Required(ErrorMessage = "رقم التسلسل مطلوب")]
    public string SerialNumber { get; set; } = null!;
    public string? Comment { get; set; }
    public string? ContactNumber { get; set; }
    public string Card { get; set; } = null!;
}



