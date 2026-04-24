namespace ClinicalTrialsApi.Models;

public class ClinicalTrial
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phase { get; set; } = string.Empty; // "I", "II", "III", "IV"
    public int PatientCount { get; set; }
    public string Status { get; set; } = string.Empty; // "Recruiting", "Active", "Completed"
    public DateTime StartDate { get; set; }

    // Relación: un trial tiene muchos pacientes
    public List<Patient> Patients { get; set; } = new ();
}