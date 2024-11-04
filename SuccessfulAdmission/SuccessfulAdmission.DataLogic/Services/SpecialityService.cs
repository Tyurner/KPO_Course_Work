using System.Data.SqlClient;
using SuccessfulAdmission.DataLogic.Models;

namespace SuccessfulAdmission.DataLogic.Services;

public class SpecialityService
{
    private string _connectionString = new ConnectionStringReader().GetConnectionString();

    public List<SpecialityModel> GetAllSpecialities()
    {
        string query = "SELECT Id, Name, Description, CountOfPlaces, FacultyId FROM [dbo].[Speciality]";
        List<SpecialityModel> specialities = new List<SpecialityModel>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SpecialityModel speciality = new SpecialityModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                            CountOfPlaces = reader.GetInt32(3),
                            FacultyId = reader.IsDBNull(4) ? null : reader.GetInt32(4)
                        };
                        specialities.Add(speciality);
                    }
                }
            }
        }
        return specialities;
    }
    
    public List<SpecialityModel> GetSpecialitiesByFacultyId(int facultyId)
    {
        string query = "SELECT Id, Name, Description, CountOfPlaces, FacultyId FROM [dbo].[Speciality] WHERE FacultyId = @FacultyId";
        List<SpecialityModel> specialities = new List<SpecialityModel>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FacultyId", facultyId);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SpecialityModel speciality = new SpecialityModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                            CountOfPlaces = reader.GetInt32(3),
                            FacultyId = reader.IsDBNull(4) ? null : reader.GetInt32(4)
                        };
                        specialities.Add(speciality);
                    }
                }
            }
        }
        return specialities;
    }
    
    public SpecialityModel GetSpecialityById(int id)
    {
        string query = "SELECT Id, Name, Description, CountOfPlaces, FacultyId FROM [dbo].[Speciality] WHERE Id = @Id";
        SpecialityModel speciality = new SpecialityModel();
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
                        speciality = new SpecialityModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                            CountOfPlaces = reader.GetInt32(3),
                            FacultyId = reader.IsDBNull(4) ? null : reader.GetInt32(4)
                        };
                    }
                }
            }
        }
        return speciality;
    }
    
    public void AddSpeciality(string name, string? description, int count, int? facultyId)
    {
        string query = "INSERT INTO [dbo].[Speciality] (Name, Description, CountOfPlaces, FacultyId) VALUES (@Name, @Description, @CountOfPlaces, @FacultyId)";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@CountOfPlaces", count);
                command.Parameters.AddWithValue("@FacultyId", facultyId);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public void UpdateSpeciality(int id, string name, string? description, int count, int? facultyId)
    {
        string query = "UPDATE [dbo].[Speciality] SET Name = @Name, Description = @Description, CountOfPlaces = @CountOfPlaces, FacultyId = @FacultyId WHERE Id = @Id";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@CountOfPlaces", count);
                command.Parameters.AddWithValue("@FacultyId", facultyId);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public void DeleteSpeciality(int id)
    {
        string query = "DELETE FROM [dbo].[Speciality] WHERE Id = @Id";
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
    
    public void AddSpecialitySubject(int specialityId, int subjectId)
    {
        string query = "IF NOT EXISTS (SELECT 1 FROM [dbo].[Speciality_Subject] " +
                       "WHERE SpecialityId = @SpecialityId AND SubjectId = @SubjectId) " +
                       "BEGIN " +
                       "INSERT INTO [dbo].[Speciality_Subject] (SpecialityId, SubjectId) " +
                       "VALUES (@SpecialityId, @SubjectId); " +
                       "END";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@SpecialityId", specialityId);
                command.Parameters.AddWithValue("@SubjectId", subjectId);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public void DeleteSpecialitySubject(int specialityId, int subjectId)
    {
        string query = "DELETE FROM [dbo].[Speciality_Subject] WHERE SpecialityId=@SpecialityId AND SubjectId=@SubjectId";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@SpecialityId", specialityId);
                command.Parameters.AddWithValue("@SubjectId", subjectId);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public void AddSpecialityApplicant(int specialityId, int applicantId)
    {
        string query = "IF NOT EXISTS (SELECT 1 FROM [dbo].[Applicant_Speciality] " +
                       "WHERE SpecialityId = @SpecialityId AND ApplicantId = @ApplicantId) " +
                       "BEGIN " +
                       "INSERT INTO [dbo].[Applicant_Speciality] (SpecialityId, ApplicantId) " +
                       "VALUES (@SpecialityId, @ApplicantId); " +
                       "END";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@SpecialityId", specialityId);
                command.Parameters.AddWithValue("@ApplicantId", applicantId);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public void DeleteSpecialityApplicant(int specialityId, int applicantId)
    {
        string query = "DELETE FROM [dbo].[Applicant_Speciality] WHERE SpecialityId=@SpecialityId AND ApplicantId=@ApplicantId";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@SpecialityId", specialityId);
                command.Parameters.AddWithValue("@ApplicantId", applicantId);
                command.ExecuteNonQuery();
            }
        }
    }
}