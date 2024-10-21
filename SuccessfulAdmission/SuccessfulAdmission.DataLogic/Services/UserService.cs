using System.Data.SqlClient;
using SuccessfulAdmission.DataLogic.Models;

namespace SuccessfulAdmission.DataLogic.Services;

public class UserService
{
    private string _connectionString = new ConnectionStringReader().GetConnectionString();

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
        string query = "SELECT Id, Login, Password, Email, IsAdmin, IsTwoFactor, [Key], Qr FROM [dbo].[User] WHERE Id = @Id";
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
    
    public void AddUser(string login, string password, string email, bool isAdmin, bool isTwoFactor, string? key, string? qr)
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
    
    public void UpdateUser(int id, string login, string password, string email, bool isAdmin, bool isTwoFactor, string? key, string? qr)
    {
        string query = "UPDATE [dbo].[User] SET Login = @Login, Password = @Password, Email = @Email, IsAdmin = @IsAdmin, IsTwoFactor = @IsTwoFactor, [Key] = @Key, Qr = @Qr WHERE Id = @Id";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
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
}