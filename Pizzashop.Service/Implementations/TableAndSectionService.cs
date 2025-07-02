using Pizzashop.Entity.Constants;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Helper;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class TableAndSectionService : ITableAndSectionService
{

    private readonly ISectionRepository _sectionRepository;
    private readonly ITableRepository _tableRepository;

    public TableAndSectionService(ISectionRepository sectionRepository, ITableRepository tableRepository)
    {
        _sectionRepository = sectionRepository;
        _tableRepository = tableRepository;
    }

    #region  Sections
    public async Task<Response<IEnumerable<SectionVM>>> GetAllSectionsAsync()
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            IEnumerable<Section> sections = await _sectionRepository.GetAllAsync();
            IEnumerable<SectionVM> sectionlist = sections.Select(c => new SectionVM()
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToList();

            return Response<IEnumerable<SectionVM>>.SuccessResponse(sectionlist, "Sections " + MessageConstants.GetMessage);
        });
    }

    public async Task<Response<SectionVM?>> GetSectionByIdAsync(Guid id)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
      {
          Section? section = await _sectionRepository.GetByIdAsync(id);
          if (section == null)
          {
              return Response<SectionVM?>.FailureResponse("Section " + MessageConstants.NotFoundMessage);
          }

          SectionVM sectionVM = new()
          {
              Id = section.Id,
              Name = section.Name,
              Description = section.Description,
              CreatedBy = section.CreatedBy,
              UpdatedBy = section.UpdatedBy
          };

          return Response<SectionVM?>.SuccessResponse(sectionVM, "Section " + MessageConstants.GetMessage);
      });
    }

    public Task<Response<SectionVM?>> AddSectionAsync(SectionVM model)
    {
        return ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            bool isExist = await _sectionRepository.IsExistsAsync(s => s.Name.ToLower().Equals(model.Name.ToLower().Trim()) && !s.IsDeleted);
            if (isExist)
            {
                return Response<SectionVM?>.FailureResponse("Section " + MessageConstants.AlreadyExistMessage);
            }

            Section section = new()
            {
                Name = model.Name,
                Description = model.Description,
                CreatedBy = model.CreatedBy,
                UpdatedBy = model.UpdatedBy
            };

            await _sectionRepository.AddAsync(section);
            return Response<SectionVM?>.SuccessResponse(model, "Section " + MessageConstants.CreateMessage);
        });
    }

    public async Task<Response<SectionVM?>> EditSectionAsync(SectionVM model)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Section? section = await _sectionRepository.GetByIdAsync(model.Id);
            if (section == null)
            {
                return Response<SectionVM?>.FailureResponse("Section " + MessageConstants.NotFoundMessage);
            }

            bool isExist = await _sectionRepository.IsExistsAsync(s => s.Name.ToLower().Equals(model.Name.ToLower().Trim()) && !s.Id.Equals(model.Id) && !s.IsDeleted);
            if (isExist)
            {
                return Response<SectionVM?>.FailureResponse("Section " + MessageConstants.AlreadyExistMessage);
            }

            section.Name = model.Name;
            section.Description = model.Description;
            section.UpdatedAt = DateTime.Now;
            section.UpdatedBy = model.UpdatedBy;

            await _sectionRepository.UpdateAsync(section);

            return Response<SectionVM?>.SuccessResponse(model, "Section " + MessageConstants.EditMessage);
        });

    }

    public async Task<Response<bool>> DeleteSectionAsync(Guid id, Guid deletorId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Section? section = await _sectionRepository.GetByIdAsync(id);
            if (section == null)
            {
                return Response<bool>.FailureResponse("Section " + MessageConstants.NotFoundMessage);
            }

            section.UpdatedAt = DateTime.Now;
            section.IsDeleted = true;
            section.UpdatedBy = deletorId;

            IEnumerable<Table> tablesToDelete = await _tableRepository.GetAllBySectionIdAsync(id);

            foreach (Table table in tablesToDelete)
            {
                if(table.IsOccupied){
                    return Response<bool>.FailureResponse(MessageConstants.SectionNotDeleteMessage);
                }

                table.IsDeleted = true;
                table.UpdatedBy = deletorId;
                table.UpdatedAt = DateTime.Now;
            }

            await _tableRepository.UpdateRangeAsync(tablesToDelete);
            await _sectionRepository.UpdateAsync(section);

            return Response<bool>.SuccessResponse(true, "Section " + MessageConstants.DeleteMessage);
        });

    }

    public async Task<Response<bool>> ChangeSectionOrder(List<Guid> sortedIds)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            List<Section> sectionsToUpdate = new();
            for (int i = 0; i < sortedIds.Count; i++)
            {
                Section? section = await _sectionRepository.GetByIdAsync(sortedIds[i]);

                if (section != null)
                {
                    section.Preference = i + 1;
                    sectionsToUpdate.Add(section);
                }
            }

            await _sectionRepository.UpdateRangeAsync(sectionsToUpdate);

            return Response<bool>.SuccessResponse(true, "Section Order " + MessageConstants.EditMessage);
        });
    }


    #endregion Sections

    #region  Tables
    public async Task<Response<TableVM?>> GetTableByIdAsync(Guid id)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
     {
         Table? table = await _tableRepository.GetByIdAsync(id);
         if (table == null)
         {
             return Response<TableVM?>.FailureResponse("Table " + MessageConstants.NotFoundMessage);
         }

         TableVM tableVM = new()
         {
             Id = table.Id,
             Name = table.Name,
             Capacity = table.Capacity,
             IsOccupied = table.IsOccupied,
             SectionId = table.SectionId,
             SectionName = table.Section.Name,
             CreatedBy = table.CreatedBy,
             UpdatedBy = table.UpdatedBy
         };

         return Response<TableVM?>.SuccessResponse(tableVM, "Table " + MessageConstants.GetMessage);
     });
    }

    public async Task<Response<IEnumerable<TableVM>>> GetTablesBySectionIdAsync(Guid sectionId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
      {
          IEnumerable<Table> tables = await _tableRepository.GetAllBySectionIdAsync(sectionId);

          IEnumerable<TableVM> tablelist = tables.Select(t => new TableVM()
          {
              Id = t.Id,
              Name = t.Name,
              Capacity = t.Capacity,
              IsOccupied = t.IsOccupied,
              SectionId = t.SectionId,
              CreatedBy = t.CreatedBy,
              UpdatedBy = t.UpdatedBy
          }).ToList();
          return Response<IEnumerable<TableVM>>.SuccessResponse(tablelist, "Tables " + MessageConstants.GetMessage);
      });
    }

    public async Task<Response<TableAndSectionVM?>> GetPagedTablesAsync(Guid sectionId, string searchString, int page, int pagesize, bool isASC)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
      {
          Section? section = await _sectionRepository.GetByIdAsync(sectionId);
          if (section == null)
          {
              return Response<TableAndSectionVM?>.FailureResponse("Section " + MessageConstants.NotFoundMessage);
          }

          (IEnumerable<Table> tables, int totalRecords) = await _tableRepository.GetPagedTablesAsync(sectionId, searchString, page, pagesize, isASC);

          IEnumerable<TableVM> tablelist = tables.Select(i => new TableVM()
          {
              Id = i.Id,
              Name = i.Name,
              Capacity = i.Capacity,
              IsOccupied = i.IsOccupied,
              SectionId = i.SectionId,
              SectionName = section.Name,
              CreatedBy = i.CreatedBy,
              UpdatedBy = i.UpdatedBy
          }).ToList();

          PagedResult<TableVM> pagedResult = new()
          {
              PagedList = tablelist
          };

          pagedResult.Pagination.SetPagination(totalRecords, pagesize, page);

          TableAndSectionVM tableAndSectionVM = new()
          {
              SectionId = sectionId,
              SectionName = section.Name,
              TablePagination = pagedResult
          };

          return Response<TableAndSectionVM?>.SuccessResponse(tableAndSectionVM, "Tables " + MessageConstants.GetMessage);
      });
    }

    public async Task<Response<TableVM?>> AddTableAsync(TableVM model)
    {

        return await ExceptionHandler.HandleExceptionsAsync(async () =>
       {
           bool isExist = await _tableRepository.IsExistsAsync(t => t.Name.ToLower().Equals(model.Name.ToLower().Trim()) && !t.IsDeleted && t.SectionId.Equals(model.SectionId));
           if (isExist)
           {
               return Response<TableVM?>.FailureResponse("Table " + MessageConstants.AlreadyExistMessage);
           }

           Table table = new()
           {
               Name = model.Name,
               Capacity = model.Capacity.GetValueOrDefault(),
               IsOccupied = model.IsOccupied,
               SectionId = model.SectionId,
               CreatedBy = model.CreatedBy,
               UpdatedBy = model.UpdatedBy
           };
           await _tableRepository.AddAsync(table);
           return Response<TableVM?>.SuccessResponse(model, "Table " + MessageConstants.CreateMessage);
       });
    }

    public async Task<Response<TableVM?>> EditTableAsync(TableVM model)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
      {
          Table? table = await _tableRepository.GetByIdAsync(model.Id);
          if (table == null)
          {
              return Response<TableVM?>.FailureResponse("Table " + MessageConstants.NotFoundMessage);
          }

          bool isExist = await _tableRepository.IsExistsAsync(t => t.Name.ToLower().Equals(model.Name.ToLower().Trim()) && !t.Id.Equals(model.Id) && !t.IsDeleted && t.SectionId.Equals(model.SectionId));
          if (isExist)
          {
              return Response<TableVM?>.FailureResponse("Table " + MessageConstants.AlreadyExistMessage);
          }

          table.Name = model.Name;
          table.Capacity = model.Capacity.GetValueOrDefault();
          table.IsOccupied = table.IsOccupied;
          table.SectionId = model.SectionId;
          table.UpdatedAt = DateTime.Now;
          table.UpdatedBy = model.UpdatedBy;

          await _tableRepository.UpdateAsync(table);

          return Response<TableVM?>.SuccessResponse(model, "Table " + MessageConstants.EditMessage);
      });

    }

    public async Task<Response<bool>> DeleteTableAsync(Guid id, Guid deletorId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Table? table = await _tableRepository.GetByIdAsync(id);
            if (table == null)
            {
                return Response<bool>.FailureResponse("Table " + MessageConstants.NotFoundMessage);
            }

            if(table.IsOccupied){
                return Response<bool>.FailureResponse(MessageConstants.TableNotDeleteMessage);
            }

            table.UpdatedAt = DateTime.Now;
            table.IsDeleted = true;
            table.UpdatedBy = deletorId;

            await _tableRepository.UpdateAsync(table);

            return Response<bool>.SuccessResponse(true, "Table " + MessageConstants.DeleteMessage);
        });
    }

    public async Task<Response<bool>> DeleteManyTableAsync(IEnumerable<Guid> ids, Guid deletorId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            List<Table> tablesToDelete = new();
            foreach (Guid id in ids)
            {
                Table? table = await _tableRepository.GetByIdAsync(id);
                if (table != null)
                {
                    table.IsDeleted = true;
                    table.UpdatedBy = deletorId;
                    table.UpdatedAt = DateTime.Now;
                    tablesToDelete.Add(table);
                }
                if(table == null || table.IsOccupied){
                    return Response<bool>.FailureResponse(MessageConstants.TableNotDeleteMessage);
                }
            }
            await _tableRepository.UpdateRangeAsync(tablesToDelete);
            return Response<bool>.SuccessResponse(true, "Selected Tables " + MessageConstants.DeleteMessage);
        });
    }

    #endregion Tables
}
