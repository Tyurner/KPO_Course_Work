namespace SuccessfulAdmission.DataLogic.Models;

public class SubjectModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxPoints { get; set; }
    
    public override bool Equals(object obj)
    {
        return obj is SubjectModel subject && Id == subject.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}