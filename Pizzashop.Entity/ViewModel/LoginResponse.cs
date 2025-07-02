namespace Pizzashop.Entity.ViewModel;

public class LoginResponse
{
    public string AccessToken {get;set;} = null!;
    public string? RefreshToken {get;set;}
}
