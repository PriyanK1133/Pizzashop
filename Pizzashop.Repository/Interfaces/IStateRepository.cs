using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IStateRepository
{
    Task<IEnumerable<State>> GetAllByCountryIdAsync(Guid countryId);
}
