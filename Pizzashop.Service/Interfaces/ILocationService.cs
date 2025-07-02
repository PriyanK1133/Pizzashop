using Pizzashop.Entity.ViewModel;

namespace Pizzashop.Service.Interfaces;

public interface ILocationService
{
    Task<IEnumerable<CountryVM>> GetAllCountriesAsync();
    Task<IEnumerable<StateVM>> GetStatesByCountryIdAsync(Guid countryId);
    Task<IEnumerable<CityVM>> GetCitiesByStateIdAsync(Guid stateId);
}
