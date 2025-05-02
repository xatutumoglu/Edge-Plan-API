using System.ComponentModel.DataAnnotations;

namespace EdgePlan.Data.Entity;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required, MinLength(2)]
    public string FirstName { get; set; }

    [Required, MinLength(2)]
    public string LastName { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Target> Targets { get; set; } = new List<Target>();
}