using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IUnitRepository
{
    Task<IEnumerable<Unit>> GetAllAsync();
}
