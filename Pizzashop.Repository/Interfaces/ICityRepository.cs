using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface ICityRepository
{
    Task<IEnumerable<City>> GetAllByStateIdAsync(Guid stateId);
}
