using System.ComponentModel.DataAnnotations;

namespace MedVaultBackend.Models.Requests;

public class AuthUser
{
    /// <summary>
    /// Почта пользователя
    /// </summary>
    [EmailAddress(ErrorMessage = "Некорректный формат email")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Поле Email обязательно к заполнению")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Email должен быть меньше 100 символов")]
    public string Email { get; set; }
    
    /// <summary>
    /// Пароль пользователя
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Поле Password обязательно к заполнению")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "Password должен быть меньше 50 символов")]
    public string Password { get; set; }
}