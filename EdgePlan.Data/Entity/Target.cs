using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EdgePlan.Data.Entity;

public class Target
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid StatusId { get; set; }

    public string Text { get; set; }

    public DateTime? DeadLine { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ChangedAt { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    
    [ForeignKey("StatusId")]
    public Status Status { get; set; } = null!;
}