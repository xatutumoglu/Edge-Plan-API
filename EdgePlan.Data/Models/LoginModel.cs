namespace EdgePlan.Data.Models;

public class UserLoginRequestModel
{
    public string Email { get; set; }
    
    public string Password { get; set; }
}

public class UserLoginResponseModel
{
    public string Token { get; set; }
}