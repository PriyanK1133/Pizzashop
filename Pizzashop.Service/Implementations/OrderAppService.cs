using Pizzashop.Entity.Constants;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class OrderAppService : IOrderAppService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly ITableRepository _tableRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerVisitDetailRepository _customerVisitDetailRepository;
    private readonly IOrderTableRepository _orderTableRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IModifierRepository _modifierRepository;
    private readonly IOrderModifierRepository _orderModifierRepository;
    private readonly IOrderTaxRepository _orderTaxRepository;
    private readonly ITaxAndFeeRepository _taxAndFeeRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IRatingRepository _ratingRepository;
    private readonly IUnitOfWork _unitOfWork;


    public OrderAppService(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, ITableRepository tableRepository, ISectionRepository sectionRepository, ICustomerRepository customerRepository, ICustomerVisitDetailRepository customerVisitDetailRepository, IOrderTableRepository orderTableRepository, IItemRepository itemRepository, IModifierRepository modifierRepository, IOrderModifierRepository orderModifierRepository, IOrderTaxRepository orderTaxRepository, ITaxAndFeeRepository taxAndFeeRepository, IPaymentRepository paymentRepository, IRatingRepository ratingRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _tableRepository = tableRepository;
        _sectionRepository = sectionRepository;
        _customerRepository = customerRepository;
        _customerVisitDetailRepository = customerVisitDetailRepository;
        _orderTableRepository = orderTableRepository;
        _itemRepository = itemRepository;
        _modifierRepository = modifierRepository;
        _orderModifierRepository = orderModifierRepository;
        _orderTaxRepository = orderTaxRepository;
        _taxAndFeeRepository = taxAndFeeRepository;
        _paymentRepository = paymentRepository;
        _ratingRepository = ratingRepository;
        _unitOfWork = unitOfWork;
    }

    #region  Private Helper Methods
    private static int GetQuantity(int quantity, int readyQuantity, string status)
    {
        if (status.Equals(Constants.OrderItemInProgress))
        {
            return quantity - readyQuantity;
        }

        if (status.Equals(Constants.OrderItemReady))
        {
            return readyQuantity;
        }

        return quantity;

    }

    private static List<OrderItemVM> GetOrderItems(IEnumerable<OrderItem> orderItems, string status)
    {
        if (status.Equals(Constants.OrderItemInProgress))
        {
            orderItems = orderItems.Where(i => (i.Quantity - i.ReadyQuantity.GetValueOrDefault()) > 0);
        }
        else if (status.Equals(Constants.OrderItemReady))
        {
            orderItems = orderItems.Where(i => i.ReadyQuantity.GetValueOrDefault() > 0);
        }

        return orderItems.Select(i => new OrderItemVM()
        {
            Id = i.Id,
            ItemId = i.ItemId,
            Name = i.ItemName,
            Quantity = GetQuantity(i.Quantity, i.ReadyQuantity.GetValueOrDefault(), status),
            Price = i.ItemRate,
            TotalAmount = i.ItemTotal,
            Instruction = i.SpecialInstruction,
            TotalModifierAmount = i.TotalModifierAmount,

            OrderModifiers = i.OrderModifiers.Select(m => new OrderModifierVM()
            {
                Id = m.ModifierId,
                Name = m.ModifierName,
                Quantity = m.Quantity,
                Price = m.ModifierRate,
            }).ToList(),
        }).ToList();
    }

    private static string GetTableStatus(string? orderStatus)
    {
        return orderStatus switch
        {
            Constants.OrderPending => Constants.OrderTableAssigned,
            Constants.OrderInProgress => Constants.OrderTableRunning,
            Constants.OrderServed => Constants.OrderTableRunning,
            _ => Constants.OrderTableAvailable
        };
    }

    private static decimal GetTaxAmount(string type, decimal taxValue, decimal subtotal)
    {
        if (type == Constants.TaxFlatAmount)
        {
            return taxValue;
        }
        else
        {
            return taxValue * subtotal * (decimal)0.01;
        }
    }

    #endregion Private Helper Methods

    #region KOT 

    public async Task<Response<IEnumerable<KOTOrderVM>>> GetKOTOrdersAsync(string status, Guid categoryId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            IEnumerable<Order> orders = await _orderRepository.GetKOTOrdersAsync(categoryId);

            IEnumerable<KOTOrderVM> kotOrders = orders.Select(o => new KOTOrderVM()
            {
                Id = o.Id,
                CreatedAt = o.CreatedAt,
                TableNames = o.TableOrderMappings.Select(t => t.Table.Name).ToList(),
                SectionName = o.CustomerVisitDetails.Section.Name,
                OrderInstruction = o.Comment,

                OrderItems = GetOrderItems(o.OrderItems, status),

            });

            kotOrders = kotOrders.Where(o => o.OrderItems.Any());

            return Response<IEnumerable<KOTOrderVM>>.SuccessResponse(kotOrders, "KOT Details " + MessageConstants.GetMessage);

        });


    }

    public async Task<Response<bool>> UpdateOrderItemStatusAsync(Guid orderId, string status, List<OrderItemVM> updatedOrderItems)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            bool isOrderExist = await _orderRepository.IsExistsAsync(o => o.Id == orderId && (o.Status.Equals(Constants.OrderInProgress) || o.Status.Equals(Constants.OrderServed)));
            if (!isOrderExist)
            {
                return Response<bool>.FailureResponse("Order " + MessageConstants.NotFoundMessage);
            }
            // Order? order = await _orderRepository.GetByIdAsync(orderId);
            // if (order == null)
            // {
            //     return Response<bool>.FailureResponse("Order " + MessageConstants.NotFoundMessage);
            // }

            // IEnumerable<OrderItem> existingOrderItems = await _orderItemRepository.GetAllByOrderIdAsync(orderId);
            // List<OrderItem> orderItemsToUpdate = new();

            // foreach (OrderItemVM updatedOrderItem in updatedOrderItems)
            // {
            //     OrderItem? orderItem = existingOrderItems.SingleOrDefault(oi => oi.Id == updatedOrderItem.Id);

            //     if (orderItem != null)
            //     {
            //         if (status.Equals(Constants.OrderItemInProgress))
            //         {
            //             orderItem.ReadyQuantity = orderItem.ReadyQuantity.GetValueOrDefault() - updatedOrderItem.Quantity;
            //         }
            //         else
            //         {
            //             orderItem.ReadyQuantity = orderItem.ReadyQuantity.GetValueOrDefault() + updatedOrderItem.Quantity;
            //         }

            //         if (orderItem.ReadyQuantity >= 0 && orderItem.ReadyQuantity <= orderItem.Quantity)
            //         {
            //             if (orderItem.ReadyQuantity == orderItem.Quantity)
            //             {
            //                 orderItem.Status = Constants.OrderItemReady;
            //             }
            //             else
            //             {
            //                 orderItem.Status = Constants.OrderItemInProgress;
            //             }
            //             orderItemsToUpdate.Add(orderItem);
            //         }
            //     }
            // }

            // await _orderItemRepository.UpdateRangeAsync(orderItemsToUpdate);
            // if (existingOrderItems.All(oi => oi.Status == Constants.OrderItemReady))
            // {
            //     order.Status = Constants.OrderServed;
            // }
            // else
            // {
            //     order.Status = Constants.OrderInProgress;
            // }

            // if (existingOrderItems.Any(oi => oi.Status == Constants.OrderItemReady))
            // {
            //     order.OrderServedTime ??= DateTime.Now;
            // }
            // else
            // {
            //     order.OrderServedTime = null;
            // }

            // await _orderRepository.UpdateAsync(order);

            await _orderRepository.UpdateKOTItemsAsync(orderId, updatedOrderItems.Select(oi => oi.Id.ToString()).ToList(), status, updatedOrderItems.Select(oi => oi.Quantity).ToList());

            return Response<bool>.SuccessResponse(true, "Order Item status " + MessageConstants.EditMessage);

        });
    }

    #endregion KOT

    #region Tables

    public async Task<Response<IEnumerable<OrderTableVM>>> GetTableDetailsBySectionIdAsync(Guid sectionId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            IEnumerable<Table> tables = await _tableRepository.GetTablesWithOrderDetailsBySectionIdAsync(sectionId);

            IEnumerable<OrderTableVM> model = tables.Select(t => new OrderTableVM()
            {
                Id = t.Id,
                Name = t.Name,
                Capacity = t.Capacity,
                Status = GetTableStatus(t.CurrentOrder?.Status),
                OrderTime = t.CurrentOrder?.CreatedAt,
                OrderTotal = t.CurrentOrder?.OrderTotal,
                OrderId = t.CurrentOrderId
            });

            return Response<IEnumerable<OrderTableVM>>.SuccessResponse(model, "Tables " + MessageConstants.GetMessage);
        });
    }

    public async Task<Response<IEnumerable<SectionVM>>> GetAllSectionsAsync()
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            IEnumerable<SectionVM> sections = await _sectionRepository.GetAllSectionsWithStatusWiseTableCountAsync();

            return Response<IEnumerable<SectionVM>>.SuccessResponse(sections, "Sections " + MessageConstants.GetMessage);
        });
    }

    public async Task<Response<Guid?>> AssignTablesAsync(CustomerDetailsVM model, List<Guid> tables)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            // Add Customer If Not Exist
            if (model.Id == Guid.Empty)
            {
                Response<CustomerDetailsVM?> addCustomerDetailResponse = await AddCustomerDetailsAsync(model);
                if (!addCustomerDetailResponse.Success)
                {
                    return Response<Guid?>.FailureResponse(addCustomerDetailResponse.Message);
                }

                model = addCustomerDetailResponse.Data!;
            }
            else
            {
                Response<CustomerDetailsVM?> updateCustomerDetailResponse = await UpdateCustomerDetailsAsync(model);
                if (!updateCustomerDetailResponse.Success)
                {
                    return Response<Guid?>.FailureResponse(updateCustomerDetailResponse.Message);
                }

                model = updateCustomerDetailResponse.Data!;
            }

            Order order = new()
            {
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                CustomerVisitDetailsId = model.Id,
                Status = Constants.OrderPending,
                CreatedBy = model.UpdatedBy,
                UpdatedBy = model.UpdatedBy
            };

            await _orderRepository.AddAsync(order);

            List<Table> tablesToUpdate = new();
            List<TableOrderMapping> orderTablesToAdd = new();

            foreach (Guid id in tables)
            {
                Table? table = await _tableRepository.GetByIdAsync(id);
                if (table != null)
                {
                    table.IsOccupied = true;
                    table.CurrentOrderId = order.Id;
                    tablesToUpdate.Add(table);

                    TableOrderMapping orderTable = new()
                    {
                        TableId = table.Id,
                        OrderId = order.Id,
                        CreatedBy = model.UpdatedBy,
                        UpdatedBy = model.UpdatedBy
                    };

                    orderTablesToAdd.Add(orderTable);
                }
            }

            await _tableRepository.UpdateRangeAsync(tablesToUpdate);
            await _orderTableRepository.AddRangeAsync(orderTablesToAdd);

            return Response<Guid?>.SuccessResponse(order.Id, MessageConstants.TableAssignMessage);
        });
    }

    public async Task<Response<IEnumerable<OrderTableVM>>> GetAvailableTablesBySectionIdAsync(Guid sectionId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            IEnumerable<Table> tables = await _tableRepository.GetAvailableTablesBySectionIdAsync(sectionId);

            IEnumerable<OrderTableVM> availableTables = tables.Select(t => new OrderTableVM()
            {
                Id = t.Id,
                Name = t.Name,
                Capacity = t.Capacity
            });

            return Response<IEnumerable<OrderTableVM>>.SuccessResponse(availableTables, "Available Tables " + MessageConstants.GetMessage);
        });
    }

    public async Task<Response<Guid?>> AssignTablesToWaitingCustomerAsync(Guid waitingTokenId, Guid sectionId, List<Guid> tableIds, Guid updatorId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            CustomerVisitDetail? customerVisitDetail = await _customerVisitDetailRepository.GetByIdAsync(waitingTokenId);

            if (customerVisitDetail == null)
            {
                return Response<Guid?>.FailureResponse("Waiting Token " + MessageConstants.NotFoundMessage);
            }

            CustomerDetailsVM model = new()
            {
                Id = customerVisitDetail.Id,
                Name = customerVisitDetail.Customer.Name,
                Email = customerVisitDetail.Customer.Email,
                Phone = customerVisitDetail.Customer.Mobile,
                CustomerId = customerVisitDetail.Customer.Id,
                NumberOfPerson = customerVisitDetail.NoOfPersons,
                SectionId = sectionId,
                IsWaiting = false,
                UpdatedBy = updatorId,
            };

            return await AssignTablesAsync(model, tableIds);
        });
    }

    #endregion Tables

    #region WaitingToken

    public async Task<Response<CustomerDetailsVM?>> AddCustomerDetailsAsync(CustomerDetailsVM model)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            CustomerVisitDetail? waitingCustomer = await _customerVisitDetailRepository.GetWaitingCustomerByEmailAsync(model.Email);

            if (waitingCustomer != null)
            {
                return Response<CustomerDetailsVM?>.FailureResponse(MessageConstants.WaitingTokenAlreadyExistMessage);
            }

            bool hasRunningOrder = await _customerVisitDetailRepository.IsExistsAsync(cv => cv.Customer.Email.ToLower().Trim() == model.Email.ToLower().Trim() && cv.Orders.Any(o => string.Concat(Constants.OrderPending, " ", Constants.OrderInProgress).Contains(o.Status)));

            if (hasRunningOrder)
            {
                return Response<CustomerDetailsVM?>.FailureResponse(MessageConstants.CustomerOrderInRunningMessage);
            }

            if (model.CustomerId == Guid.Empty)
            {
                bool isExist = await _customerRepository.IsExistsAsync(c => c.Email == model.Email);
                if (isExist)
                {
                    Customer? existingCustomer = await _customerRepository.GetByEmailAsync(model.Email);
                    model.CustomerId = existingCustomer!.Id;
                }
                else
                {
                    Customer customer = new()
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Mobile = model.Phone,
                        CreatedBy = model.CreatedBy,
                        UpdatedBy = model.UpdatedBy
                    };

                    await _customerRepository.AddAsync(customer);
                    model.CustomerId = customer.Id;
                }
            }


            CustomerVisitDetail customerVisitDetail = new()
            {
                CustomerId = model.CustomerId,
                SectionId = model.SectionId,
                NoOfPersons = model.NumberOfPerson.GetValueOrDefault(),
                IsWaiting = model.IsWaiting,
                CreatedBy = model.CreatedBy,
                UpdatedBy = model.UpdatedBy
            };

            await _customerVisitDetailRepository.AddAsync(customerVisitDetail);
            model.Id = customerVisitDetail.Id;

            return Response<CustomerDetailsVM?>.SuccessResponse(model, "Customer Details " + MessageConstants.CreateMessage);
        });
    }

    public async Task<Response<CustomerDetailsVM?>> UpdateCustomerDetailsAsync(CustomerDetailsVM model)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            CustomerVisitDetail? customerVisitDetail = await _customerVisitDetailRepository.GetByIdAsync(model.Id);
            if (customerVisitDetail == null)
            {
                return Response<CustomerDetailsVM?>.FailureResponse("Customer Details " + MessageConstants.NotFoundMessage);
            }

            Customer? customer = await _customerRepository.GetByIdAsync(customerVisitDetail.CustomerId);

            if (customer == null)
            {
                return Response<CustomerDetailsVM?>.FailureResponse("Customer " + MessageConstants.NotFoundMessage);
            }

            bool isExist = await _customerRepository.IsExistsAsync(c => c.Email.ToLower().Trim() == model.Email.ToLower().Trim() && c.Id != model.CustomerId);

            if (isExist)
            {
                return Response<CustomerDetailsVM?>.FailureResponse("Customer with email " + MessageConstants.AlreadyExistMessage);
            }

            customer.Email = model.Email;
            customer.Name = model.Name;
            customer.Mobile = model.Phone;
            customer.UpdatedBy = model.UpdatedBy;
            customer.UpdatedAt = DateTime.Now;
            await _customerRepository.UpdateAsync(customer);

            customerVisitDetail.SectionId = model.SectionId;
            customerVisitDetail.NoOfPersons = model.NumberOfPerson.GetValueOrDefault();
            customerVisitDetail.IsWaiting = model.IsWaiting;
            customerVisitDetail.UpdatedBy = model.UpdatedBy;
            customerVisitDetail.UpdatedAt = DateTime.Now;

            await _customerVisitDetailRepository.UpdateAsync(customerVisitDetail);

            return Response<CustomerDetailsVM?>.SuccessResponse(model, "Customer Details " + MessageConstants.EditMessage);

        });
    }

    public async Task<Response<IEnumerable<CustomerDetailsVM>>> GetWaitingListAsync(Guid sectionId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            IEnumerable<CustomerVisitDetail> customerVisitDetails = _customerVisitDetailRepository.GetWaitingListBySP(sectionId);

            IEnumerable<CustomerDetailsVM> waitinglist = customerVisitDetails.Select(cv => new CustomerDetailsVM()
            {
                Id = cv.Id,
                Name = cv.Customer.Name,
                Email = cv.Customer.Email,
                NumberOfPerson = cv.NoOfPersons,
                Phone = cv.Customer.Mobile,
                CustomerId = cv.CustomerId,
                IsWaiting = cv.IsWaiting,
                SectionId = cv.SectionId,
                CreatedAt = cv.CreatedAt
            });

            return Response<IEnumerable<CustomerDetailsVM>>.SuccessResponse(waitinglist, "Waiting List " + MessageConstants.GetMessage);
        });

    }

    public async Task<Response<CustomerDetailsVM?>> GetCustomerDetailsByEmailAsync(string email)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            CustomerVisitDetail? customerDetail = _customerVisitDetailRepository.GetWaitingCustomerByEmailSP(email);
            CustomerDetailsVM? CustomerDetailsVM;

            if (customerDetail != null)
            {
                CustomerDetailsVM = new()
                {
                    Id = customerDetail.Id,
                    CustomerId = customerDetail.CustomerId,
                    Name = customerDetail.Customer.Name,
                    Email = customerDetail.Customer.Email,
                    Phone = customerDetail.Customer.Mobile,
                    NumberOfPerson = customerDetail.NoOfPersons,
                    SectionId = customerDetail.SectionId,
                    IsWaiting = customerDetail.IsWaiting,
                };

                return Response<CustomerDetailsVM?>.SuccessResponse(CustomerDetailsVM, "Customer Details " + MessageConstants.GetMessage);
            }

            Customer? customer = await _customerRepository.GetByEmailAsync(email);
            if (customer == null)
            {
                return Response<CustomerDetailsVM?>.FailureResponse("Customer " + MessageConstants.NotFoundMessage);
            }

            CustomerDetailsVM = new()
            {
                CustomerId = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Mobile,
            };

            return Response<CustomerDetailsVM?>.SuccessResponse(CustomerDetailsVM, "Customer Details " + MessageConstants.GetMessage);

        });
    }

    public async Task<Response<bool>> DeleteWaitingToken(Guid id)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            // CustomerVisitDetail? customerVisitDetail = await _customerVisitDetailRepository.GetByIdAsync(id);
            // if (customerVisitDetail == null || !customerVisitDetail.IsWaiting)
            // {
            //     return Response<bool>.FailureResponse("Waiting Token " + MessageConstants.NotFoundMessage);
            // }

            // Guid customerId = customerVisitDetail.CustomerId;

            // bool isACustomer = await _customerRepository.IsExistsAsync(c => c.Id == customerId && c.CustomerVisitDetails.Any(cv => !cv.IsWaiting));
            // await _customerVisitDetailRepository.RemoveAsync(customerVisitDetail);

            // if (!isACustomer)
            // {
            //     Customer? customer = await _customerRepository.GetByIdAsync(customerId);
            //     await _customerRepository.RemoveAsync(customer!);
            // }
            string message = await _customerVisitDetailRepository.RemoveBySP(id);
            // if (string.IsNullOrEmpty(message))
            // {
            //     return Response<bool>.FailureResponse("Waiting Token " + MessageConstants.NotFoundMessage);
            // }
            return Response<bool>.SuccessResponse(true, "Waiting Token " + MessageConstants.DeleteMessage);
        });
    }

    #endregion WaitingToken

    #region Menu

    public async Task<Response<string?>> EditOrderCommentAsync(Guid id, string? comment, Guid updatorId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Order? order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
            {
                return Response<string?>.FailureResponse("Order " + MessageConstants.NotFoundMessage);
            }

            order.Comment = comment;
            order.UpdatedAt = DateTime.Now;
            order.UpdatedBy = updatorId;

            await _orderRepository.UpdateAsync(order);

            return Response<string?>.SuccessResponse(comment, "Order Comment " + MessageConstants.EditMessage);
        });
    }

    public async Task<Response<string?>> SaveOrderItemInstructionAsync(Guid id, string? specialInstruction, Guid updatorId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            OrderItem? orderItem = await _orderItemRepository.GetByIdAsync(id);

            if (orderItem == null)
            {
                return Response<string?>.FailureResponse("Order Item " + MessageConstants.NotFoundMessage);
            }

            orderItem.SpecialInstruction = specialInstruction;
            orderItem.UpdatedAt = DateTime.Now;
            orderItem.UpdatedBy = updatorId;

            await _orderItemRepository.UpdateAsync(orderItem);

            return Response<string?>.SuccessResponse(specialInstruction, "Order Item instruction " + MessageConstants.EditMessage);
        });
    }

    public async Task<Response<string?>> GetOrderItemInstructionAsync(Guid id)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            string? specialInstruction = await _orderItemRepository.GetSpecialInstructionByIdAsync(id);
            return Response<string?>.SuccessResponse(specialInstruction, "Order Item instruction " + MessageConstants.GetMessage);
        });
    }

    public async Task<Response<bool>> SaveOrderAsync(OrderDetailsVM model)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            Order? order = await _orderRepository.GetByIdAsync(model.Id);

            if (order == null)
            {
                await _unitOfWork.RollbackAsync();
                return Response<bool>.FailureResponse("Order " + MessageConstants.NotFoundMessage);
            }

            List<OrderItem> orderItemsToUpdate = new();
            List<OrderItem> orderItemsToRemove = new();

            List<OrderModifier> orderModifiersToRemove = new();
            List<OrderModifier> orderModifiersToAdd = new();
            List<OrderModifier> orderModifiersToUpdate = new();
            decimal subtotal = 0;

            IEnumerable<OrderItem> existingOrderItems = await _orderItemRepository.GetAllByOrderIdAsync(model.Id);

            #region  OrderItemRemove
            foreach (OrderItem existingOrderItem in existingOrderItems)
            {
                if (!model.OrderItems.Any(oi => oi.Id == existingOrderItem.Id))
                {
                    orderItemsToRemove.Add(existingOrderItem);
                    IEnumerable<OrderModifier> orderModifiers = await _orderModifierRepository.GetAllByOrderItemIdAsync(existingOrderItem.Id);
                    orderModifiersToRemove.AddRange(orderModifiers);
                }
            }
            await _orderModifierRepository.RemoveRangeAsync(orderModifiersToRemove);
            await _orderItemRepository.RemoveRangeAsync(orderItemsToRemove);
            #endregion OrderItemRemove

            #region  Order Items Add and Update
            foreach (OrderItemVM orderItemToSave in model.OrderItems)
            {
                OrderItem? orderItem = existingOrderItems.SingleOrDefault(oi => oi.Id == orderItemToSave.Id);

                decimal totalModifierAmount = 0;

                if (orderItem == null)
                {
                    Item? item = await _itemRepository.GetByIdAsync(orderItemToSave.ItemId);

                    if (item == null)
                    {
                        await _unitOfWork.RollbackAsync();
                        return Response<bool>.FailureResponse("Item " + MessageConstants.NotFoundMessage);
                    }

                    orderItem = new()
                    {
                        ItemId = item.Id,
                        OrderId = model.Id,
                        Quantity = orderItemToSave.Quantity,
                        Status = Constants.OrderItemInProgress,
                        Tax = item.TaxPercentage,
                        TotalModifierAmount = 0,
                        ItemTotal = item.Rate * orderItemToSave.Quantity,
                        ItemName = item.Name,
                        ItemRate = item.Rate,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        CreatedBy = model.CreatedBy,
                        UpdatedBy = model.UpdatedBy,
                    };

                    await _orderItemRepository.AddAsync(orderItem);

                    foreach (Guid id in orderItemToSave.ModifierIds)
                    {
                        Modifier? modifier = await _modifierRepository.GetByIdAsync(id);

                        if (modifier == null)
                        {
                            await _unitOfWork.RollbackAsync();
                            return Response<bool>.FailureResponse("Modifier " + MessageConstants.NotFoundMessage);
                        }

                        OrderModifier orderModifier = new()
                        {
                            OrderItemId = orderItem.Id,
                            Quantity = orderItem.Quantity,
                            ModifierId = modifier.Id,
                            TotalAmount = orderItem.Quantity * modifier.Rate,
                            ModifierName = modifier.Name,
                            ModifierRate = modifier.Rate,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            CreatedBy = model.CreatedBy,
                            UpdatedBy = model.UpdatedBy
                        };

                        totalModifierAmount += orderModifier.TotalAmount;
                        orderModifiersToAdd.Add(orderModifier);
                    }
                }
                else
                {
                    orderItem.Quantity = orderItemToSave.Quantity;
                    orderItem.ItemTotal = orderItem.ItemRate * orderItemToSave.Quantity;
                    orderItem.Status = (orderItem.ReadyQuantity < orderItem.Quantity) ? Constants.OrderItemInProgress : Constants.OrderItemReady;
                    IEnumerable<OrderModifier> existingOrderModifiersForOrderItem = await _orderModifierRepository.GetAllByOrderItemIdAsync(orderItem.Id);

                    foreach (Guid id in orderItemToSave.ModifierIds)
                    {
                        OrderModifier? orderModifier = existingOrderModifiersForOrderItem.SingleOrDefault(om => om.ModifierId == id);

                        if (orderModifier == null)
                        {
                            Modifier? modifier = await _modifierRepository.GetByIdAsync(id);

                            if (modifier == null)
                            {
                                await _unitOfWork.RollbackAsync();
                                return Response<bool>.FailureResponse("Modifier " + MessageConstants.NotFoundMessage);
                            }

                            orderModifier = new()
                            {
                                OrderItemId = orderItem.Id,
                                Quantity = orderItemToSave.Quantity,
                                ModifierId = modifier.Id,
                                TotalAmount = orderItemToSave.Quantity * modifier.Rate,
                                ModifierName = modifier.Name,
                                ModifierRate = modifier.Rate,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                CreatedBy = model.CreatedBy,
                                UpdatedBy = model.UpdatedBy
                            };

                            totalModifierAmount += orderModifier.TotalAmount;
                            orderModifiersToAdd.Add(orderModifier);
                        }
                        else
                        {
                            orderModifier.Quantity = orderItemToSave.Quantity;
                            orderModifier.TotalAmount = orderModifier.ModifierRate * orderItemToSave.Quantity;
                            orderModifier.UpdatedAt = DateTime.Now;
                            orderModifier.UpdatedBy = model.UpdatedBy;

                            totalModifierAmount += orderModifier.TotalAmount;
                            orderModifiersToUpdate.Add(orderModifier);
                        }
                    }
                }
                orderItem.ItemTotal *= 1 + ((decimal)0.01 * orderItem.Tax);
                orderItem.TotalModifierAmount = totalModifierAmount;
                orderItem.TotalAmount = orderItem.ItemTotal + orderItem.TotalModifierAmount;

                subtotal += orderItem.ItemTotal + totalModifierAmount;
                orderItemsToUpdate.Add(orderItem);
            }

            await _orderModifierRepository.AddRangeAsync(orderModifiersToAdd);
            await _orderModifierRepository.UpdateRangeAsync(orderModifiersToUpdate);
            await _orderItemRepository.UpdateRangeAsync(orderItemsToUpdate);
            #endregion  Order Items Add and Update

            #region Order Tax Add and Update
            IEnumerable<OrderTaxis> orderTaxes = await _orderTaxRepository.GetAllByOrderId(model.Id);

            List<OrderTaxis> orderTaxesToAdd = new();
            List<OrderTaxis> orderTaxesToUpdate = new();
            decimal total = subtotal;

            foreach (Guid id in model.OrderTaxIds)
            {
                OrderTaxis? orderTax = orderTaxes.SingleOrDefault(ot => ot.TaxId == id);

                if (orderTax == null)
                {
                    TaxesAndFee? tax = await _taxAndFeeRepository.GetByIdAsync(id);
                    if (tax == null)
                    {
                        await _unitOfWork.RollbackAsync();
                        return Response<bool>.FailureResponse("Tax " + MessageConstants.NotFoundMessage);
                    }
                    orderTax = new()
                    {
                        OrderId = model.Id,
                        TaxId = id,
                        TaxName = tax.Name,
                        TaxValue = tax.TaxAmount,
                        TaxType = tax.Type,
                        TotalTax = GetTaxAmount(tax.Type, tax.TaxAmount, subtotal),
                        CreatedBy = model.CreatedBy,
                        UpdatedBy = model.UpdatedBy
                    };
                    orderTaxesToAdd.Add(orderTax);
                }
                else
                {
                    orderTax.TotalTax = GetTaxAmount(orderTax.TaxType, orderTax.TaxValue, subtotal);
                    orderTax.UpdatedAt = DateTime.Now;
                    orderTax.UpdatedBy = model.UpdatedBy;

                    orderTaxesToUpdate.Add(orderTax);
                }
                total += orderTax.TotalTax;
            }

            await _orderTaxRepository.UpdateRangeAsync(orderTaxesToUpdate);
            await _orderTaxRepository.AddRangeAsync(orderTaxesToAdd);
            #endregion Order Tax Add and Update

            order.OrderTotal = total;
            order.Subtotal = subtotal;
            order.UpdatedAt = DateTime.Now;
            order.UpdatedBy = model.UpdatedBy;

            existingOrderItems = await _orderItemRepository.GetAllByOrderIdAsync(model.Id);
            if (existingOrderItems.Any(i => i.ReadyQuantity != i.Quantity))
            {
                order.Status = Constants.OrderInProgress;
            }
            else if (!existingOrderItems.Any())
            {
                order.Status = Constants.OrderPending;
            }
            else
            {
                order.Status = Constants.OrderServed;
            }

            await _orderRepository.UpdateAsync(order);



            await _unitOfWork.CommitAsync();
            return Response<bool>.SuccessResponse(true, "Order " + MessageConstants.SaveMessage);
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            return Response<bool>.FailureResponse(MessageConstants.DefaultErrorMessage);
        }
    }

    public async Task<Response<bool>> IsItemQuantityPrepared(Guid orderItemId, int quantity)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            int readyQuantity = await _orderItemRepository.GetReadyQuantityAsync(orderItemId);

            if (readyQuantity < quantity)
            {
                return Response<bool>.FailureResponse(MessageConstants.ItemNotPreparedMessage);
            }

            return Response<bool>.SuccessResponse(true, MessageConstants.ItemAlreadyPreparedMessage);

        });
    }

    public async Task<Response<bool>> CompleteOrderAsync(Guid orderId, string paymentMethod, Guid updatorId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Order? order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                return Response<bool>.FailureResponse("Order " + MessageConstants.NotFoundMessage);
            }

            if (!order.Status.Equals(Constants.OrderServed))
            {
                return Response<bool>.FailureResponse(MessageConstants.FailedToCompleteOrderMessage);
            }

            order.Status = Constants.OrderCompleted;
            order.UpdatedAt = DateTime.Now;
            order.UpdatedBy = updatorId;

            IEnumerable<TableOrderMapping> orderTables = await _orderTableRepository.GetByOrderIdAsync(orderId);
            List<Table> tablesToUpdate = new();

            foreach (TableOrderMapping orderTable in orderTables)
            {
                Table table = orderTable.Table;
                table.CurrentOrderId = null;
                table.IsOccupied = false;
                tablesToUpdate.Add(table);
            }

            Payment payment = new()
            {
                OrderId = orderId,
                PaymentMode = paymentMethod,
                Amount = order.OrderTotal,
                Status = Constants.PaymentPending,
                CreatedBy = updatorId,
                UpdatedBy = updatorId
            };

            await _paymentRepository.AddAsync(payment);
            await _tableRepository.UpdateRangeAsync(tablesToUpdate);
            await _orderRepository.UpdateAsync(order);
            return Response<bool>.SuccessResponse(true, "Order " + MessageConstants.CompleteMessage);
        });
    }

    public async Task<Response<bool>> IsOrderServedAsync(Guid orderId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            bool isOrderServed = await _orderRepository.IsExistsAsync(o => o.Id == orderId && o.Status.Equals(Constants.OrderServed));

            if (!isOrderServed)
            {
                return Response<bool>.FailureResponse(MessageConstants.OrderNotServedMessage);
            }

            return Response<bool>.SuccessResponse(true, MessageConstants.OrderServedMessage);
        });
    }

    public async Task<Response<bool>> SaveRatingAsync(RatingVM model)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Rating rating = new()
            {
                OrderId = model.OrderId,
                FoodRating = model.FoodRating,
                ServiceRating = model.ServiceRating,
                AmbienceRating = model.AmbienceRating,
                Comment = model.Comment,
                CreatedBy = model.CreatedBy,
                UpdatedBy = model.UpdatedBy
            };

            await _ratingRepository.AddAsync(rating);
            return Response<bool>.SuccessResponse(true, "Rating " + MessageConstants.SaveMessage);
        });
    }

    public async Task<Response<bool>> CancelOrderAsync(Guid orderId, Guid updatorId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Order? order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return Response<bool>.FailureResponse("Order " + MessageConstants.NotFoundMessage);
            }

            bool isAnyItemPrepared = await _orderRepository.IsExistsAsync(o => o.Id == orderId && o.OrderItems.Any(oi => oi.ReadyQuantity > 0));

            if (isAnyItemPrepared)
            {
                return Response<bool>.FailureResponse(MessageConstants.OrderNotCancelMessage);
            }

            order.Status = Constants.OrderCancelled;
            order.UpdatedAt = DateTime.Now;
            order.UpdatedBy = updatorId;

            IEnumerable<TableOrderMapping> orderTables = await _orderTableRepository.GetByOrderIdAsync(orderId);
            List<Table> tablesToUpdate = new();

            foreach (TableOrderMapping orderTable in orderTables)
            {
                Table table = orderTable.Table;
                table.CurrentOrderId = null;
                table.IsOccupied = false;
                tablesToUpdate.Add(table);
            }

            await _tableRepository.UpdateRangeAsync(tablesToUpdate);
            await _orderRepository.UpdateAsync(order);
            return Response<bool>.SuccessResponse(true, "Order " + MessageConstants.CancelMessage);
        });
    }

    #endregion Menu
}
