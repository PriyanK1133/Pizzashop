namespace Pizzashop.Entity.Helper;

public class PasswordHelper
{
    public static string HashPassword(string password){
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool Verify(string passwordToVerify, string passwordHash){
        return BCrypt.Net.BCrypt.Verify(passwordToVerify,passwordHash);
    }
}
