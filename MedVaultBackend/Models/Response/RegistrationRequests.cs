using System.Text.Json.Serialization;

namespace MedVaultBackend.Models.Response;

/// <summary>
/// Класс с токенами пользователя
/// </summary>
public class RegistrationRequests
{
    /// <summary>
    /// сообщение об успешном созданном пользователе
    /// </summary>
    public string message { get; set; }
    
    
    /// <summary>
    /// Время жизни access токена в минутах
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int token_expires { get; set; }
    
    /// <summary>
    /// Access токен
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string access_token { get; set; }
    
    /// <summary>
    /// Refresh токен
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string refresh_token { get; set; }
}