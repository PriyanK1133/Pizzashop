using Pizzashop.Entity.ViewModel;

namespace Pizzashop.Service.Helper;

public static class PaginationHelper
{
    public static void SetPagination(
        this Pagination pagination,
        int totalRecords,
        int pageSize,
        int pageNumber
    )
    {
        if (totalRecords < 0)
            return;
            
        pagination.TotalRecords = totalRecords;
        pagination.PageSize = pageSize;
        
        pagination.FromRec = (pageNumber - 1) * pageSize;
        pagination.ToRec = pagination.FromRec + pageSize;

        if (pagination.ToRec > pagination.TotalRecords)
        {
            pagination.ToRec = pagination.TotalRecords;
        }

        pagination.FromRec += 1;
        if(pagination.FromRec>pagination.TotalRecords){
            pagination.FromRec = pagination.TotalRecords;
        }
        pagination.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        pagination.CurrentPage = pageNumber;
    }
}

