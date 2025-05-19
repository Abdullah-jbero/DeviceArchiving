using System.ComponentModel.DataAnnotations;

namespace DeviceArchiving.Data.Dto.Users;

public class CreateUserRequest
{
    [Required(ErrorMessage = "UserName is required.")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = null!;
    public byte[] Picture { get; set; } = null!;

}



