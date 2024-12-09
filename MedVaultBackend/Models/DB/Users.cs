using MedVaultBackend.Models.DB;

namespace MedVaultBackend.Models;

public class Users
{
    public int Id { get; set; }
    public string PersonId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Patronymic { get; set; }
    public DateTime Birthday { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime CreateDate { get; set; }
    public int PasswordVersion { get; set; }
    
    public ICollection<Documents> Documents { get; set; } = new List<Documents>();
}