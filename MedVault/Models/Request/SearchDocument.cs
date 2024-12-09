namespace MedVault.Models.Request;

public class SearchDocument
{
    public DateTime? DateStart { get; set; }
    public DateTime? DateEnd { get; set; }
    public string? DoctorName { get; set; }
    public string? DoctorSpeciality { get; set; }
    public string? DocumentType { get; set; }
    public string? Analyzes { get; set; }
    public double? PriceStart { get; set; }
    public double? PriceEnd { get; set; }
}