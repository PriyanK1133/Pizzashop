using Pizzashop.Entity.Constants;
using Pizzashop.Entity.DTO;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerVisitDetailRepository _customerVisitDetailRepository;

    public DashboardService(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, ICustomerRepository customerRepository, ICustomerVisitDetailRepository customerVisitDetailRepository){
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _customerRepository = customerRepository;
        _customerVisitDetailRepository = customerVisitDetailRepository;
    }

    public async Task<Response<DashboardVM?>> GetDashboardDataAsync(DateOnly? fromDate, DateOnly? toDate)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async() => {
            OrderSummaryDTO orderSummary = await _orderRepository.GetOrderSummaryAsync(fromDate, toDate);
            (IEnumerable<ItemSummaryDTO> topSellingItems, IEnumerable<ItemSummaryDTO> leastSellingItems) = await _orderItemRepository.GetItemsSummary(fromDate, toDate);
            int newCustomersCount = await _customerRepository.GetNewCustomerCountAsync(fromDate, toDate);
            int waitingCustomerCount = await _customerVisitDetailRepository.GetWaitingCustomerCountAsync();

            DashboardVM dashboardVM = new(){
                TotalOrder = orderSummary.TotalOrders,
                TotalSales = orderSummary.TotalSales,
                AvgOrderValue = orderSummary.AvgOrderValue,
                NewCustomerCount = newCustomersCount,
                WaitingListCount = waitingCustomerCount,
                AvgWaitingTime = orderSummary.AvgWaitingTime,

                LeastSellingItems = leastSellingItems.Select(i => new ItemSummaryVM(){
                    Id = i.Id,
                    Name = i.Name,
                    Image = i.Image,
                    OrderCount = i.OrderCount
                }).ToList(),

                TopSellingItems = topSellingItems.Select(i => new ItemSummaryVM(){
                    Id = i.Id,
                    Name = i.Name,
                    Image = i.Image,
                    OrderCount = i.OrderCount
                }).ToList(),
            };

            return Response<DashboardVM?>.SuccessResponse(dashboardVM,"Dashboard Data " + MessageConstants.GetMessage);
        });
    }

    public async Task<Response<GraphDataVM?>> GetGraphDataAsync(DateOnly? fromDate, DateOnly? toDate){
        return await ExceptionHandler.HandleExceptionsAsync(async() => {
            (IEnumerable<GraphDataDTO> revenueData, IEnumerable<GraphDataDTO> customerGrowthData) = await _orderRepository.GetGraphDataAsync(fromDate, toDate);

            GraphDataVM graphData = new(){
                RevenueData = revenueData.Select(r => new GraphPointVM(){
                    Date = r.Date,
                    Value = r.Value,
                }).ToList(),
                CustomerGrowthData = customerGrowthData.Select(cg => new GraphPointVM(){
                    Date = cg.Date,
                    Value = cg.Value
                }).ToList()
            };

            return Response<GraphDataVM?>.SuccessResponse(graphData, "Graph Data " + MessageConstants.GetMessage);
        });
    }

}
