using System.ComponentModel.DataAnnotations;

namespace Models;

public class User
{
    [Required]
    [StringLength(50, ErrorMessage = "The ID length can't exceed 50 characters.")]
    public string ID { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The name length can't exceed 100 characters.")]
    public string Name { get; set; }

    public string Email { get; set; }
    public string Address { get; set; }
    public string Password { get; set; }
    public bool isManager { get; set; }
}