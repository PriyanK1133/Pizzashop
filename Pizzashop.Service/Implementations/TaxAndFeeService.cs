using Pizzashop.Entity.Constants;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Helper;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class TaxAndFeeService : ITaxAndFeeService
{
    private readonly ITaxAndFeeRepository _taxAndFeeRepository;
    public TaxAndFeeService(ITaxAndFeeRepository taxAndFeeRepository)
    {
        _taxAndFeeRepository = taxAndFeeRepository;
    }
    public async Task<Response<TaxAndFeeVM?>> GetById(Guid id)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            TaxesAndFee? taxesAndFee = await _taxAndFeeRepository.GetByIdAsync(id);
            if (taxesAndFee == null)
            {
                return Response<TaxAndFeeVM?>.FailureResponse("Tax/Fee " + MessageConstants.NotFoundMessage);
            }

            TaxAndFeeVM taxAndFeeVM = new()
            {
                Id = taxesAndFee.Id,
                Name = taxesAndFee.Name,
                Type = taxesAndFee.Type,
                TaxAmount = taxesAndFee.TaxAmount,
                IsEnabled = taxesAndFee.IsEnabled.GetValueOrDefault(),
                IsDefault = taxesAndFee.IsDefault,
                CreatedBy = taxesAndFee.CreatedBy,
                UpdatedBy = taxesAndFee.UpdatedBy
            };

            return Response<TaxAndFeeVM?>.SuccessResponse(taxAndFeeVM, "Tax/Fee " + MessageConstants.GetMessage);
        });
    }

    public async Task<Response<IEnumerable<TaxAndFeeVM>>> GetAllAsync()
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            IEnumerable<TaxesAndFee> taxesAndFees = await _taxAndFeeRepository.GetAllAsync();

            IEnumerable<TaxAndFeeVM> taxAndFeeList = taxesAndFees.Select(taxesAndFee => new TaxAndFeeVM()
            {
                Id = taxesAndFee.Id,
                Name = taxesAndFee.Name,
                Type = taxesAndFee.Type,
                TaxAmount = taxesAndFee.TaxAmount,
                IsEnabled = taxesAndFee.IsEnabled.GetValueOrDefault(),
                IsDefault = taxesAndFee.IsDefault,
                CreatedBy = taxesAndFee.CreatedBy,
                UpdatedBy = taxesAndFee.UpdatedBy
            }).ToList();

            return Response<IEnumerable<TaxAndFeeVM>>.SuccessResponse(taxAndFeeList, "Taxes/Fees " + MessageConstants.GetMessage);
        });
    }
    public async Task<Response<PagedResult<TaxAndFeeVM>>> GetPagedListAsync(string searchString, int page, int pagesize, bool isASC)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
     {
         (IEnumerable<TaxesAndFee> taxesAndFees, int totalRecords) = await _taxAndFeeRepository.GetPagedListAsync(searchString, page, pagesize, isASC);

         IEnumerable<TaxAndFeeVM> taxAndFeeList = taxesAndFees.Select(taxesAndFee => new TaxAndFeeVM()
         {
             Id = taxesAndFee.Id,
             Name = taxesAndFee.Name,
             Type = taxesAndFee.Type,
             TaxAmount = taxesAndFee.TaxAmount,
             IsEnabled = taxesAndFee.IsEnabled.GetValueOrDefault(),
             IsDefault = taxesAndFee.IsDefault,
             CreatedBy = taxesAndFee.CreatedBy,
             UpdatedBy = taxesAndFee.UpdatedBy
         }).ToList();

         PagedResult<TaxAndFeeVM> pagedResult = new()
         {
             PagedList = taxAndFeeList
         };

         pagedResult.Pagination.SetPagination(totalRecords, pagesize, page);

         return Response<PagedResult<TaxAndFeeVM>>.SuccessResponse(pagedResult, "Taxes/Fees " + MessageConstants.GetMessage);
     });
    }

    public async Task<Response<TaxAndFeeVM?>> AddAsync(TaxAndFeeVM model)
    {
        TaxesAndFee taxesAndFee = new()
        {
            Name = model.Name,
            Type = model.Type,
            TaxAmount = model.TaxAmount.GetValueOrDefault(),
            IsEnabled = model.IsEnabled,
            IsDefault = model.IsDefault,
            CreatedBy = model.CreatedBy,
            UpdatedBy = model.UpdatedBy
        };

        await _taxAndFeeRepository.AddAsync(taxesAndFee);
        return Response<TaxAndFeeVM?>.SuccessResponse(model, "Tax/Fee " + MessageConstants.CreateMessage);
    }

    public async Task<Response<TaxAndFeeVM?>> EditAsync(TaxAndFeeVM model)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            TaxesAndFee? taxesAndFee = await _taxAndFeeRepository.GetByIdAsync(model.Id);
            if (taxesAndFee == null)
            {
                return Response<TaxAndFeeVM?>.FailureResponse("Tax/Fee " + MessageConstants.NotFoundMessage);
            }

            taxesAndFee.Name = model.Name;
            taxesAndFee.Type = model.Type;
            taxesAndFee.TaxAmount = model.TaxAmount.GetValueOrDefault();
            taxesAndFee.IsEnabled = model.IsEnabled;
            taxesAndFee.IsDefault = model.IsDefault;
            taxesAndFee.UpdatedAt = DateTime.Now;
            taxesAndFee.UpdatedBy = model.UpdatedBy;

            await _taxAndFeeRepository.UpdateAsync(taxesAndFee);

            return Response<TaxAndFeeVM?>.SuccessResponse(model, "Tax/Fee " + MessageConstants.EditMessage);

        });
    }

    public async Task<Response<bool>> DeleteAsync(Guid id, Guid deltorId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            TaxesAndFee? taxesAndFee = await _taxAndFeeRepository.GetByIdAsync(id);
            if (taxesAndFee == null)
            {
                return Response<bool>.FailureResponse("Tax/Fee " + MessageConstants.NotFoundMessage);
            }

            taxesAndFee.IsDeleted = true;
            taxesAndFee.UpdatedAt = DateTime.Now;
            taxesAndFee.UpdatedBy = deltorId;

            await _taxAndFeeRepository.UpdateAsync(taxesAndFee);

            return Response<bool>.SuccessResponse(true, "Tax/Fee " + MessageConstants.DeleteMessage);
        });
    }
}
