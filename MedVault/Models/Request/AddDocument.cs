namespace MedVault.Models.Request;

public class AddDocument
{
    
    public DateTime? DateAdmission { get; set; }
    public string? DoctorName { get; set; }
    public string? DoctorSpecialty { get; set; }
    public string DocumentType { get; set; }
    public string? Analyzes { get; set; }
    public int? Price { get; set; }
}