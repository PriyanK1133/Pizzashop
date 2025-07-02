using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface IUserService
{
    Task<Response<UserVM?>> GetByIdAsync(Guid id);
    Task<Response<UserVM?>> UpdateAsync(UserVM model);
    Task<Response<bool>> UpdateProfileImageAsync(Guid id, string? profileImage);
    Task<Response<UserVM?>> AddAsync(UserVM model, Guid creatorId);
    Task<Response<bool>> DeleteAsync(Guid id);

    Task<Response<PagedResult<UserListVM>>> GetUsersAsync(string searchString, int page, int pagesize, string sortColumn, bool isASC);
    
}
