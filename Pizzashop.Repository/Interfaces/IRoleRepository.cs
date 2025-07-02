using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAllAsync();
}
