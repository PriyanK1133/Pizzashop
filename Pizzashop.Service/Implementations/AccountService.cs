using Pizzashop.Entity.Data;
using Pizzashop.Service.Interfaces;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Utils;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.Helper;
using System.Text;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Pizzashop.Service.Implementations;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;

    public AccountService(IAccountRepository accountRepository, IJwtService jwtService, IEmailService emailService)
    {
        _accountRepository = accountRepository;
        _jwtService = jwtService;
        _emailService = emailService;
    }

    public async Task<Response<LoginResponse?>> AuthenticateUserAsync(LoginVM model)
    {
        Account? account = await _accountRepository.GetByEmailAsync(model.Email);

        if (account == null)
        {
            return Response<LoginResponse?>.FailureResponse("User " + MessageConstants.NotFoundMessage);
        }

        if (!account.User.IsActive.GetValueOrDefault())
        {
            return Response<LoginResponse?>.FailureResponse(MessageConstants.UserInactiveMessage);
        }

        if (!account.User.IsActive.GetValueOrDefault() || !PasswordHelper.Verify(model.Password, account.Password))
        {
            return Response<LoginResponse?>.FailureResponse(MessageConstants.InvalidCredentialsMessage);
        }

        // Generate JWT Access Token
        string accessToken = await _jwtService.GenerateAccessTokenAsync(account.Id);
        string? refreshToken = null;
        // Save User Data to Cookie for Remember Me functionality.
        if (model.RememberMe)
        {
            refreshToken = _jwtService.GenerateRefreshToken(account.Id);
        }
        
        // var jsonPayload = JsonSerializer.Serialize(new { email = model.Email });
        // var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        // var client = new HttpClient();
        // var response = await client.PostAsync("https://n8n-2-0qci.onrender.com/webhook/5f6caead-76c6-4cf0-ac92-7bfcfa0223a2", content);
    
        return Response<LoginResponse?>.SuccessResponse(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        }, MessageConstants.LoginSuccessMessage);
    }

    public async Task<Response<Account?>> ForgotPasswordAsync(LoginVM model, string baseUrl)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Account? account = await _accountRepository.GetByEmailAsync(model.Email);
            if (account == null)
            {
                return Response<Account?>.FailureResponse("User " + MessageConstants.NotFoundMessage);
            }

            Guid resetCode = Guid.NewGuid();

            string resetLink = $@"{baseUrl}/Account/ResetPassword/{account.Id}?token={resetCode}";
            account.Token = resetCode;
            account.TokenExpiry = DateTime.Now.AddMinutes(30);

            await _accountRepository.UpdateAsync(account);

            string subject = "Password Reset Request";
            string body = _emailService.GetResetPasswordEmailBody(resetLink);

            bool isSent = await _emailService.SendEmailAsync(account.Email, body, subject);

            return Response<Account?>.SuccessResponse(account, MessageConstants.ResetPasswordLinkSentMessage);
        });

    }

    public async Task<bool> IsTokenValidAsync(Guid id, Guid token)
    {
        try
        {
            Account? account = await _accountRepository.GetByIdAsync(id);
            if (
                account == null
                || account.Token != token
                || account.TokenExpiry < DateTime.Now)
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<Response<bool>> ResetPasswordAsync(ResetPasswordVM model)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Account? account = await _accountRepository.GetByIdAsync(model.Id);
            if (account == null)
            {
                return Response<bool>.FailureResponse("User " + MessageConstants.NotFoundMessage);
            }

            if (account == null
                || account.Token != model.Token
                || account.TokenExpiry < DateTime.Now)
            {
                return Response<bool>.FailureResponse(MessageConstants.InvalidTokenMessage);
            }

            account.Password = PasswordHelper.HashPassword(model.Password);
            account.Token = null;

            await _accountRepository.UpdateAsync(account);

            return Response<bool>.SuccessResponse(true, MessageConstants.PasswordResetMessage);

        });
    }

    public async Task<Response<bool>> ChangePasswordAsync(ChangePasswordVM model)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Account? account = await _accountRepository.GetByEmailAsync(model.Email!);
            if (account == null || !PasswordHelper.Verify(model.CurrentPassword, account.Password))
            {
                return Response<bool>.FailureResponse(MessageConstants.InvalidCurrentPasswordMessage);
            }

            if (model.NewPassword == model.CurrentPassword)
            {
                return Response<bool>.FailureResponse(MessageConstants.NewPasswordMustDifferentMessage);
            }

            account.Password = PasswordHelper.HashPassword(model.NewPassword);
            if (account.IsFirstLogin == true)
            {
                account.IsFirstLogin = false;
            }

            await _accountRepository.UpdateAsync(account);

            return Response<bool>.SuccessResponse(true, "Password " + MessageConstants.EditMessage);
        });
    }

}

