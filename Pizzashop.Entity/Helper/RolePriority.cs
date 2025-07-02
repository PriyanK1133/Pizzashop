
namespace Pizzashop.Entity.Helper;
using Pizzashop.Entity.Constants;

public class RolePriority
{

    public static int GetRolePriority(string? role)
    {
        return role switch
        {
            Constants.SuperAdmin => 4,
            Constants.Admin => 3,
            Constants.AccountManager => 2,
            Constants.Chef => 1,
            _ => 0,
        };

    }
}
