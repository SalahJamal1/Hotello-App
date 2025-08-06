using System.ComponentModel.DataAnnotations;

namespace MyApplication.Models.Users;

public class ApiUserDto
{
    [Required] public string FirstName { get; set; }

    [Required] public string LastName { get; set; }

    [EmailAddress] [Required] public string Email { get; set; }

    [Required]
    [MaxLength(16, ErrorMessage = "Password must be at least 16 characters long")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string Password { get; set; }
}