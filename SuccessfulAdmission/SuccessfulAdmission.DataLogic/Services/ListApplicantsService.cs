using System.Data.SqlClient;
using SuccessfulAdmission.DataLogic.Models;

namespace SuccessfulAdmission.DataLogic.Services;

public class ListApplicantsService
{
    private string _connectionString = new ConnectionStringReader().GetConnectionString();
    
    public ListApplicantsModel GetListApplicantsForSpeciality(int specialityId)
    {
        // Модель для возвращения
        var model = new ListApplicantsModel
        {
            SpecialityId = specialityId,
            Speciality = new SpecialityService().GetSpecialityById(specialityId)
        };

        // Заполняем список предметов для специальности
        model.Subjects = GetSubjectsForSpeciality(specialityId);

        // Заполняем абитуриентов для специальности и их баллы
        model.Applicants = GetApplicantsForSpeciality(specialityId, model.Subjects);

        return model;
    }
    
    // Получаем список предметов для специальности
    private List<SubjectModel> GetSubjectsForSpeciality(int specialityId)
    {
        string query = @"SELECT s.Id, s.Name, s.MaxPoints 
            FROM [dbo].[Subject] s
            JOIN [dbo].[Speciality_Subject] ss ON s.Id = ss.SubjectId
            WHERE ss.SpecialityId = @SpecialityId";

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
                        var subject = new SubjectModel
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

    // Получаем абитуриентов для специальности и рассчитываем их баллы
    private Dictionary<ApplicantModel, int> GetApplicantsForSpeciality(int specialityId, List<SubjectModel> subjects)
    {
        string query = @"SELECT a.Id, a.Name, r.SubjectId, r.Points
            FROM [dbo].[Applicant] a
            JOIN [dbo].[Applicant_Speciality] sa ON a.Id = sa.ApplicantId
            JOIN [dbo].[Applicant_Subject] r ON a.Id = r.ApplicantId
            WHERE sa.SpecialityId = @SpecialityId";

        var applicantsDict = new Dictionary<ApplicantModel, int>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@SpecialityId", specialityId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // Словарь для хранения результатов по абитуриентам
                    var applicantsResults = new Dictionary<int, ApplicantModel>();

                    while (reader.Read())
                    {
                        int applicantId = reader.GetInt32(0);
                        string applicantName = reader.GetString(1);
                        int subjectId = reader.GetInt32(2);
                        int score = reader.GetInt32(3);

                        // Получаем или создаем модель абитуриента
                        if (!applicantsResults.ContainsKey(applicantId))
                        {
                            applicantsResults[applicantId] = new ApplicantModel
                            {
                                Id = applicantId,
                                Name = applicantName
                            };
                        }

                        var applicant = applicantsResults[applicantId];

                        // Если предмет есть в списке специальности, добавляем баллы
                        var subject = subjects.FirstOrDefault(s => s.Id == subjectId);
                        if (subject != null)
                        {
                            if (!applicant.Results.ContainsKey(subject))
                            {
                                applicant.Results[subject] = 0;
                            }
                            applicant.Results[subject] += score;
                        }
                    }

                    // Теперь заполняем итоговый словарь абитуриентов
                    foreach (var applicant in applicantsResults.Values)
                    {
                        int totalScore = applicant.Results.Values.Sum();
                        applicantsDict[applicant] = totalScore;
                    }
                }
            }
        }
        // Сортировка словаря по баллам от большего к меньшему
        var sortedApplicantsDict = applicantsDict.OrderByDescending(kvp => kvp.Value)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return sortedApplicantsDict;
    }
}