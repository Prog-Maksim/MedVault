namespace MedVault.Models.Response;

public class AuthResponse
{
    public string message { get; set; }
    public int token_expires { get; set; }
    public string access_token { get; set; }
    public string refresh_token { get; set; }
}