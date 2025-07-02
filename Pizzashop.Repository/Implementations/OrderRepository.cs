using System.Data;
using System.Linq.Expressions;
using System.Text.Json;
using Pizzashop.Entity.Constants;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.DTO;
using Pizzashop.Repository.Interfaces;
using System.Threading.Tasks;

namespace Pizzashop.Repository.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly PizzashopContext _context;
    public OrderRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Order> orders, int totalRecords)> GetOrdersAsync(string searchString, string status, DateOnly? fromDate, DateOnly? toDate)
    {
        IQueryable<Order> query = _context.Orders.Where(i => !i.IsDeleted).Include(i => i.CustomerVisitDetails).ThenInclude(c => c.Customer).Include(i => i.Ratings).Include(i => i.Payments);

        if (fromDate != null)
        {
            query = query.Where(i => i.OrderDate >= fromDate);
        }

        if (toDate != null)
        {
            query = query.Where(i => i.OrderDate <= toDate);
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower().Trim();
            query = query.Where(i => i.CustomerVisitDetails.Customer.Name.ToLower().Contains(searchString)
                                || i.Id.ToString().ToLower().Substring(i.Id.ToString().Length - 5).Contains(searchString));
        }

        if (!string.IsNullOrEmpty(status))
        {
            status = status.ToLower().Trim();
            query = query.Where(i => i.Status.ToLower().Contains(status));
        }

        query = query.OrderBy(i => i.Id.ToString().Substring(i.Id.ToString().Length - 5));

        return (await query.ToListAsync(), await query.CountAsync());
    }

    public async Task<IEnumerable<Order>> GetKOTOrdersAsync(Guid categoryId)
    {
        // IQueryable<Order> query = _context.Orders.Where(i => !i.IsDeleted)
        //                                         .Include(i => i.TableOrderMappings).ThenInclude(i => i.Table)
        //                                         .Include(i => i.CustomerVisitDetails.Section)
        //                                         .Include(i => i.OrderItems).ThenInclude(i => i.Item)
        //                                         .Include(i => i.OrderItems).ThenInclude(i => i.OrderModifiers).ThenInclude(i => i.Modifier)
        //                                         .Where(i => string.Concat(Constants.OrderInProgress, Constants.OrderRunning, Constants.OrderServed).Contains(i.Status));

        // if (categoryId != Guid.Empty)
        // {
        //     query = query.Include(i => i.OrderItems.Where(oi => oi.Item.CategoryId == categoryId));
        //     query = query.Where(i => i.OrderItems.Where(oi => oi.Item.CategoryId == categoryId).Count() > 0);
        // }

        // return await query.ToListAsync();

        string? result = _context.Database.SqlQueryRaw<string>($"select get_order_details_for_kot('{categoryId}')").AsEnumerable().FirstOrDefault();

        if (string.IsNullOrEmpty(result))
        {
            return new List<Order>();
        }

        List<Order> orders = JsonSerializer.Deserialize<List<Order>>(result) ?? new List<Order>();

        return orders;
    }

    public async Task<OrderSummaryDTO> GetOrderSummaryAsync(DateOnly? fromDate, DateOnly? toDate)
    {
        IQueryable<Order> query = _context.Orders.OrderBy(o => o.CreatedAt);

        if (fromDate != null)
        {
            query = query.Where(o => o.OrderDate >= fromDate);
        }

        if (toDate != null)
        {
            query = query.Where(o => o.OrderDate <= toDate);
        }

        OrderSummaryDTO orderSummaryDTO = new()
        {
            TotalOrders = await query.CountAsync(),
            TotalSales = await query.SumAsync(o => o.OrderTotal),
            AvgWaitingTime = TimeSpan.FromTicks((long)(await query
                .Where(o => o.OrderServedTime.HasValue)
                .Select(o => (o.OrderServedTime.GetValueOrDefault() - o.CreatedAt).Ticks).ToListAsync()).DefaultIfEmpty(0).Average())
        };

        orderSummaryDTO.AvgOrderValue = orderSummaryDTO.TotalSales / (orderSummaryDTO.TotalOrders > 0 ? orderSummaryDTO.TotalOrders : 1);

        return orderSummaryDTO;
    }

    public async Task<(IEnumerable<GraphDataDTO> revenueData, IEnumerable<GraphDataDTO> customerGrowthData)> GetGraphDataAsync(DateOnly? fromDate, DateOnly? toDate)
    {
        IQueryable<Order> query = _context.Orders.OrderBy(o => o.CreatedAt);


        if (fromDate != null)
        {
            query = query.Where(o => o.OrderDate >= fromDate);
        }

        if (toDate != null)
        {
            query = query.Where(o => o.OrderDate <= toDate);
        }


        DateTime firstDate = fromDate?.ToDateTime(TimeOnly.MinValue) ?? (await query.FirstOrDefaultAsync())?.CreatedAt ?? DateTime.MinValue;
        DateTime lastDate = toDate?.ToDateTime(TimeOnly.MaxValue) ?? (await query.LastOrDefaultAsync())?.CreatedAt ?? DateTime.MinValue;

        int dataPoints;


        if (firstDate.AddDays(1) >= lastDate)
        {
            dataPoints = 24;
        }
        else if (firstDate.AddDays(31) >= lastDate)
        {
            dataPoints = lastDate.Subtract(firstDate).Days + 1;
        }
        else
        {
            dataPoints = 5;
        }

        TimeSpan interval = (lastDate - firstDate) / dataPoints;
        List<GraphDataDTO> revenueData = new();
        List<GraphDataDTO> customerGrowthData = new();

        for (int i = 0; i < dataPoints; i++)
        {
            DateTime intervalStart = firstDate.Add(interval * i);
            DateTime intervalEnd = intervalStart.Add(interval);
            IQueryable<Order> segmentQuery = query.Where(o => o.CreatedAt >= intervalStart && o.CreatedAt < intervalEnd);

            decimal revenue = await segmentQuery.SumAsync(o => o.OrderTotal);

            decimal customerCount = await segmentQuery.GroupBy(o => o.CustomerVisitDetails.CustomerId).CountAsync();

            string date;

            if (firstDate.AddDays(1) >= lastDate)
            {
                date = intervalStart.ToString("hh:mm");
            }
            else if (firstDate.AddDays(31) >= lastDate)
            {
                date = intervalStart.ToString("dd MMM yyyy");
            }
            else
            {
                date = intervalStart.ToString("dd MMM yyyy") + " - " + intervalEnd.ToString("dd MMM yyyy");
            }

            revenueData.Add(new GraphDataDTO
            {
                Date = date,
                Value = revenue
            });

            customerGrowthData.Add(new GraphDataDTO
            {
                Date = date,
                Value = customerCount
            });

        }

        return (revenueData, customerGrowthData);

    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _context.Orders
                    .Include(o => o.Payments)
                    .Include(o => o.TableOrderMappings).ThenInclude(t => t.Table)
                    .Include(o => o.CustomerVisitDetails).ThenInclude(c => c.Customer)
                    .Include(o => o.CustomerVisitDetails).ThenInclude(c => c.Section)
                    .Include(o => o.OrderItems.OrderBy(oi => oi.CreatedAt)).ThenInclude(i => i.Item)
                    .Include(o => o.OrderItems).ThenInclude(i => i.OrderModifiers).ThenInclude(m => m.Modifier)
                    .Include(o => o.OrderTaxes)
                    .SingleOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
    }

    public async Task<(IEnumerable<Order> list, int totalRecords)> GetPagedListAsync(string searchString, int page, int pagesize, string sortColumn, bool isASC, string status, DateOnly? fromDate, DateOnly? toDate)
    {
        IQueryable<Order> query = _context.Orders.Where(i => !i.IsDeleted).Include(i => i.CustomerVisitDetails).ThenInclude(c => c.Customer).Include(i => i.Ratings).Include(i => i.Payments);

        if (fromDate != null)
        {
            query = query.Where(i => i.OrderDate >= fromDate);
        }

        if (toDate != null)
        {
            query = query.Where(i => i.OrderDate <= toDate);
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower().Trim();
            query = query.Where(i => i.CustomerVisitDetails.Customer.Name.ToLower().Contains(searchString)
                                || i.Id.ToString().ToLower().Substring(i.Id.ToString().Length - 5).Contains(searchString));
        }

        if (!string.IsNullOrEmpty(status))
        {
            status = status.ToLower().Trim();
            query = query.Where(i => i.Status.ToLower().Trim().Contains(status));
        }

        if (isASC)
        {
            query = sortColumn.ToLower() switch
            {
                "customer" => query.OrderBy(i => i.CustomerVisitDetails.Customer.Name),
                "totalamount" => query.OrderBy(i => i.OrderTotal),
                "date" => query.OrderBy(i => i.OrderDate),
                _ => query.OrderBy(i => i.Id.ToString().Substring(i.Id.ToString().Length - 5)),
            };
        }
        else
        {
            query = sortColumn.ToLower() switch
            {
                "customer" => query.OrderByDescending(i => i.CustomerVisitDetails.Customer.Name),
                "totalamount" => query.OrderByDescending(i => i.OrderTotal),
                "date" => query.OrderByDescending(i => i.OrderDate),
                _ => query.OrderByDescending(i => i.Id.ToString().Substring(i.Id.ToString().Length - 5)),
            };

        }

        return (await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync(), await query.CountAsync());
    }

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsExistsAsync(Expression<Func<Order, bool>> predicate)
    {
        return await _context.Orders.AnyAsync(predicate);
    }

    public async Task UpdateKOTItemsAsync(Guid orderId, List<string> orderItemIds, string status, List<int> quantities)
    {


        var orderItemIdsParam = new Npgsql.NpgsqlParameter("@order_item_ids", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text)
        {
            Value = orderItemIds.ToArray()
        };
        var orderIdParam = new Npgsql.NpgsqlParameter("@order_id", orderId);
        var statusParam = new Npgsql.NpgsqlParameter("@status", status);
        var quantitiesParam = new Npgsql.NpgsqlParameter("@quantities", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer)
        {
            Value = quantities.ToArray()
        };

        var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
           "call update_order_items_status(@order_id, @order_item_ids, @status, @quantities)", orderIdParam, orderItemIdsParam, statusParam, quantitiesParam
        );
    }
}
