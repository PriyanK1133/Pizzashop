using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;

namespace Pizzashop.Service.Implementations;

public class LocationService : ILocationService
{
    private readonly ICountryRepository _countryRepository;
    private readonly IStateRepository _stateRepository;
    private readonly ICityRepository _cityRepository;

    public LocationService(ICountryRepository countryRepository, IStateRepository stateRepository, ICityRepository cityRepository){
        _countryRepository = countryRepository;
        _stateRepository = stateRepository;
        _cityRepository = cityRepository;
    }

    public async Task<IEnumerable<CountryVM>> GetAllCountriesAsync()
    {
        IEnumerable<Country> countries = await _countryRepository.GetAllAsync();
        return countries.Select(c => new CountryVM{
            Id = c.Id,
            Name = c.Name
        });
    }

    public async Task<IEnumerable<CityVM>> GetCitiesByStateIdAsync(Guid stateId)
    {
        IEnumerable<City> cities = await _cityRepository.GetAllByStateIdAsync(stateId);
        return cities.Select(c => new CityVM{
            Id = c.Id,
            Name = c.Name
        });
    }

    public async Task<IEnumerable<StateVM>> GetStatesByCountryIdAsync(Guid countryId)
    {
        IEnumerable<State> states = await _stateRepository.GetAllByCountryIdAsync(countryId);
        return states.Select(s => new StateVM{
            Id = s.Id,
            Name = s.Name
        });
    }
}
