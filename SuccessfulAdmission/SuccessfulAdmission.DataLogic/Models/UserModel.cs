namespace SuccessfulAdmission.DataLogic.Models;

public class UserModel
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
    public bool IsTwoFactor { get; set; } = false;
    public string? Key { get; set; } = string.Empty;
    public string? Qr { get; set; } = string.Empty;
}