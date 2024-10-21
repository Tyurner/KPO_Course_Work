using System.Data.SqlClient;
using SuccessfulAdmission.DataLogic.Models;

namespace SuccessfulAdmission.DataLogic.Services;

public class ApplicantService
{
    private string _connectionString = new ConnectionStringReader().GetConnectionString();

    public List<ApplicantModel> GetAllApplicants()
    {
        string query = "SELECT Id, Name FROM [dbo].[Applicant]";
        List<ApplicantModel> applicants = new List<ApplicantModel>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ApplicantModel applicant = new ApplicantModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        };
                        applicants.Add(applicant);
                    }
                }
            }
        }
        return applicants;
    }
    
    public ApplicantModel GetApplicantById(int id)
    {
        string query = "SELECT Id, Name FROM [dbo].[Applicant] WHERE Id = @Id";
        ApplicantModel applicant = new ApplicantModel();
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
                        applicant = new ApplicantModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        };
                    }
                }
            }
        }
        return applicant;
    }
    
    public void AddApplicant(string name)
    {
        string query = "INSERT INTO [dbo].[Applicant] (Name) VALUES (@Name)";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public void UpdateApplicant(int id, string name)
    {
        string query = "UPDATE [dbo].[Applicant] SET Name = @Name WHERE Id = @Id";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Name", name);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public void DeleteApplicant(int id)
    {
        string query = "DELETE FROM [dbo].[Applicant] WHERE Id = @Id";
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