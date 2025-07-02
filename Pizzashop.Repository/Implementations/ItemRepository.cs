using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class ItemRepository : IItemRepository
{
    private readonly PizzashopContext _context;

    public ItemRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Item>> GetAllByCategoryIdAsync(Guid categoryId)
    {
        return await _context.Items.Where(i => i.CategoryId == categoryId && !i.IsDeleted).OrderByDescending(i => i.UpdatedAt).ToListAsync();
    }

    public async Task<IEnumerable<Item>> GetAvailableByCategoryIdAsync(Guid categoryId)
    {
        return await _context.Items.Where(i => i.CategoryId == categoryId && !i.IsDeleted && i.IsAvailable).OrderByDescending(i => i.UpdatedAt).ToListAsync();
    }

    public async Task<IEnumerable<Item>> GetAllAvailableAsync()
    {
        return await _context.Items.Where(i => !i.IsDeleted && i.IsAvailable).OrderByDescending(i => i.UpdatedAt).ToListAsync();
    }

    public async Task<IEnumerable<Item>> GetFavoriteItemsAsync()
    {
        return await _context.Items.Where(i => !i.IsDeleted && i.IsFavourite && i.IsAvailable).OrderByDescending(i => i.UpdatedAt).ToListAsync();
    }

    public async Task<IEnumerable<Item>> GetAllItemsAsync()
    {
        return await _context.Items.Where(i => !i.IsDeleted).OrderByDescending(i => i.UpdatedAt).ToListAsync();
    }

    public async Task<(IEnumerable<Item> list, int totalRecords)> GetPagedItemsAsync(Guid categoryId, string searchString, int page, int pagesize, bool isASC)
    {
        IQueryable<Item> query = _context.Items.Where(i => i.CategoryId == categoryId && !i.IsDeleted);
        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower().Trim();
            query = query.Where(i => i.Name.ToLower().Contains(searchString));
        }

        if (!isASC)
        {
            query = query.OrderBy(i => i.UpdatedAt);
        }
        else
        {
            query = query.OrderByDescending(i => i.UpdatedAt);
        }

        return (await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync(), await query.CountAsync());
    }

    public async Task<Item?> GetByIdAsync(Guid id)
    {
        return await _context.Items.SingleOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
    }

    public async Task AddAsync(Item item)
    {
        _context.Items.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Item item)
    {
        _context.Items.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<Item> items)
    {
        _context.Items.UpdateRange(items);
        await _context.SaveChangesAsync();
    }

}
