using SuccessfulAdmission.DataLogic.Models;

namespace SuccessfulAdmission.Web;

public class ApiClient
{
    public static UserModel? Client { get; set; } = null;
    
    public int Id = Client?.Id ?? 0;

    public string Login = Client?.Login ?? string.Empty;
    
    public string Password = Client?.Password ?? string.Empty;
    
    public string Email = Client?.Email ?? string.Empty;

    public bool IsAdmin = Client?.IsAdmin ?? false;
    
    public bool IsTwoFactor = Client?.IsTwoFactor ?? false;
    
    public string Key = Client?.Key ?? string.Empty;
    
    public string Qr = Client?.Qr ?? string.Empty;
}