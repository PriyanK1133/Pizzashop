using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface ITableAndSectionService
{
    Task<Response<SectionVM?>> GetSectionByIdAsync(Guid id);
    Task<Response<IEnumerable<SectionVM>>> GetAllSectionsAsync();
    Task<Response<SectionVM?>> AddSectionAsync(SectionVM model);
    Task<Response<SectionVM?>> EditSectionAsync(SectionVM model);
    Task<Response<bool>> DeleteSectionAsync(Guid id, Guid deltorId);
    Task<Response<bool>> ChangeSectionOrder(List<Guid> sortedIds);

    Task<Response<IEnumerable<TableVM>>> GetTablesBySectionIdAsync(Guid sectionId);
    Task<Response<TableAndSectionVM?>> GetPagedTablesAsync(Guid sectionId, string searchString, int page, int pagesize, bool isASC);
    Task<Response<TableVM?>> GetTableByIdAsync(Guid id);
    Task<Response<TableVM?>> AddTableAsync(TableVM model);
    Task<Response<TableVM?>> EditTableAsync(TableVM model);
    Task<Response<bool>> DeleteTableAsync(Guid id, Guid deletorId);
    Task<Response<bool>> DeleteManyTableAsync(IEnumerable<Guid> ids, Guid deletorId);
}
