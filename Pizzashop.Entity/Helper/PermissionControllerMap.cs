namespace Pizzashop.Entity.Helper;

public static class PermissionControllerMap
{
    public static Dictionary<string, string> GetControllers(List<string> permissions)
    {
        Dictionary<string, string> controllers = new()
        {
            { "Dashboard", "dashboard.svg" }
        };

        foreach (string permissionName in permissions)
        {
            switch (permissionName)
            {
                case "Users":
                    controllers.Add("Users", "users.svg");
                    break;
                case "RoleAndPermission":
                    controllers.Add("Role And Permission", "role-and-permissions.svg");
                    break;
                case "Menu":
                    controllers.Add("Menu", "menu.svg");
                    break;
                case "TableAndSection":
                    controllers.Add("Table And Section", "table-and-session.svg");
                    break;
                case "TaxesAndFees":
                    controllers.Add("Taxes And Fees", "tax-and-fees.svg");
                    break;
                case "Orders":
                    controllers.Add("Orders", "orders.svg");
                    break;
                case "Customers":
                    controllers.Add("Customers", "customer-user.svg");
                    break;
            }
        }

        return controllers;
    }
}
