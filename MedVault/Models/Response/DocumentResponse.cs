namespace MedVault.Models.Response;

public class DocumentResponse
{
    public int documentId { get; set; }
    
    public DateTime? dateAdmission { get; set; }
    
    public string? doctorName { get; set; }
    
    public string? doctorSpecialty { get; set; }
    
    public string documentType { get; set; }
    
    public string? analyzes { get; set; }
    
    public int? price { get; set; }
    
    public int? adress { get; set; }
    
    public string? name { get; set; }
    
    public string? city { get; set; }
    
    public string? street { get; set; }
}