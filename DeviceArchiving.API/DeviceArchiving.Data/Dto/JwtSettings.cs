namespace DeviceArchiving.Data.Dto;

public class JwtSettings
{
    /// <summary>
    /// Secret key used to sign the JWT.
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    /// Issuer of the JWT (e.g., your application's URL).
    /// </summary>
    public string Issuer { get; set; } = null!;

    /// <summary>
    /// Audience for the JWT (e.g., intended clients or users).
    /// </summary>
    public string Audience { get; set; } = null!;

    /// <summary>
    /// Duration in minutes the JWT is valid for.
    /// </summary>
    public int DurationInMinutes { get; set; }
}
