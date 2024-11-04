namespace SuccessfulAdmission.DataLogic.Models;

public class ApplicantModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Dictionary<SubjectModel, int> Results { get; set; } = new Dictionary<SubjectModel, int>();
}