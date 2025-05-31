using System.ComponentModel.DataAnnotations;

namespace task_4.Models;

public class LoginViewModel {
    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Неверный формат email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Введите пароль")]
    public string Password { get; set; }
}