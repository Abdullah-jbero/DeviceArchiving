using System.ComponentModel.DataAnnotations;

namespace DeviceArchiving.Data.Dto;

public class CreateUserRequest
{
    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = null!;
    public byte[] Picture { get; set; } = null!;

}



