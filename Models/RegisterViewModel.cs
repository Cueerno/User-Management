using System.ComponentModel.DataAnnotations;

namespace task_4.Models;

public class RegisterViewModel {
    [Required(ErrorMessage = "Enter name")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Enter email")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Enter password")]
    public string Password { get; set; }
}