namespace MedVault.Models.Request;

public class RegisrationModel
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Patronymic { get; set; }
    public DateTime Birthday { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}