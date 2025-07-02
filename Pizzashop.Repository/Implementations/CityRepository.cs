using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class CityRepository : ICityRepository
{
    private readonly PizzashopContext _context;

    public CityRepository(PizzashopContext context){
        _context = context;
    }

    public async Task<IEnumerable<City>> GetAllByStateIdAsync(Guid stateId)
    {
        return await _context.Cities.Where(c => c.StateId == stateId).ToListAsync();
    }

}
