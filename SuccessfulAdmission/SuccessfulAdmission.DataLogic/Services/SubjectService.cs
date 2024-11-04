using System.Data.SqlClient;
using SuccessfulAdmission.DataLogic.Models;

namespace SuccessfulAdmission.DataLogic.Services;

public class SubjectService
{
    private string _connectionString = new ConnectionStringReader().GetConnectionString();

    public List<SubjectModel> GetAllSubjects()
    {
        string query = "SELECT Id, Name, MaxPoints FROM [dbo].[Subject]";
        List<SubjectModel> subjects = new List<SubjectModel>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SubjectModel subject = new SubjectModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            MaxPoints = reader.GetInt32(2)
                        };
                        subjects.Add(subject);
                    }
                }
            }
        }
        return subjects;
    }
    
    public List<SubjectModel> GetSubjectsBySpecialityId(int specialityId)
    {
        string query = "SELECT Id, Name, MaxPoints FROM [dbo].[Subject]" + 
                       "JOIN [dbo].[Speciality_Subject] ON [dbo].[Subject].Id = [dbo].[Speciality_Subject].SubjectId " +
                       "WHERE [dbo].[Speciality_Subject].SpecialityId = @SpecialityId";
        List<SubjectModel> subjects = new List<SubjectModel>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@SpecialityId", specialityId);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SubjectModel subject = new SubjectModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            MaxPoints = reader.GetInt32(2)
                        };
                        subjects.Add(subject);
                    }
                }
            }
        }
        return subjects;
    }
    
    public SubjectModel GetSubjectById(int id)
    {
        string query = "SELECT Id, Name, MaxPoints FROM [dbo].[Subject] WHERE Id = @Id";
        SubjectModel subject = new SubjectModel();
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
                        subject = new SubjectModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            MaxPoints = reader.GetInt32(2)
                        };
                    }
                }
            }
        }
        return subject;
    }
    
    public void AddSubject(string name, int maxPoints)
    {
        string query = "INSERT INTO [dbo].[Subject] (Name, MaxPoints) VALUES (@Name, @MaxPoints)";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@MaxPoints", maxPoints);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public void UpdateSubject(int id, string name, int maxPoints)
    {
        string query = "UPDATE [dbo].[Subject] SET Name = @Name, MaxPoints = @MaxPoints WHERE Id = @Id";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@MaxPoints", maxPoints);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public void DeleteSubject(int id)
    {
        string query = "DELETE FROM [dbo].[Subject] WHERE Id = @Id";
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