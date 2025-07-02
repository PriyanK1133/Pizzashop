namespace Pizzashop.Entity.ViewModel;

public class PagedResult<T>
{
    public IEnumerable<T> PagedList { get; set; } = new List<T>();

    public Pagination Pagination {get; set;} = new();
}

public class Pagination{
    public int CurrentPage { get; set; } = 1;

    public int TotalPages { get; set; } = 0;

    public int TotalRecords { get; set; } = 0;

    public int PageSize {get; set;}

    public int FromRec { get; set; }

    public int ToRec { get; set; }
}
