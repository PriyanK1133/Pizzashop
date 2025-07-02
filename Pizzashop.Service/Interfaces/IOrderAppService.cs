using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface IOrderAppService
{
    #region KOT
    Task<Response<IEnumerable<KOTOrderVM>>> GetKOTOrdersAsync(string status, Guid categoryId);
    Task<Response<bool>> UpdateOrderItemStatusAsync(Guid orderId, string status, List<OrderItemVM> updatedOrderItems);
    #endregion KOT

    #region Tables
    Task<Response<IEnumerable<OrderTableVM>>> GetTableDetailsBySectionIdAsync(Guid sectionId);
    Task<Response<IEnumerable<SectionVM>>> GetAllSectionsAsync();
    Task<Response<IEnumerable<OrderTableVM>>> GetAvailableTablesBySectionIdAsync(Guid sectionId);
    Task<Response<Guid?>> AssignTablesAsync(CustomerDetailsVM model, List<Guid> tables);
    Task<Response<Guid?>> AssignTablesToWaitingCustomerAsync(Guid waitingTokenId, Guid sectionId, List<Guid> tableIds, Guid updatorId);
    #endregion Tables

    #region WaitingList
    Task<Response<CustomerDetailsVM?>> AddCustomerDetailsAsync(CustomerDetailsVM model);
    Task<Response<CustomerDetailsVM?>> UpdateCustomerDetailsAsync(CustomerDetailsVM model);
    Task<Response<IEnumerable<CustomerDetailsVM>>> GetWaitingListAsync(Guid SectionId);
    Task<Response<CustomerDetailsVM?>> GetCustomerDetailsByEmailAsync(string email);
    Task<Response<bool>> DeleteWaitingToken(Guid id);
    #endregion WaitingList

    #region Menu
    Task<Response<string?>> EditOrderCommentAsync(Guid id, string? comment, Guid updatorId);
    Task<Response<string?>> SaveOrderItemInstructionAsync(Guid id, string? specialInstruction, Guid updatorId);
    Task<Response<string?>> GetOrderItemInstructionAsync(Guid id);
    Task<Response<bool>> SaveOrderAsync(OrderDetailsVM model);
    Task<Response<bool>> CompleteOrderAsync(Guid orderId, string paymentMethod, Guid updatorId);
    Task<Response<bool>> IsItemQuantityPrepared(Guid orderItemId, int quantity);
    Task<Response<bool>> IsOrderServedAsync(Guid orderId);
    Task<Response<bool>> SaveRatingAsync(RatingVM model);
    Task<Response<bool>> CancelOrderAsync(Guid orderId, Guid updatorId);
    #endregion Menu

}
