namespace MedVaultBackend.Models.DB;

public class Documents
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public string PersonId { get; set; }
    public DateTime DocumentAdded { get; set; }

    public DateTime? DateAdmission { get; set; }
    public string? DoctorName { get; set; }
    public string? DoctorSpecialty { get; set; }
    public string DocumentType { get; set; }
    public string? Surveys { get; set; }
    public int? Price { get; set; }
    public int? PaymentResult { get; set; }
    public int? Address { get; set; }
    
    public MedicalInstitution? MedicalInstitution { get; set; }
    public Users User { get; set; } = null!;
}