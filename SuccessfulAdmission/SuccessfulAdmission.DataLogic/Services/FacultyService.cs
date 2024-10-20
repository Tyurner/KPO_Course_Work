using System.Data.SqlClient;
using SuccessfulAdmission.DataLogic.Models;

namespace SuccessfulAdmission.DataLogic.Services;

public class FacultyService
{
    private string _connectionString = new ConnectionStringReader().GetConnectionString();

    public List<FacultyModel> GetAllFaculties()
    {
        string query = "SELECT Id, Name, Description FROM [dbo].[Faculty]";
        List<FacultyModel> faculties = new List<FacultyModel>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        FacultyModel faculty = new FacultyModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2)
                        };
                        faculties.Add(faculty);
                    }
                }
            }
        }
        return faculties;
    }
    
    public FacultyModel GetFacultyById(int id)
    {
        string query = "SELECT Id, Name, Description FROM [dbo].[Faculty] WHERE Id = @Id";
        FacultyModel faculty = new FacultyModel();
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
                        faculty = new FacultyModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2)
                        };
                    }
                }
            }
        }
        return faculty;
    }
    
    public void AddFaculty(string name, string? description)
    {
        string query = "INSERT INTO [dbo].[Faculty] (Name, Description) VALUES (@Name, @Description)";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Description", description);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public void UpdateFaculty(int id, string name, string? description)
    {
        string query = "UPDATE [dbo].[Faculty] SET Name = @Name, Description = @Description WHERE Id = @Id";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Description", description);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public void DeleteFaculty(int id)
    {
        string query = "DELETE FROM [dbo].[Faculty] WHERE Id = @Id";
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
}