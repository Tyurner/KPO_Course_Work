namespace SuccessfulAdmission.DataLogic.Models;

public class ListApplicantsModel
{
    public int SpecialityId { get; set; }
    public SpecialityModel Speciality { get; set; } = new SpecialityModel();
    public Dictionary<ApplicantModel, int> Applicants { get; set; } = new Dictionary<ApplicantModel, int>();
    public List<SubjectModel> Subjects { get; set; } = new List<SubjectModel>();
}