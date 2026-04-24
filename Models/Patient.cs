namespace ClinicalTrialsApi.Models;

public class Patient
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime EnrolledAt { get; set; }
    public int ClinicalTrialId { get; set; }
    public ClinicalTrial? ClinicalTrial { get; set; }
}
