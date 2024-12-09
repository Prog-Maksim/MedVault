namespace MedVaultBackend.Models.DB;

public class MedicalInstitution
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public string City { get; set; }
    public string Street { get; set; }
    
    public ICollection<Documents> Documents { get; set; } = new List<Documents>();
}