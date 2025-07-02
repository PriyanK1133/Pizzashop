using Pizzashop.Entity.Constants;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.Helper;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Helper;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    public readonly IEmailService _emailService;

    public UserService(IUserRepository userRepository, IAccountRepository accountRepository, IEmailService emailService)
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _emailService = emailService;
    }

    public async Task<Response<UserVM?>> GetByIdAsync(Guid id)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return Response<UserVM?>.FailureResponse("User Does not exist");
            }

            UserVM userVM = new()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                ProfileImage = user.ProfileImage,
                Address = user.Address,
                CityId = user.CityId,
                StateId = user.StateId,
                CountryId = user.CountryId,
                RoleId = user.RoleId,
                Zipcode = user.Zipcode,
                Phone = user.Phone,
                IsActive = user.IsActive,
                Role = user.Role.Name
            };

            return Response<UserVM?>.SuccessResponse(userVM, "User Fetched Successfully.");
        });
    }

    public async Task<Response<UserVM?>> UpdateAsync(UserVM model)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            var IsExist = await _userRepository.IsExistsAsync(u => u.UserName == model.UserName && u.Id != model.Id && !u.IsDeleted);
            if (IsExist)
            {
                return Response<UserVM?>.FailureResponse("UserName must be unique");
            }

            var user = await _userRepository.GetByIdAsync(model.Id);
            if (user == null)
            {
                return Response<UserVM?>.FailureResponse("User does not exist!");
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.UserName;
            user.ProfileImage = model.ProfileImage;
            user.Address = model.Address;
            user.CityId = model.CityId;
            user.StateId = model.StateId;
            user.CountryId = model.CountryId;
            user.Zipcode = model.Zipcode;
            user.Phone = model.Phone;

            if (!model.RoleId.Equals(Guid.Empty))
            {
                user.RoleId = model.RoleId;
                Account? account = await _accountRepository.GetByEmailAsync(user.Email);
                account!.RoleId = model.RoleId;
                await _accountRepository.UpdateAsync(account);
            }

            if (model.IsActive != null)
            {
                user.IsActive = model.IsActive;
            }

            await _userRepository.UpdateAsync(user);

            return Response<UserVM?>.SuccessResponse(model, "User Updated Successfully!");
        });
    }

    public async Task<Response<UserVM?>> AddAsync(UserVM model, Guid creatorId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            var IsExist = await _userRepository.IsExistsAsync(u => u.UserName == model.UserName && !u.IsDeleted);
            if (IsExist)
            {
                return Response<UserVM?>.FailureResponse("UserName " + MessageConstants.AlreadyExistMessage);
            }

            IsExist = await _userRepository.IsExistsAsync(u => u.Email == model.Email && u.IsDeleted);
            if (IsExist)
            {
                return Response<UserVM?>.FailureResponse("Email  " + MessageConstants.AlreadyExistMessage);
            }

            User newUser = new()
            {
                Id = Guid.NewGuid(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email,
                ProfileImage = model.ProfileImage,
                Address = model.Address,
                CityId = model.CityId,
                StateId = model.StateId,
                CountryId = model.CountryId,
                RoleId = model.RoleId,
                Zipcode = model.Zipcode,
                Phone = model.Phone,
                CreatedBy = creatorId,
                UpdatedBy = creatorId
            };

            Account account = new()
            {
                Email = newUser.Email,
                UserId = newUser.Id,
                RoleId = newUser.RoleId,
                Password = PasswordHelper.HashPassword(model.Password),
                CreatedBy = creatorId,
                UpdatedBy = creatorId
            };

            await _userRepository.AddAsync(newUser);
            await _accountRepository.AddAsync(account);

            string body = _emailService.GetCreateUserEmailBody(newUser.Email, model.Password);
            await _emailService.SendEmailAsync(newUser.Email, body, "Your Registration is Successfull! Here is your Credentials!");

            return Response<UserVM?>.SuccessResponse(model, "User " + MessageConstants.CreateMessage);
        });
    }

    public async Task<Response<bool>> UpdateProfileImageAsync(Guid id, string? profileImage)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            User? user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return Response<bool>.FailureResponse("User " + MessageConstants.NotFoundMessage);
            }

            user.ProfileImage = profileImage;
            user.UpdatedBy = id;
            user.UpdatedAt = DateTime.Now;

            await _userRepository.UpdateAsync(user);

            return Response<bool>.SuccessResponse(true, "User " + MessageConstants.EditMessage);
        });
    }

    public async Task<Response<bool>> DeleteAsync(Guid id)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return Response<bool>.FailureResponse("User " + MessageConstants.NotFoundMessage);
            }

            var account = await _accountRepository.GetByEmailAsync(user.Email);
            if (account != null)
            {
                await _accountRepository.DeleteAsync(account.Id);
            }

            user.IsDeleted = true;

            await _userRepository.UpdateAsync(user);
            return Response<bool>.SuccessResponse(true, "User " + MessageConstants.DeleteMessage);
        });
    }

    public async Task<Response<PagedResult<UserListVM>>> GetUsersAsync(string searchString, int page, int pagesize, string sortColumn, bool isASC)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            (IEnumerable<User> users, int totalRecords) = await _userRepository.GetPagedUsersAsync(searchString, page, pagesize, sortColumn, isASC);
            IEnumerable<UserListVM> userlist = users.Select(u => new UserListVM()
            {
                Id = u!.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                Email = u.Email,
                ProfileImage = u.ProfileImage,
                IsActive = u.IsActive,
                Role = u.Role.Name,
                Phone = u.Phone
            });

            PagedResult<UserListVM> pagedResult = new()
            {
                PagedList = userlist
            };

            pagedResult.Pagination.SetPagination(totalRecords, pagesize, page);

            return Response<PagedResult<UserListVM>>.SuccessResponse(pagedResult, "Users " + MessageConstants.GetMessage);
        });
    }
}
