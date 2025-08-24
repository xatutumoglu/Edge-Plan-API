namespace EdgePlan.API.Settings;

public class EmailOptions
{
    public string Host { get; set; } = "";
    
    public int Port { get; set; } = 587;
    
    public bool UseStartTls { get; set; } = true;
    
    public string? UserName { get; set; }
    
    public string? Password { get; set; }
    
    public string From { get; set; } = "";
    
    public string FromName { get; set; } = "EdgePlan";
    
    public string[]? AdminRecipients { get; set; }
}