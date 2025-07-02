using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Helper;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;
using ClosedXML.Excel;
using Pizzashop.Entity.Constants;

namespace Pizzashop.Service.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ITaxAndFeeRepository _taxAndFeeRepository;

    public OrderService(IOrderRepository orderRepository, ITaxAndFeeRepository taxAndFeeRepository)
    {
        _orderRepository = orderRepository;
        _taxAndFeeRepository = taxAndFeeRepository;
    }

    private async Task<List<OrderTaxVM>> GetOrderTaxes(IEnumerable<OrderTaxis> orderTaxes)
    {
        if (orderTaxes == null || !orderTaxes.Any())
        {
            IEnumerable<TaxesAndFee> enabledTaxes = await _taxAndFeeRepository.GetEnabledAsync();

            return enabledTaxes.Select(t => new OrderTaxVM()
            {
                Id = t.Id,
                Name = t.Name,
                TotalTax = 0,
                Type = t.Type,
                Rate = t.TaxAmount
            }).ToList();
        }

        return orderTaxes.Select(t => new OrderTaxVM()
        {
            Id = t.TaxId,
            Name = t.TaxName,
            TotalTax = t.TotalTax,
            Type = t.TaxType,
            Rate = t.TaxValue
        }).ToList();
    }

    public async Task<Response<OrderDetailsVM?>> GetByIdAsync(Guid id)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Order? order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return Response<OrderDetailsVM?>.FailureResponse("Order " + MessageConstants.NotFoundMessage);
            }

            OrderDetailsVM orderDetailsVM = new()
            {
                Id = order.Id,
                InvoiceNumber = order.InvoiceNumber,
                Status = order.Status,
                PlacedOn = order.CreatedAt,
                ModifiedOn = order.UpdatedAt,
                OrderDuration = order.UpdatedAt.Subtract(order.CreatedAt),

                CustomerDetails = new CustomerDetailsVM()
                {
                    Id = order.CustomerVisitDetails.Id,
                    Name = order.CustomerVisitDetails.Customer.Name,
                    Phone = order.CustomerVisitDetails.Customer.Mobile.ToString(),
                    NumberOfPerson = order.CustomerVisitDetails.NoOfPersons,
                    Email = order.CustomerVisitDetails.Customer.Email,
                    CustomerId = order.CustomerVisitDetails.CustomerId,
                    SectionId = order.CustomerVisitDetails.SectionId,
                    IsWaiting = order.CustomerVisitDetails.IsWaiting,
                    TableCapacity = order.TableOrderMappings.Sum(ot => ot.Table.Capacity)
                },

                TableNames = order.TableOrderMappings.Select(t => t.Table.Name).ToList(),
                SectionName = order.CustomerVisitDetails.Section.Name,

                OrderItems = order.OrderItems.Select(i => new OrderItemVM()
                {
                    Id = i.Id,
                    ItemId = i.Item.Id,
                    Name = i.ItemName,
                    Quantity = i.Quantity,
                    Price = i.ItemRate,
                    TotalAmount = i.ItemTotal,
                    TotalModifierAmount = i.TotalModifierAmount,
                    TaxPercentage = i.Tax,

                    OrderModifiers = i.OrderModifiers.Select(m => new OrderModifierVM()
                    {
                        Id = m.Id,
                        ModifierId = m.ModifierId,
                        Name = m.ModifierName,
                        Quantity = m.Quantity,
                        Price = m.ModifierRate,
                    }).ToList(),
                }).ToList(),

                SubTotal = order.Subtotal,
                OrderTaxes = await GetOrderTaxes(order.OrderTaxes),
                PaymentMethod = order.Payments.FirstOrDefault()?.PaymentMode ?? "Pending",
                TotalAmount = order.OrderTotal,
                Comment = order.Comment
            };

            return Response<OrderDetailsVM?>.SuccessResponse(orderDetailsVM, "Order Details " + MessageConstants.GetMessage);
        });
    }

    public async Task<Response<PagedResult<OrderListVM>>> GetPagedListAsync(string searchString, int page, int pagesize, string sortColumn, bool isASC, string status, DateOnly? fromDate, DateOnly? toDate)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
     {
         (IEnumerable<Order> orders, int totalRecords) = await _orderRepository.GetPagedListAsync(searchString, page, pagesize, sortColumn, isASC, status, fromDate, toDate);

         IEnumerable<OrderListVM> orderLists = orders.Select(order => new OrderListVM()
         {
             Id = order.Id,
             OrderDate = order.OrderDate,
             CustomerName = order.CustomerVisitDetails.Customer.Name,
             Status = order.Status,
             PaymentMode = order.Payments?.FirstOrDefault()?.PaymentMode ?? "Pending",
             Rating = (int)Math.Round(((order.Ratings.FirstOrDefault()?.AmbienceRating ?? 0) + (order.Ratings.FirstOrDefault()?.ServiceRating ?? 0) + (order.Ratings.FirstOrDefault()?.FoodRating ?? 0)) / 3),
             OrderTotal = order.OrderTotal
         }).ToList();

         PagedResult<OrderListVM> pagedResult = new()
         {
             PagedList = orderLists
         };

         pagedResult.Pagination.SetPagination(totalRecords, pagesize, page);

         return Response<PagedResult<OrderListVM>>.SuccessResponse(pagedResult, "Orders list " + MessageConstants.GetMessage);
     });
    }

    public async Task<Response<MemoryStream>> ExportOrdersAsync(string searchString, string status, DateOnly? fromDate, DateOnly? toDate)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
     {
         (IEnumerable<Order> orders, int totalRecords) = await _orderRepository.GetOrdersAsync(searchString, status, fromDate, toDate);

         IEnumerable<OrderListVM> orderLists = orders.Select(order => new OrderListVM()
         {
             Id = order.Id,
             OrderDate = order.OrderDate,
             CustomerName = order.CustomerVisitDetails.Customer.Name,
             Status = order.Status,
             PaymentMode = order.Payments.FirstOrDefault()?.PaymentMode ?? "Pending",
             Rating = (int)Math.Round(((order.Ratings.FirstOrDefault()?.AmbienceRating ?? 0) + (order.Ratings.FirstOrDefault()?.ServiceRating ?? 0) + (order.Ratings.FirstOrDefault()?.FoodRating ?? 0)) / 3),
             OrderTotal = order.OrderTotal
         }).ToList();

         var rootpath = Directory.GetCurrentDirectory();

         var path = Path.Combine(rootpath, "wwwroot/Template/OrderTemplate.xlsx");

         using var wb = new XLWorkbook(path);
         IXLWorksheet worksheet = wb.Worksheet("Orders");

         worksheet.Cell(2, 3).Value = status;
         worksheet.Cell(2, 10).Value = searchString;
         if (fromDate != null && toDate != null)
         {
             worksheet.Cell(5, 3).Value = fromDate.ToString() + " to " + toDate.ToString();
         }
         else
         {
             worksheet.Cell(5, 3).Value = "All Time";
         }
         worksheet.Cell(5, 10).Value = totalRecords;

         var row = 10;
         foreach (OrderListVM order in orderLists)
         {
             int col = 1;
             worksheet.Cell(row, col).Value = string.Concat("#", order.Id.ToString().ToUpper()[^5..]);
             worksheet.Cell(row, col++).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

             worksheet.Cell(row, col).Value = order.OrderDate.ToString();
             worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 2)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

             worksheet.Cell(row, ++col).Value = order.CustomerName;
             worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 2)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

             worksheet.Cell(row, ++col).Value = order.Status;
             worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 2)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

             worksheet.Cell(row, ++col).Value = order.PaymentMode;
             worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 1)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

             worksheet.Cell(row, ++col).Value = order.Rating;
             worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 1)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

             worksheet.Cell(row, ++col).Value = "Rs. " + order.OrderTotal;
             worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 1)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

             row++;
         }

         MemoryStream stream = new();
         wb.SaveAs(stream);
         stream.Position = 0;

         return Response<MemoryStream>.SuccessResponse(stream, "Orders " + MessageConstants.ExportSuccessMessage);

     });
    }

}
