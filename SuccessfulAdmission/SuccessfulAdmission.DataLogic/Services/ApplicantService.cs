using System.Data.SqlClient;
using SuccessfulAdmission.DataLogic.Models;

namespace SuccessfulAdmission.DataLogic.Services;

public class ApplicantService
{
    private string _connectionString = new ConnectionStringReader().GetConnectionString();
    private SubjectService subjectService = new SubjectService();

    public List<ApplicantModel> GetAllApplicants()
    {
        string query = "SELECT a.Id, a.Name, s.Id AS SubjectId, r.Points FROM [dbo].[Applicant] a " +
                       "LEFT JOIN [dbo].[Applicant_Subject] r ON a.Id = r.ApplicantId " +
                       "LEFT JOIN [dbo].[Subject] s ON r.SubjectId = s.Id";
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
                        int applicantId = reader.GetInt32(0);
                        string applicantName = reader.GetString(1);

                        // Находим или создаем абитуриента
                        var applicant = applicants.FirstOrDefault(a => a.Id == applicantId);
                        if (applicant == null)
                        {
                            applicant = new ApplicantModel
                            {
                                Id = applicantId,
                                Name = applicantName,
                                Results = new Dictionary<SubjectModel, int>()
                            };
                            applicants.Add(applicant);
                        }
                        // Проверяем, есть ли предмет и балл
                        if (!reader.IsDBNull(2) && !reader.IsDBNull(3))
                        {
                            int SubjectId = reader.GetInt32(2);
                            int score = reader.GetInt32(3);
                            if (!string.IsNullOrEmpty(subjectService.GetSubjectById(SubjectId).Name))
                                applicant.Results[subjectService.GetSubjectById(SubjectId)] = score;
                        }
                    }
                }
            }
        }
        return applicants;
    }
    
    public List<ApplicantModel> GetApplicantsBySpecialityId(int specialityId)
    {
        string query = "SELECT a.Id, a.Name, s.Id AS SubjectId, r.Points FROM [dbo].[Applicant] a " +
                       "INNER JOIN [dbo].[Applicant_Speciality] aspec ON a.Id = aspec.ApplicantId " + 
                       "LEFT JOIN [dbo].[Applicant_Subject] r ON a.Id = r.ApplicantId " +
                       "LEFT JOIN [dbo].[Subject] s ON r.SubjectId = s.Id " +
                       "WHERE aspec.SpecialityId = @SpecialityId";
        List<ApplicantModel> applicants = new List<ApplicantModel>();
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
                        int applicantId = reader.GetInt32(0);
                        string applicantName = reader.GetString(1);

                        // Находим или создаем абитуриента
                        var applicant = applicants.FirstOrDefault(a => a.Id == applicantId);
                        if (applicant == null)
                        {
                            applicant = new ApplicantModel
                            {
                                Id = applicantId,
                                Name = applicantName,
                                Results = new Dictionary<SubjectModel, int>()
                            };
                            applicants.Add(applicant);
                        }
                        // Проверяем, есть ли предмет и балл
                        if (!reader.IsDBNull(2) && !reader.IsDBNull(3))
                        {
                            int SubjectId = reader.GetInt32(2);
                            int score = reader.GetInt32(3);
                            if (!string.IsNullOrEmpty(subjectService.GetSubjectById(SubjectId).Name))
                                applicant.Results[subjectService.GetSubjectById(SubjectId)] = score;
                        }
                    }
                }
            }
        }
        return applicants;
    }
    
    public ApplicantModel GetApplicantById(int id)
    {
        string query = "SELECT a.Id, a.Name, s.Id AS SubjectId, r.Points FROM [dbo].[Applicant] a " +
                       "LEFT JOIN [dbo].[Applicant_Subject] r ON a.Id = r.ApplicantId " +
                       "LEFT JOIN [dbo].[Subject] s ON r.SubjectId = s.Id WHERE a.Id = @Id";
        ApplicantModel applicant = new ApplicantModel();
        applicant.Results = new Dictionary<SubjectModel, int>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        applicant.Id = reader.GetInt32(0);
                        applicant.Name = reader.GetString(1);
                        if (!reader.IsDBNull(2) && !reader.IsDBNull(3))
                        {
                            int SubjectId = reader.GetInt32(2);
                            int score = reader.GetInt32(3);
                            if (!string.IsNullOrEmpty(subjectService.GetSubjectById(SubjectId).Name))
                                applicant.Results[subjectService.GetSubjectById(SubjectId)] = score;
                        }
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
    
    public void AddApplicantSubject(int applicantId, int subjectId, int points)
    {
        string query = "MERGE INTO Applicant_Subject AS target " +
                       "USING (VALUES (@ApplicantId, @SubjectId, @Points)) AS source (ApplicantId, SubjectId, Points) " +
                       "ON target.ApplicantId = source.ApplicantId AND target.SubjectId = source.SubjectId " +
                       "WHEN MATCHED THEN " +
                       "UPDATE SET target.Points = source.Points " +
                       "WHEN NOT MATCHED THEN " +
                       "INSERT (ApplicantId, SubjectId, Points) " +
                       "VALUES (source.ApplicantId, source.SubjectId, source.Points);";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ApplicantId", applicantId);
                command.Parameters.AddWithValue("@SubjectId", subjectId);
                command.Parameters.AddWithValue("@Points", points);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public void DeleteApplicantSubject(int applicantId, int subjectId)
    {
        string query = "DELETE FROM [dbo].[Applicant_Subject] WHERE ApplicantId=@ApplicantId AND SubjectId=@SubjectId";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ApplicantId", applicantId);
                command.Parameters.AddWithValue("@SubjectId", subjectId);
                command.ExecuteNonQuery();
            }
        }
    }
}