namespace EdgePlan.Data.Entity;

public class Status
{
    public Guid Id { get; set; }
    
    public string Text { get; set; }
    
    public ICollection<Target> Targets { get; set; } = new List<Target>();
}