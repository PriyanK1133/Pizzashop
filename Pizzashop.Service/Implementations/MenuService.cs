using Pizzashop.Entity.Constants;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Helper;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class MenuService : IMenuService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IModifierAndGroupRepository _modifierAndGroupRepository;
    private readonly IModifierGroupRepository _modifierGroupRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IModifierRepository _modifierRepository;
    private readonly IItemAndModifierGroupRepository _itemAndModifierGroupRepository;

    public MenuService(ICategoryRepository categoryRepository, IItemRepository itemRepository, IModifierAndGroupRepository modifierAndGroupRepository, IModifierGroupRepository modifierGroupRepository, IUnitRepository unitRepository, IModifierRepository modifierRepository, IItemAndModifierGroupRepository itemAndModifierGroupRepository)
    {
        _categoryRepository = categoryRepository;
        _itemRepository = itemRepository;
        _modifierAndGroupRepository = modifierAndGroupRepository;
        _modifierGroupRepository = modifierGroupRepository;
        _unitRepository = unitRepository;
        _modifierRepository = modifierRepository;
        _itemAndModifierGroupRepository = itemAndModifierGroupRepository;
    }

    #region  Categories

    public async Task<Response<IEnumerable<CategoryVM>>> GetAllCategoriesAsync()
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            IEnumerable<Category> categories = await _categoryRepository.GetAllAsync();
            IEnumerable<CategoryVM> categoryList = categories.Select(c => new CategoryVM()
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToList();

            return Response<IEnumerable<CategoryVM>>.SuccessResponse(categoryList, "Category " + MessageConstants.GetMessage);
        });
    }

    public async Task<Response<CategoryVM?>> GetCategoryByIdAsync(Guid id)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
       {
           Category? category = await _categoryRepository.GetByIdAsync(id);
           if (category == null)
           {
               return Response<CategoryVM?>.FailureResponse("Category " + MessageConstants.NotFoundMessage);
           }

           CategoryVM categoryVM = new()
           {
               Id = category.Id,
               Name = category.Name,
               Description = category.Description,
               CreatedBy = category.CreatedBy,
               UpdatedBy = category.UpdatedBy
           };

           return Response<CategoryVM?>.SuccessResponse(categoryVM, "Category " + MessageConstants.GetMessage);
       });
    }

    public async Task<Response<CategoryVM?>> AddCategoryAsync(CategoryVM model)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            bool isExist = await _categoryRepository.IsExistsAsync(c => c.Name.ToLower().Trim() == model.Name.ToLower().Trim() && !c.IsDeleted);

            if (isExist)
            {
                return Response<CategoryVM?>.FailureResponse("Category " + MessageConstants.AlreadyExistMessage);
            }

            Category category = new()
            {
                Name = model.Name,
                Description = model.Description,
                CreatedBy = model.CreatedBy,
                UpdatedBy = model.UpdatedBy
            };
            await _categoryRepository.AddAsync(category);
            return Response<CategoryVM?>.SuccessResponse(model, "Category " + MessageConstants.CreateMessage);
        });
    }

    public async Task<Response<CategoryVM?>> EditCategoryAsync(CategoryVM model)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Category? category = await _categoryRepository.GetByIdAsync(model.Id);
            if (category == null)
            {
                return Response<CategoryVM?>.FailureResponse("Category " + MessageConstants.NotFoundMessage);
            }

            bool isExist = await _categoryRepository.IsExistsAsync(c => c.Name.ToLower().Trim() == model.Name.ToLower().Trim() && !c.IsDeleted && c.Id != model.Id);
            if (isExist)
            {
                return Response<CategoryVM?>.FailureResponse("Category " + MessageConstants.AlreadyExistMessage);
            }

            category.Id = model.Id;
            category.Name = model.Name;
            category.Description = model.Description;
            category.UpdatedAt = DateTime.Now;
            category.UpdatedBy = model.UpdatedBy;

            await _categoryRepository.UpdateAsync(category);
            return Response<CategoryVM?>.SuccessResponse(model, "Category " + MessageConstants.EditMessage);
        });
    }

    public async Task<Response<bool>> DeleteCategoryAsync(Guid id, Guid deletorId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Category? category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return Response<bool>.FailureResponse("Category " + MessageConstants.NotFoundMessage);
            }

            IEnumerable<Item> itemsToDelete = await _itemRepository.GetAllByCategoryIdAsync(id);
            foreach (Item item in itemsToDelete)
            {
                item.IsDeleted = true;
                item.UpdatedBy = deletorId;
                item.UpdatedAt = DateTime.Now;
            }
            await _itemRepository.UpdateRangeAsync(itemsToDelete);

            category.IsDeleted = true;
            category.UpdatedBy = deletorId;
            category.UpdatedAt = DateTime.Now;
            await _categoryRepository.UpdateAsync(category);


            return Response<bool>.SuccessResponse(true, "Category " + MessageConstants.DeleteMessage);
        });

    }

    public async Task<Response<bool>> ChangeCategoriesOrder(List<Guid> sortedIds)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            List<Category> categoriesToUpdate = new();
            for (int i = 0; i < sortedIds.Count; i++)
            {
                Category? category = await _categoryRepository.GetByIdAsync(sortedIds[i]);

                if (category != null)
                {
                    category.Preference = i + 1;
                    categoriesToUpdate.Add(category);
                }
            }

            await _categoryRepository.UpdateRangeAsync(categoriesToUpdate);

            return Response<bool>.SuccessResponse(true, "Category Order " + MessageConstants.EditMessage);
        });
    }

    #endregion

    #region  Items

    public async Task<Response<IEnumerable<ItemListVM>>> GetItemsByCategoryIdAsync(Guid categoryId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
       {

           IEnumerable<Item> items;
           if (categoryId != Guid.Empty)
           {
               items = await _itemRepository.GetAllByCategoryIdAsync(categoryId);
           }
           else
           {
               items = await _itemRepository.GetAllItemsAsync();
           }

           IEnumerable<ItemListVM> itemlist = items.Select(i => new ItemListVM()
           {
               Id = i.Id,
               Name = i.Name,
               Type = i.Type,
               Rate = i.Rate,
               Quantity = i.Quantity,
               Image = i.Image,
               IsAvailable = i.IsAvailable,
               IsFavourite = i.IsFavourite
           }).ToList();
           return Response<IEnumerable<ItemListVM>>.SuccessResponse(itemlist, "Item list " + MessageConstants.GetMessage);
       });
    }

    public async Task<Response<IEnumerable<ItemListVM>>> GetAvailableItemsByCategoryIdAsync(Guid categoryId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
       {

           IEnumerable<Item> items;
           if (categoryId != Guid.Empty)
           {
               items = await _itemRepository.GetAvailableByCategoryIdAsync(categoryId);
           }
           else
           {
               items = await _itemRepository.GetAllAvailableAsync();
           }

           IEnumerable<ItemListVM> itemlist = items.Select(i => new ItemListVM()
           {
               Id = i.Id,
               Name = i.Name,
               Type = i.Type,
               Rate = i.Rate,
               Quantity = i.Quantity,
               Image = i.Image,
               IsAvailable = i.IsAvailable,
               IsFavourite = i.IsFavourite,
               TaxPercentage = i.TaxPercentage
           }).ToList();
           return Response<IEnumerable<ItemListVM>>.SuccessResponse(itemlist, "Item list " + MessageConstants.GetMessage);
       });
    }

    public async Task<Response<IEnumerable<ItemListVM>>> GetFavoriteItemsAsync()
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            IEnumerable<Item> items = await _itemRepository.GetFavoriteItemsAsync();

            IEnumerable<ItemListVM> itemlist = items.Select(i => new ItemListVM()
            {
                Id = i.Id,
                Name = i.Name,
                Type = i.Type,
                Rate = i.Rate,
                Quantity = i.Quantity,
                Image = i.Image,
                IsAvailable = i.IsAvailable,
                IsFavourite = i.IsFavourite,
                TaxPercentage = i.TaxPercentage
            }).ToList();
            return Response<IEnumerable<ItemListVM>>.SuccessResponse(itemlist, "Favourite Items " + MessageConstants.GetMessage);
        });
    }

    public async Task<Response<PagedResult<ItemListVM>>> GetPagedItemsAsync(Guid categoryId, string searchString, int page, int pagesize, bool isASC)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
      {
          (IEnumerable<Item> items, int totalRecords) = await _itemRepository.GetPagedItemsAsync(categoryId, searchString, page, pagesize, isASC);

          IEnumerable<ItemListVM> itemlist = items.Select(i => new ItemListVM()
          {
              Id = i.Id,
              Name = i.Name,
              Type = i.Type,
              Rate = i.Rate,
              Quantity = i.Quantity,
              Image = i.Image,
              IsAvailable = i.IsAvailable
          }).ToList();

          PagedResult<ItemListVM> pagedResult = new()
          {
              PagedList = itemlist
          };

          pagedResult.Pagination.SetPagination(totalRecords, pagesize, page);

          return Response<PagedResult<ItemListVM>>.SuccessResponse(pagedResult, "Item list " + MessageConstants.GetMessage);
      });
    }

    public async Task<Response<IEnumerable<ModifierGroupForItemVM>>> GetApplicableModifiersForItem(Guid itemId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            IEnumerable<ItemsAndModifierGroup> itemsAndModifierGroups = await _itemAndModifierGroupRepository.GetApplicableModifierByItemId(itemId);

            IEnumerable<ModifierGroupForItemVM> modifierGroupsForItem = itemsAndModifierGroups.Select(mg => new ModifierGroupForItemVM()
            {
                Id = mg.ModifierGroupId,
                Name = mg.ModifierGroup.Name,
                MinSelection = mg.MinSelection,
                MaxSelection = mg.MaxSelection,
                Modifiers = mg.ModifierGroup.ModifiersAndGroups.Select(m => new ModifierListVM()
                {
                    Id = m.Modifier.Id,
                    Name = m.Modifier.Name,
                    Rate = m.Modifier.Rate
                }).ToList(),
            });

            return Response<IEnumerable<ModifierGroupForItemVM>>.SuccessResponse(modifierGroupsForItem, "Modifier Groups For Item " + MessageConstants.GetMessage);
        });
    }

    public async Task<Response<ItemVM?>> GetItemByIdAsync(Guid id)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Item? item = await _itemRepository.GetByIdAsync(id);
            if (item == null)
            {
                return Response<ItemVM?>.FailureResponse("Item " + MessageConstants.NotFoundMessage);
            }

            ItemVM itemVM = new()
            {
                Id = item.Id,
                CategoryId = item.CategoryId,
                Name = item.Name,
                Type = item.Type,
                Rate = item.Rate,
                Quantity = item.Quantity,
                UnitId = item.UnitId,
                Shortcode = item.Shortcode,
                Description = item.Description,
                Image = item.Image,
                IsDefaultTax = item.IsDefaultTax.GetValueOrDefault(),
                TaxPercentage = item.TaxPercentage,
                IsFavourite = item.IsFavourite,
                IsAvailable = item.IsAvailable,
                CreatedBy = item.CreatedBy,
                UpdatedBy = item.UpdatedBy
            };

            IEnumerable<ItemsAndModifierGroup> modifierGroupsForItem = await _itemAndModifierGroupRepository.GetByItemId(item.Id);

            itemVM.SelectedModifierGroups = modifierGroupsForItem.Select(mg => new ModifierGroupForItemVM()
            {
                Id = mg.ModifierGroupId,
                MinSelection = mg.MinSelection,
                MaxSelection = mg.MaxSelection
            }).ToList();

            return Response<ItemVM?>.SuccessResponse(itemVM, "Item " + MessageConstants.GetMessage);
        });
    }

    public async Task<Response<ItemVM?>> AddItemAsync(ItemVM model)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Item item = new()
            {
                CategoryId = model.CategoryId.GetValueOrDefault(),
                Name = model.Name,
                Type = model.Type,
                Rate = model.Rate.GetValueOrDefault(),
                Quantity = model.Quantity.GetValueOrDefault(),
                UnitId = model.UnitId.GetValueOrDefault(),
                Shortcode = model.Shortcode,
                Description = model.Description,
                Image = model.Image,
                IsDefaultTax = model.IsDefaultTax,
                TaxPercentage = model.TaxPercentage.GetValueOrDefault(),
                IsFavourite = model.IsFavourite,
                IsAvailable = model.IsAvailable,
                CreatedBy = model.CreatedBy,
                UpdatedBy = model.UpdatedBy,
            };
            await _itemRepository.AddAsync(item);

            List<ItemsAndModifierGroup> modifierGroupsForItem = model.SelectedModifierGroups.Select(mg => new ItemsAndModifierGroup()
            {
                ItemId = item.Id,
                ModifierGroupId = mg.Id,
                MinSelection = mg.MinSelection,
                MaxSelection = mg.MaxSelection,
                CreatedBy = model.CreatedBy,
                UpdatedBy = model.UpdatedBy
            }).ToList();

            await _itemAndModifierGroupRepository.AddRangeAsync(modifierGroupsForItem);

            return Response<ItemVM?>.SuccessResponse(model, "Item " + MessageConstants.CreateMessage);
        });
    }

    public async Task<Response<ItemVM?>> EditItemAsync(ItemVM model)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Item? item = await _itemRepository.GetByIdAsync(model.Id);
            if (item == null)
            {
                return Response<ItemVM?>.FailureResponse("Item " + MessageConstants.NotFoundMessage);
            }

            item.CategoryId = model.CategoryId.GetValueOrDefault();
            item.Name = model.Name;
            item.Type = model.Type;
            item.Rate = model.Rate.GetValueOrDefault();
            item.Quantity = model.Quantity.GetValueOrDefault();
            item.UnitId = model.UnitId.GetValueOrDefault();
            item.Shortcode = model.Shortcode;
            item.Description = model.Description;
            item.Image = model.Image;
            item.IsAvailable = model.IsAvailable;
            item.IsDefaultTax = model.IsDefaultTax;
            item.TaxPercentage = model.TaxPercentage.GetValueOrDefault();
            item.IsFavourite = model.IsFavourite;
            item.UpdatedBy = model.UpdatedBy;
            item.UpdatedAt = DateTime.Now;

            IEnumerable<ItemsAndModifierGroup> existingModifierGroupsForItem = await _itemAndModifierGroupRepository.GetByItemId(model.Id);

            List<ItemsAndModifierGroup> modifierGroupsForItemToAdd = new();
            List<ItemsAndModifierGroup> modifierGroupsForItemToRemove = new();
            List<ItemsAndModifierGroup> modifierGroupsForItemToUpdate = new();

            foreach (ModifierGroupForItemVM modifierGroupForItem in model.SelectedModifierGroups)
            {
                if (!existingModifierGroupsForItem.Any(mg => modifierGroupForItem.Id == mg.ModifierGroupId))
                {
                    ItemsAndModifierGroup m = new()
                    {
                        ItemId = item.Id,
                        ModifierGroupId = modifierGroupForItem.Id,
                        MinSelection = modifierGroupForItem.MinSelection,
                        MaxSelection = modifierGroupForItem.MaxSelection,
                        CreatedBy = model.UpdatedBy,
                        UpdatedBy = model.UpdatedBy
                    };

                    modifierGroupsForItemToAdd.Add(m);
                }
            }

            foreach (ItemsAndModifierGroup itemsAndModifierGroup in existingModifierGroupsForItem)
            {
                if (!model.SelectedModifierGroups.Any(mg => mg.Id == itemsAndModifierGroup.ModifierGroupId))
                {
                    modifierGroupsForItemToRemove.Add(itemsAndModifierGroup);
                }
                else
                {
                    ModifierGroupForItemVM updatedData = model.SelectedModifierGroups.Single(mg => mg.Id == itemsAndModifierGroup.ModifierGroupId);
                    itemsAndModifierGroup.MinSelection = updatedData.MinSelection;
                    itemsAndModifierGroup.MaxSelection = updatedData.MaxSelection;

                    modifierGroupsForItemToUpdate.Add(itemsAndModifierGroup);
                }
            }

            await _itemAndModifierGroupRepository.AddRangeAsync(modifierGroupsForItemToAdd);
            await _itemAndModifierGroupRepository.RemoveRangeAsync(modifierGroupsForItemToRemove);
            await _itemAndModifierGroupRepository.UpdateRangeAsync(modifierGroupsForItemToUpdate);

            await _itemRepository.UpdateAsync(item);
            return Response<ItemVM?>.SuccessResponse(model, "Item " + MessageConstants.EditMessage);
        });


    }

    public async Task<Response<bool>> DeleteItemAsync(Guid id, Guid deletorId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Item? item = await _itemRepository.GetByIdAsync(id);
            if (item == null)
            {
                return Response<bool>.FailureResponse("Item " + MessageConstants.NotFoundMessage);
            }

            item.IsDeleted = true;
            item.UpdatedBy = deletorId;
            item.UpdatedAt = DateTime.Now;

            List<ItemsAndModifierGroup> itemsAndModifierGroups = (await _itemAndModifierGroupRepository.GetByItemId(id)).ToList();
            await _itemAndModifierGroupRepository.RemoveRangeAsync(itemsAndModifierGroups);

            await _itemRepository.UpdateAsync(item);
            return Response<bool>.SuccessResponse(true, "Item " + MessageConstants.DeleteMessage);
        });
    }

    public async Task<Response<bool>> DeleteManyItemAsync(IEnumerable<Guid> ids, Guid deletorId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            List<Item> itemsToDelete = new();
            foreach (Guid id in ids)
            {
                Item? item = await _itemRepository.GetByIdAsync(id);
                if (item != null)
                {
                    item.IsDeleted = true;
                    item.UpdatedBy = deletorId;
                    item.UpdatedAt = DateTime.Now;
                    itemsToDelete.Add(item);
                }
            }
            await _itemRepository.UpdateRangeAsync(itemsToDelete);
            return Response<bool>.SuccessResponse(true, "Selected Items " + MessageConstants.DeleteMessage);
        });
    }

    public async Task<Response<bool>> ToggleFavoriteItemAsync(Guid itemId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Item? item = await _itemRepository.GetByIdAsync(itemId);
            if (item == null)
            {
                return Response<bool>.FailureResponse("Item " + MessageConstants.NotFoundMessage);
            }

            item.IsFavourite = !item.IsFavourite;
            await _itemRepository.UpdateAsync(item);

            return Response<bool>.SuccessResponse(true, "Item Status " + MessageConstants.EditMessage);
        });
    }

    #endregion

    #region  ModifierGroups

    public async Task<Response<ModifierGroupVM?>> GetModifierGroupByIdAsync(Guid id)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
       {
           ModifierGroup? modifierGroup = await _modifierGroupRepository.GetByIdAsync(id);
           if (modifierGroup == null)
           {
               return Response<ModifierGroupVM?>.FailureResponse("Modifier Group " + MessageConstants.NotFoundMessage);
           }

           ModifierGroupVM modifierGroupVM = new()
           {
               Id = modifierGroup.Id,
               Name = modifierGroup.Name,
               Description = modifierGroup.Description,
               CreatedBy = modifierGroup.CreatedBy,
               UpdatedBy = modifierGroup.UpdatedBy
           };

           return Response<ModifierGroupVM?>.SuccessResponse(modifierGroupVM, "Modifier Group " + MessageConstants.GetMessage);
       });
    }

    public async Task<Response<IEnumerable<ModifierGroupVM>>> GetModifierGroupsByModifierIdAsync(Guid modifierId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
       {
           IEnumerable<ModifierGroup> modifierGroups = await _modifierAndGroupRepository.GetModifierGroupsByModifierIdAsync(modifierId);
           IEnumerable<ModifierGroupVM> modifierGroupList = modifierGroups.Select(mg => new ModifierGroupVM()
           {
               Id = mg.Id,
               Name = mg.Name,
               Description = mg.Description,
               CreatedBy = mg.CreatedBy,
               UpdatedBy = mg.UpdatedBy
           });
           return Response<IEnumerable<ModifierGroupVM>>.SuccessResponse(modifierGroupList, "Modifier List " + MessageConstants.GetMessage);
       });
    }

    public async Task<Response<IEnumerable<ModifierGroupVM>>> GetAllModifierGroupsAsync()
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            IEnumerable<ModifierGroup> modifierGroups = await _modifierGroupRepository.GetAllAsync();
            IEnumerable<ModifierGroupVM> modifierGroupList = modifierGroups.Select(c => new ModifierGroupVM()
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToList();

            return Response<IEnumerable<ModifierGroupVM>>.SuccessResponse(modifierGroupList, "Modifier Groups " + MessageConstants.GetMessage);

        });
    }

    public async Task<Response<ModifierGroupVM?>> AddModifierGroupAsync(ModifierGroupVM model, List<Guid> modifierIds)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {

            bool isExist = await _modifierGroupRepository.IsExistsAsync(mg => mg.Name.ToLower().Trim() == model.Name.ToLower().Trim() && !mg.IsDeleted);
            if (isExist)
            {
                return Response<ModifierGroupVM?>.FailureResponse("Modifier Group " + MessageConstants.AlreadyExistMessage);
            }

            ModifierGroup modifierGroup = new()
            {
                // Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                CreatedBy = model.CreatedBy,
                UpdatedBy = model.UpdatedBy
            };
            await _modifierGroupRepository.AddAsync(modifierGroup);


            List<ModifiersAndGroup> modifiersAndGroups = new();

            foreach (Guid id in modifierIds)
            {
                ModifiersAndGroup modifierAndGroup = new()
                {
                    ModifierGroupId = modifierGroup.Id,
                    ModifierId = id,
                    CreatedBy = model.CreatedBy,
                    UpdatedBy = model.UpdatedBy
                };

                modifiersAndGroups.Add(modifierAndGroup);
            }

            await _modifierAndGroupRepository.AddRangeAsync(modifiersAndGroups);
            return Response<ModifierGroupVM?>.SuccessResponse(model, "Modifier Group " + MessageConstants.CreateMessage);
        });
    }

    public async Task<Response<ModifierGroupVM?>> EditModifierGroupAsync(ModifierGroupVM model, List<Guid> modifierIds)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            ModifierGroup? modifierGroup = await _modifierGroupRepository.GetByIdAsync(model.Id);
            if (modifierGroup == null)
            {
                return Response<ModifierGroupVM?>.FailureResponse("Modifier Group" + MessageConstants.NotFoundMessage);
            }

             bool isExist = await _modifierGroupRepository.IsExistsAsync(mg => mg.Name.ToLower().Trim() == model.Name.ToLower().Trim() && !mg.IsDeleted && mg.Id != model.Id);
            if (isExist)
            {
                return Response<ModifierGroupVM?>.FailureResponse("Modifier Group " + MessageConstants.AlreadyExistMessage);
            }

            modifierGroup.Name = model.Name;
            modifierGroup.Description = model.Description;
            modifierGroup.UpdatedBy = model.UpdatedBy;
            modifierGroup.UpdatedAt = DateTime.Now;

            IEnumerable<ModifiersAndGroup> existingModifiers = await _modifierAndGroupRepository.GetModifiersAndGroupByGroupIdAsync(model.Id);

            List<ModifiersAndGroup> modifiersToAddInGroup = new();
            List<ModifiersAndGroup> modifiersToDeleteInGroup = new();

            foreach (Guid id in modifierIds)
            {

                if (!existingModifiers.Any(mg => mg.ModifierId == id))
                {
                    ModifiersAndGroup modifiersAndGroup = new()
                    {
                        ModifierGroupId = modifierGroup.Id,
                        ModifierId = id,
                        CreatedBy = model.UpdatedBy,
                        UpdatedBy = model.UpdatedBy
                    };

                    modifiersToAddInGroup.Add(modifiersAndGroup);
                }
            }

            foreach (ModifiersAndGroup modifier in existingModifiers)
            {
                if (!modifierIds.Any(id => id == modifier.ModifierId))
                {
                    modifiersToDeleteInGroup.Add(modifier);
                }
            }

            await _modifierAndGroupRepository.AddRangeAsync(modifiersToAddInGroup);
            await _modifierAndGroupRepository.DeleteRangeAsync(modifiersToDeleteInGroup);

            await _modifierGroupRepository.UpdateAsync(modifierGroup);

            return Response<ModifierGroupVM?>.SuccessResponse(model, "Modifier Group " + MessageConstants.EditMessage);

        });
    }

    public async Task<Response<bool>> DeleteModifierGroupAsync(Guid id, Guid deletorId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            ModifierGroup? modifierGroup = await _modifierGroupRepository.GetByIdAsync(id);
            if (modifierGroup == null)
            {
                return Response<bool>.FailureResponse("Modifier Group " + MessageConstants.NotFoundMessage);
            }

            IEnumerable<ModifiersAndGroup> modifiersAndGroups = await _modifierAndGroupRepository.GetModifiersAndGroupByGroupIdAsync(id);

            await _modifierAndGroupRepository.DeleteRangeAsync(modifiersAndGroups);

            modifierGroup.IsDeleted = true;
            modifierGroup.UpdatedAt = DateTime.Now;
            modifierGroup.UpdatedBy = deletorId;



            await _modifierGroupRepository.UpdateAsync(modifierGroup);

            return Response<bool>.SuccessResponse(true, "Modifier Group " + MessageConstants.DeleteMessage);
        });
    }

    public async Task<Response<bool>> ChangeModifierGroupsOrder(List<Guid> sortedIds)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            List<ModifierGroup> modifierGroupsToUpdate = new();
            for (int i = 0; i < sortedIds.Count; i++)
            {
                ModifierGroup? modifierGroup = await _modifierGroupRepository.GetByIdAsync(sortedIds[i]);

                if (modifierGroup != null)
                {
                    modifierGroup.Preference = i + 1;
                    modifierGroupsToUpdate.Add(modifierGroup);
                }
            }

            await _modifierGroupRepository.UpdateRangeAsync(modifierGroupsToUpdate);

            return Response<bool>.SuccessResponse(true, "Modifier Groups Order " + MessageConstants.EditMessage);
        });
    }


    #endregion

    #region  Modifiers

    public async Task<Response<IEnumerable<ModifierListVM>>> GetModifiersByGroupIdAsync(Guid modifierGroupId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
       {
           IEnumerable<Modifier> modifiers = await _modifierAndGroupRepository.GetModifiersByGroupIdAsync(modifierGroupId);
           IEnumerable<ModifierListVM> modifierlist = modifiers.Select(m => new ModifierListVM()
           {
               Id = m.Id,
               Name = m.Name,
               Rate = m.Rate,
               Quantity = m.Quantity,
               Unit = m.Unit.Name
           });
           return Response<IEnumerable<ModifierListVM>>.SuccessResponse(modifierlist, "Modifier List " + MessageConstants.GetMessage);
       });
    }

    public async Task<Response<IEnumerable<ModifierListVM>>> GetAllModifiersAsync()
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            IEnumerable<Modifier> modifiers = await _modifierRepository.GetAllAsync();
            IEnumerable<ModifierListVM> modifierLists = modifiers.Select(m => new ModifierListVM()
            {
                Id = m.Id,
                Name = m.Name,
                Unit = m.Unit.Name,
                Rate = m.Rate,
                Quantity = m.Quantity
            }).ToList();

            return Response<IEnumerable<ModifierListVM>>.SuccessResponse(modifierLists, "Modifiers " + MessageConstants.GetMessage);
        });
    }

    public async Task<Response<PagedResult<ModifierListVM>>> GetAllPagedModifiersAsync(string searchString, int page, int pagesize, bool isASC)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
      {
          (IEnumerable<Modifier> modifiers, int totalRecords) = await _modifierRepository.GetAllPagedModifiersAsync(searchString, page, pagesize, isASC);

          IEnumerable<ModifierListVM> modifierlist = modifiers.Select(m => new ModifierListVM()
          {
              Id = m.Id,
              Name = m.Name,
              Rate = m.Rate,
              Quantity = m.Quantity,
              Unit = m.Unit.Name
          }).ToList();

          PagedResult<ModifierListVM> pagedResult = new()
          {
              PagedList = modifierlist
          };

          pagedResult.Pagination.SetPagination(totalRecords, pagesize, page);

          return Response<PagedResult<ModifierListVM>>.SuccessResponse(pagedResult, "Modifier list " + MessageConstants.GetMessage);
      });
    }

    public async Task<Response<PagedResult<ModifierListVM>>> GetPagedModifiersAsync(Guid modifierGroupId, string searchString, int page, int pagesize, bool isASC)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
      {
          (IEnumerable<Modifier> modifiers, int totalRecords) = await _modifierAndGroupRepository.GetPagedModifiersAsync(modifierGroupId, searchString, page, pagesize, isASC);

          IEnumerable<ModifierListVM> modifierlist = modifiers.Select(m => new ModifierListVM()
          {
              Id = m.Id,
              Name = m.Name,
              Rate = m.Rate,
              Quantity = m.Quantity,
              Unit = m.Unit.Name
          }).ToList();

          PagedResult<ModifierListVM> pagedResult = new()
          {
              PagedList = modifierlist
          };

          pagedResult.Pagination.SetPagination(totalRecords, pagesize, page);

          return Response<PagedResult<ModifierListVM>>.SuccessResponse(pagedResult, "Modifier list fetched successfully!");
      });
    }

    public async Task<Response<ModifierVM?>> GetModifierByIdAsync(Guid id)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Modifier? modifier = await _modifierRepository.GetByIdAsync(id);
            if (modifier == null)
            {
                return Response<ModifierVM?>.FailureResponse("Modifier " + MessageConstants.NotFoundMessage);
            }

            ModifierVM modifierVM = new()
            {
                Id = modifier.Id,
                Name = modifier.Name,
                Rate = modifier.Rate,
                Quantity = modifier.Quantity,
                UnitId = modifier.UnitId,
                Description = modifier.Description
            };

            return Response<ModifierVM?>.SuccessResponse(modifierVM, "Modifier " + MessageConstants.GetMessage);
        });
    }

    public async Task<Response<ModifierVM?>> AddModifierAsync(ModifierVM model, List<Guid> modifierGroups)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Modifier modifier = new()
            {
                Name = model.Name,
                Rate = model.Rate.GetValueOrDefault(),
                Quantity = model.Quantity.GetValueOrDefault(),
                UnitId = model.UnitId.GetValueOrDefault(),
                Description = model.Description,
                CreatedBy = model.CreatedBy,
                UpdatedBy = model.UpdatedBy
            };

            await _modifierRepository.AddAsync(modifier);

            List<ModifiersAndGroup> modifiersAndGroupsToAdd = new();
            foreach (Guid id in modifierGroups)
            {
                ModifiersAndGroup modifiersAndGroup = new()
                {
                    ModifierGroupId = id,
                    ModifierId = modifier.Id,
                    CreatedBy = model.CreatedBy,
                    UpdatedBy = model.UpdatedBy
                };

                modifiersAndGroupsToAdd.Add(modifiersAndGroup);
            }

            await _modifierAndGroupRepository.AddRangeAsync(modifiersAndGroupsToAdd);

            return Response<ModifierVM?>.SuccessResponse(model, "Modifier " + MessageConstants.CreateMessage);
        });
    }

    public async Task<Response<ModifierVM?>> EditModifierAsync(ModifierVM model, List<Guid> modifierGroups)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Modifier? modifier = await _modifierRepository.GetByIdAsync(model.Id);
            if (modifier == null)
            {
                return Response<ModifierVM?>.FailureResponse("Modifier " + MessageConstants.NotFoundMessage);
            }

            modifier.Name = model.Name;
            modifier.Rate = model.Rate.GetValueOrDefault();
            modifier.Quantity = model.Quantity.GetValueOrDefault();
            modifier.UnitId = model.UnitId.GetValueOrDefault();
            modifier.Description = model.Description;
            modifier.UpdatedBy = model.UpdatedBy;
            modifier.UpdatedAt = DateTime.Now;

            IEnumerable<ModifiersAndGroup> existingModifierGroups = await _modifierAndGroupRepository.GetModifiersAndGroupByModifierIdAsync(model.Id);

            List<ModifiersAndGroup> modifierGroupsToAddInGroup = new();
            List<ModifiersAndGroup> modifierGroupsToDeleteInGroup = new();

            foreach (Guid id in modifierGroups)
            {

                if (!existingModifierGroups.Any(mg => mg.ModifierGroupId == id))
                {
                    ModifiersAndGroup modifiersAndGroup = new()
                    {
                        ModifierGroupId = id,
                        ModifierId = modifier.Id,
                        CreatedBy = model.UpdatedBy,
                        UpdatedBy = model.UpdatedBy
                    };

                    modifierGroupsToAddInGroup.Add(modifiersAndGroup);
                }
            }

            foreach (ModifiersAndGroup modifierAndGroup in existingModifierGroups)
            {
                if (!modifierGroups.Any(id => id == modifierAndGroup.ModifierGroupId))
                {
                    modifierGroupsToDeleteInGroup.Add(modifierAndGroup);
                }
            }

            await _modifierAndGroupRepository.AddRangeAsync(modifierGroupsToAddInGroup);
            await _modifierAndGroupRepository.DeleteRangeAsync(modifierGroupsToDeleteInGroup);

            await _modifierRepository.UpdateAsync(modifier);

            return Response<ModifierVM?>.SuccessResponse(model, "Modifier " + MessageConstants.EditMessage);
        });
    }

    public async Task<Response<bool>> DeleteModifierAsync(Guid id, Guid deletorId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            Modifier? modifier = await _modifierRepository.GetByIdAsync(id);
            if (modifier == null)
            {
                return Response<bool>.FailureResponse("Modifier " + MessageConstants.NotFoundMessage);
            }

            IEnumerable<ModifiersAndGroup> modifiersAndGroups = await _modifierAndGroupRepository.GetModifiersAndGroupByModifierIdAsync(id);

            await _modifierAndGroupRepository.DeleteRangeAsync(modifiersAndGroups);

            modifier.IsDeleted = true;
            modifier.UpdatedAt = DateTime.Now;
            modifier.UpdatedBy = deletorId;



            await _modifierRepository.UpdateAsync(modifier);

            return Response<bool>.SuccessResponse(true, "Modifier " + MessageConstants.DeleteMessage);
        });
    }

    public async Task<Response<bool>> DeleteManyModifierAsync(IEnumerable<Guid> ids, Guid deletorId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
       {
           foreach (Guid id in ids)
           {
               Response<bool> response = await DeleteModifierAsync(id, deletorId);
           }

           return Response<bool>.SuccessResponse(true, "Selected Modifiers " + MessageConstants.DeleteMessage);
       });
    }

    #endregion

    #region  Units
    public async Task<Response<IEnumerable<UnitVM>>> GetAllUnitsAsync()
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            IEnumerable<Unit> units = await _unitRepository.GetAllAsync();
            IEnumerable<UnitVM> unitlist = units.Select(u => new UnitVM()
            {
                Id = u.Id,
                Name = u.Name,
                Shortname = u.Shortname
            }).ToList();

            return Response<IEnumerable<UnitVM>>.SuccessResponse(unitlist, "Unit list " + MessageConstants.GetMessage);
        });
    }

    #endregion
}
