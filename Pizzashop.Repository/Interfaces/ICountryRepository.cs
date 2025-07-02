using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface ICountryRepository
{
    Task<IEnumerable<Country>> GetAllAsync();
}
