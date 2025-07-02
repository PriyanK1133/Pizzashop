using ClosedXML.Excel;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Helper;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class CustomersService : ICustomersService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomersService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    #region Private Helper Method
    private static decimal GetAverageBill(Customer customer)
    {
        return Math.Round(customer.CustomerVisitDetails.Average(cv => cv.Orders.Average(o => (decimal?)o.OrderTotal)).GetValueOrDefault(), 2);
    }

    private static decimal GetMaxOrder(Customer customer)
    {
        return customer.CustomerVisitDetails.Max(cv => cv.Orders.Max(o => (decimal?)o.OrderTotal)).GetValueOrDefault();
    }

    private static string GetPaymentMode(Order order)
    {
        return order.Payments.OrderByDescending(o => o.CreatedAt).FirstOrDefault()?.PaymentMode ?? "Pending";
    }

    private static MemoryStream CreateCustomerExcel(IEnumerable<CustomerVM> customerList, string searchString, DateOnly? fromDate, DateOnly? toDate, string account)
    {
        var rootpath = Directory.GetCurrentDirectory();

        var path = Path.Combine(rootpath, "wwwroot/Template/CustomerTemplate.xlsx");

        using var wb = new XLWorkbook(path);
        IXLWorksheet worksheet = wb.Worksheet("Customers");

        worksheet.Cell(2, 3).Value = account;
        worksheet.Cell(2, 10).Value = searchString;
        if (fromDate != null && toDate != null)
        {
            worksheet.Cell(5, 3).Value = fromDate.ToString() + " to " + toDate.ToString();
        }
        else
        {
            worksheet.Cell(5, 3).Value = "All Time";
        }
        worksheet.Cell(5, 10).Value = customerList.Count();

        var row = 10;
        foreach (CustomerVM c in customerList)
        {
            int col = 1;
            worksheet.Cell(row, col).Value = string.Concat("#", c.Id.ToString().ToUpper()[^5..]);
            worksheet.Cell(row, col++).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            worksheet.Cell(row, col).Value = c.Name;
            worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 2)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            worksheet.Cell(row, ++col).Value = c.Email;
            worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 3)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            worksheet.Cell(row, ++col).Value = c.Date.ToString();
            worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 2)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            worksheet.Cell(row, ++col).Value = c.Phone;
            worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 2)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            worksheet.Cell(row, ++col).Value = c.TotalOrder;
            worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 1)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            row++;
        }

        MemoryStream stream = new();
        wb.SaveAs(stream);
        stream.Position = 0;

        return stream;
    }

    #endregion Private Helper Method

    #region Fetch Customer History

    public async Task<Response<CustomerHistoryVM?>> GetByIdAsync(Guid id)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Customer? customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
            {
                return Response<CustomerHistoryVM?>.FailureResponse("Customer " + MessageConstants.NotFoundMessage);
            }

            CustomerHistoryVM model = new()
            {
                Id = customer.Id,
                Name = customer.Name,
                AverageBill = GetAverageBill(customer),
                MobileNumber = customer.Mobile.ToString(),
                ComingSince = customer.CreatedAt,
                MaxOrder = GetMaxOrder(customer),
                Visits = customer.CustomerVisitDetails.Count,

                CustomerOrders = customer.CustomerVisitDetails.SelectMany(cv => cv.Orders.Select(o => new CustomerOrder()
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    OrderType = "dinein", // TODO
                    Payment = GetPaymentMode(o),
                    Items = o.OrderItems.Count,
                    Amount = o.OrderTotal
                }).ToList()).ToList()
            };

            return Response<CustomerHistoryVM?>.SuccessResponse(model, "Customer History " + MessageConstants.GetMessage);

        });
    }

    #endregion Fetch Customer History

    #region Paged Customers List
    public async Task<Response<PagedResult<CustomerVM>>> GetPagedListAsync(string searchString, int page, int pagesize, string sortColumn, bool isASC, DateOnly? fromDate, DateOnly? toDate)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            (IEnumerable<Customer> customers, int totalRecords) = await _customerRepository.GetPagedListAsync(searchString, page, pagesize, sortColumn, isASC, fromDate, toDate);

            IEnumerable<CustomerVM> customerlist = customers.Select(c => new CustomerVM()
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Mobile.ToString(),
                Date = DateOnly.FromDateTime(c.CreatedAt),
                TotalOrder = c.CustomerVisitDetails.Sum(cv => cv.Orders.Count)
            }).ToList();

            PagedResult<CustomerVM> pagedResult = new()
            {
                PagedList = customerlist
            };

            pagedResult.Pagination.SetPagination(totalRecords, pagesize, page);

            return Response<PagedResult<CustomerVM>>.SuccessResponse(pagedResult, "Customer list " + MessageConstants.GetMessage);
        });

    }
    #endregion Paged Customer List

    #region Export Customers 
    public async Task<Response<MemoryStream>> ExportCustomersAsync(string searchString, DateOnly? fromDate, DateOnly? toDate, string account)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
     {
         IEnumerable<Customer> customers = await _customerRepository.GetCustomersAsync(searchString, fromDate, toDate);

         IEnumerable<CustomerVM> customerList = customers.Select(c => new CustomerVM()
         {
             Id = c.Id,
             Name = c.Name,
             Email = c.Email,
             Phone = c.Mobile.ToString(),
             Date = DateOnly.FromDateTime(c.CreatedAt),
             TotalOrder = c.CustomerVisitDetails.Sum(cv => cv.Orders.Count)
         }).ToList();

         MemoryStream stream = CreateCustomerExcel(customerList, searchString, fromDate, toDate, account);

         return Response<MemoryStream>.SuccessResponse(stream, "Customer Data " + MessageConstants.ExportSuccessMessage);

     });
    }

    #endregion Export Customers

}
