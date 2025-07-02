namespace Pizzashop.Entity.ViewModel;

public class UserListVM
{
    public Guid Id { get; set; }
    
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? ProfileImage { get; set; }

    public string Role {get; set;} = null!;

    public string Phone {get; set;} = null!;

    public bool? IsActive {get; set;}
}
