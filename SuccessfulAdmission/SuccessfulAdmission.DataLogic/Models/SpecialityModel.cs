namespace SuccessfulAdmission.DataLogic.Models;

public class SpecialityModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public int CountOfPlaces { get; set; }
    public int? FacultyId { get; set; }
}