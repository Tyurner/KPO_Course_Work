using System.Data.SqlClient;
using Google.Authenticator;
using SuccessfulAdmission.DataLogic.Models;

namespace SuccessfulAdmission.DataLogic.Services;

public class UserService
{
    private string _connectionString = new ConnectionStringReader().GetConnectionString();
    private static readonly TwoFactorAuthenticator TwoFactorAuthenticator = new TwoFactorAuthenticator();

    public List<UserModel> GetAllUsers()
    {
        string query = "SELECT Id, Login, Password, Email, IsAdmin, IsTwoFactor, [Key], Qr FROM [dbo].[User]";
        List<UserModel> users = new List<UserModel>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UserModel user = new UserModel
                        {
                            Id = reader.GetInt32(0),
                            Login = reader.GetString(1),
                            Password = reader.GetString(2),
                            Email = reader.GetString(3),
                            IsAdmin = reader.GetBoolean(4),
                            IsTwoFactor = reader.GetBoolean(5),
                            Key = reader.IsDBNull(6) ? null : reader.GetString(6),
                            Qr = reader.IsDBNull(7) ? null : reader.GetString(7) 
                        };
                        users.Add(user);
                    }
                }
            }
        }
        return users;
    }
    
    public UserModel GetUserById(int id)
    {
        string query = "SELECT TOP 1 Id, Login, Password, Email, IsAdmin, IsTwoFactor, [Key], Qr FROM [dbo].[User] WHERE Id = @Id";
        UserModel user = new UserModel();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserModel
                        {
                            Id = reader.GetInt32(0),
                            Login = reader.GetString(1),
                            Password = reader.GetString(2),
                            Email = reader.GetString(3),
                            IsAdmin = reader.GetBoolean(4),
                            IsTwoFactor = reader.GetBoolean(5),
                            Key = reader.IsDBNull(6) ? null : reader.GetString(6),
                            Qr = reader.IsDBNull(7) ? null : reader.GetString(7) 
                        };
                    }
                }
            }
        }
        return user;
    }
    
    public UserModel? GetUserByLoginPassword(string login, string password)
    {
        string query = "SELECT TOP 1 Id, Login, Password, Email, IsAdmin, IsTwoFactor, [Key], Qr FROM [dbo].[User] WHERE Login = @Login AND Password = @Password";
        UserModel user = new UserModel();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Password", password);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserModel
                        {
                            Id = reader.GetInt32(0),
                            Login = reader.GetString(1),
                            Password = reader.GetString(2),
                            Email = reader.GetString(3),
                            IsAdmin = reader.GetBoolean(4),
                            IsTwoFactor = reader.GetBoolean(5),
                            Key = reader.IsDBNull(6) ? null : reader.GetString(6),
                            Qr = reader.IsDBNull(7) ? null : reader.GetString(7) 
                        };
                    }
                }
            }
        }
        return user.Id > 0? user : null;
    }
    
    public UserModel? GetUserByLogin(string login)
    {
        string query = "SELECT TOP 1 Id, Login, Password, Email, IsAdmin, IsTwoFactor, [Key], Qr FROM [dbo].[User] WHERE Login = @Login";
        UserModel user = new UserModel();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Login", login);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserModel
                        {
                            Id = reader.GetInt32(0),
                            Login = reader.GetString(1),
                            Password = reader.GetString(2),
                            Email = reader.GetString(3),
                            IsAdmin = reader.GetBoolean(4),
                            IsTwoFactor = reader.GetBoolean(5),
                            Key = reader.IsDBNull(6) ? null : reader.GetString(6),
                            Qr = reader.IsDBNull(7) ? null : reader.GetString(7) 
                        };
                    }
                }
            }
        }
        return user.Id > 0? user : null;
    }
    
    public UserModel? GetUserByEmail(string email)
    {
        string query = "SELECT TOP 1 Id, Login, Password, Email, IsAdmin, IsTwoFactor, [Key], Qr FROM [dbo].[User] WHERE Email = @Email";
        UserModel user = new UserModel();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserModel
                        {
                            Id = reader.GetInt32(0),
                            Login = reader.GetString(1),
                            Password = reader.GetString(2),
                            Email = reader.GetString(3),
                            IsAdmin = reader.GetBoolean(4),
                            IsTwoFactor = reader.GetBoolean(5),
                            Key = reader.IsDBNull(6) ? null : reader.GetString(6),
                            Qr = reader.IsDBNull(7) ? null : reader.GetString(7) 
                        };
                    }
                }
            }
        }
        return user.Id > 0? user : null;
    }
    
    public void AddUser(string login, string password, string email, bool isAdmin=false, bool isTwoFactor=false, string? key="", string? qr="")
    {
        string query = "INSERT INTO [dbo].[User] (Login, Password, Email, IsAdmin, IsTwoFactor, [Key], Qr) VALUES (@Login, @Password, @Email, @IsAdmin, @IsTwoFactor, @Key, @Qr)";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@IsAdmin", isAdmin);
                command.Parameters.AddWithValue("@IsTwoFactor", isTwoFactor);
                command.Parameters.AddWithValue("@Key", key);
                command.Parameters.AddWithValue("@Qr", qr);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public void UpdateUser(int id, string login, string password, string email)
    {
        string query = "UPDATE [dbo].[User] SET Login = @Login, Password = @Password, Email = @Email WHERE Id = @Id";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@Email", email);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public void DeleteUser(int id)
    {
        string query = "DELETE FROM [dbo].[User] WHERE Id = @Id";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public void PromoteUser(int id)
    {
        string query = "UPDATE [dbo].[User] SET IsAdmin = @IsAdmin WHERE Id = @Id";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@IsAdmin", true);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public bool CheckCodeValid(string key, string code) => TwoFactorAuthenticator.ValidateTwoFactorPIN(key, code, true);
    
    public (string, string) RegisterUser(string issuer, string username, string secretKey = null)
    {
        // Если секретный ключ не предоставлен, сгенерируем новый.
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            secretKey = GenerateSecretKey();
        }

        // Генерация кода настройки и QR-кода.
        var setupCode = TwoFactorAuthenticator.GenerateSetupCode(
            issuer: issuer,
            accountTitleNoSpaces: username,
            accountSecretKey: secretKey,
            secretIsBase32: true, // Учитываем, что секретный ключ будет в Base32 формате.
            qrPixelsPerModule: 3);

        // Возвращаем URL для QR-кода и секретный ключ.
        return (secretKey, setupCode.QrCodeSetupImageUrl);
    }
    
    private string GenerateSecretKey()
    {
        // Длина секретного ключа для двухфакторной аутентификации (обычно 16 байт).
        var key = new byte[16];
        using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
        {
            rng.GetBytes(key);
        }

        return Base32Encoding.ToString(key);
    }

    public void UpdateTwoFactorSetting(int id, bool enableTwoFactor)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var command = new SqlCommand("UPDATE [dbo].[User] SET IsTwoFactor = @IsTwoFactor WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@IsTwoFactor", enableTwoFactor);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }
    
    public void UpdateTwoFactorKeys(int id, string key, string qr)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var command = new SqlCommand("UPDATE [dbo].[User] SET [Key] = @Key, Qr = @Qr WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Key", key);
            command.Parameters.AddWithValue("@Qr", qr);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}