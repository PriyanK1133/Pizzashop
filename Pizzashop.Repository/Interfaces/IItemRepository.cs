using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IItemRepository
{
    Task<Item?> GetByIdAsync(Guid id);
    Task<IEnumerable<Item>> GetAllItemsAsync();
    Task<IEnumerable<Item>> GetAvailableByCategoryIdAsync(Guid categoryId);
    Task<IEnumerable<Item>> GetAllAvailableAsync();
    Task<IEnumerable<Item>> GetFavoriteItemsAsync();
    Task<IEnumerable<Item>> GetAllByCategoryIdAsync(Guid categoryId);
    Task<(IEnumerable<Item> list, int totalRecords )> GetPagedItemsAsync(Guid categoryId, string searchString, int page, int pagesize, bool isASC);
    Task AddAsync(Item item);
    Task UpdateAsync(Item item);
    Task UpdateRangeAsync(IEnumerable<Item> items);
}
