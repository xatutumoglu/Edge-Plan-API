using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EdgePlan.Data.Enums;

namespace EdgePlan.Data.Entity;

public class Target
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public TargetStatus Status { get; set; }
    
    [Required]
    public string Text { get; set; }

    [Required]
    public DateTime? DeadLine { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public DateTime? ChangedAt { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}