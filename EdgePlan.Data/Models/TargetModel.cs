
using EdgePlan.Data.Entity;
using EdgePlan.Data.Enums;

namespace EdgePlan.Data.Models;

public class TargetCreateModel
{
    public string Text { get; set; }
    
    public DateTime DeadLine { get; set; }
}

public class TargetUpdateModel
{
    public string Text { get; set; }
    
    public DateTime DeadLine { get; set; }

    public TargetStatus Status { get; set; }
}

