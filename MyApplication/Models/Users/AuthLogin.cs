using System.ComponentModel.DataAnnotations;

namespace MyApplication.Models.Users;

public class AuthLogin
{
    [EmailAddress] [Required] public string Email { get; set; }

    [Required] public string Password { get; set; }
}