using System.ComponentModel.DataAnnotations;

namespace MedVaultBackend.Models.Requests;

public class AddDocument
{
    
    public DateTime? DateAdmission { get; set; } = null;
    public string? DoctorName { get; set; } = null;
    public string? DoctorSpecialty { get; set; } = null;
    
    [Required(AllowEmptyStrings = false, ErrorMessage = "Поле DocumentType обязательно к заполнению")]
    public string DocumentType { get; set; }
    public string? Analyzes { get; set; } = null;
    public int? Price { get; set; } = null;
}