using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;
public interface IAccountService
{
    Task<Response<LoginResponse?>> AuthenticateUserAsync(LoginVM model);
    Task<Response<Account?>> ForgotPasswordAsync(LoginVM model, string baseUrl);
    Task<bool> IsTokenValidAsync(Guid id, Guid token);
    Task<Response<bool>> ResetPasswordAsync(ResetPasswordVM model);
    Task<Response<bool>> ChangePasswordAsync(ChangePasswordVM model);
}

