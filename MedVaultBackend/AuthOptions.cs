using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MedVaultBackend;

public class AuthOptions
{
    public const string ISSUER = "MyAuthServer"; // издатель токена
    public const string AUDIENCE = "https://axample.com"; // потребитель токена
    private static readonly string KEY = Environment.GetEnvironmentVariable("JWT_KEY");

    public static SymmetricSecurityKey GetSymmetricSecurityKey() => new (Encoding.UTF8.GetBytes(KEY));
}