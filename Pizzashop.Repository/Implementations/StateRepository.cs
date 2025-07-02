using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class StateRepository : IStateRepository
{
    private readonly PizzashopContext _context;

    public StateRepository(PizzashopContext context){
        _context = context;
    }

    public async Task<IEnumerable<State>> GetAllByCountryIdAsync(Guid countryId)
    {
        return await _context.States.Where(s => s.CountryId == countryId).ToListAsync();
    }

}
