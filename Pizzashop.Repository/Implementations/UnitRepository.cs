using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class UnitRepository : IUnitRepository
{
    private readonly PizzashopContext _context;

    public UnitRepository(PizzashopContext context){
        _context =context;
    }

    public async Task<IEnumerable<Unit>> GetAllAsync(){
        return await _context.Units.ToListAsync();
    }
}
