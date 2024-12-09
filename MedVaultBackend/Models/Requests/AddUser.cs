using System.ComponentModel.DataAnnotations;

namespace MedVaultBackend.Models.Requests;

public class AddUser
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Поле Name обязательно к заполнению")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Name должен быть меньше 100 символов")]
    public string Name { get; set; }
    
    /// <summary>
    /// Фамилия пользователя
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Поле Surname обязательно к заполнению")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Surname должен быть меньше 100 символов")]
    public string Surname { get; set; }
    
    /// <summary>
    /// Отчество пользователя
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Поле Patronimyc обязательно к заполнению")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Patronimyc должен быть меньше 100 символов")]
    public string Patronymic { get; set; }
    
    /// <summary>
    /// Дата рождения пользователя
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Поле Birthday обязательно к заполнению")]
    public DateTime Birthday { get; set; }
    
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