using System.ComponentModel.DataAnnotations;
namespace DeliveryAppSystem.Models;
public class User
{
    public int UserId { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    public string Role { get; set; } // "Client" or "Driver"

    public ICollection<Rating> RatingsGiven { get; set; }

    // ✅ Computed property
    public string FullName => $"{FirstName} {LastName}";
}

