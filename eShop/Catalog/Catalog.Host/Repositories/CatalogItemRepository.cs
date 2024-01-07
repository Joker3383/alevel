using Catalog.Host.Data;
using Catalog.Host.Data.Entity;
using Catalog.Host.Repositories.Interfaces;
using Catalog.Host.Services.Interfaces;
using Catalog.Host.Services.Interfaces.AddResponses;
using Catalog.Host.Services.Interfaces.DeleteRequests;
using Catalog.Host.Services.Interfaces.UpdateRequests;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Host.Repositories;

public class CatalogItemRepository : ICatalogItemRepository
{
        private readonly AppDbContext _dbContext;
        private readonly ILogger<CatalogItemRepository> _logger;

        public CatalogItemRepository(
            IDbContextWrapper<AppDbContext> dbContextWrapper,
            ILogger<CatalogItemRepository> logger)
        {
            _dbContext = dbContextWrapper.DbContext;
            _logger = logger;
        }


        public async Task<PaginatedItems<CatalogItem>> GetByPageAsyncHttpGet(int pageIndex, int pageSize)
        {
            var totalItems = await _dbContext.CatalogItems.LongCountAsync();

            var catalogItems = await _dbContext.CatalogItems
                .Include(item => item.CatalogBrand)
                .Include(item => item.CatalogType)
                .OrderBy(x => x.Id)
                .Skip(pageSize * (pageIndex - 1))
                .Take(pageSize)
                .ToListAsync();


            return new PaginatedItems<CatalogItem>
            {
                TotalCount = totalItems,
                Data = catalogItems
            };
        }

        public async Task<PaginatedItems<CatalogItem>> GetItemsByPageAsync(PaginatedItemsRequest request)
        {
            request.PageIndex = request.PageIndex <= 0 ? 1 : request.PageIndex;
            request.PageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            var query = _dbContext.CatalogItems
                .Include(item => item.CatalogBrand)
                .Include(item => item.CatalogType)
                .AsQueryable();

            if (request != null && request.BrandIds != null && request.BrandIds.Any())
            {
                query = query.Where(item => request.BrandIds.Any(brandId => brandId == item.CatalogBrandId));
            }

            if (request.TypeIds != null && request.TypeIds.Any())
            {
                query = query.Where(item => request.TypeIds.Any(typeId => typeId == item.CatalogTypeId));
            }
 
            var totalItems = await query.LongCountAsync();

            var catalogItems = await query
                .OrderBy(c => c.Name)
                .Skip(request.PageSize * (request.PageIndex - 1))
                .Take(request.PageSize)
                .ToListAsync();

            return new PaginatedItems<CatalogItem>
            {
                TotalCount = totalItems,
                Data = catalogItems
            };
        }

        public async Task<CatalogItem> GetItemByIdAsync(Guid id)
        {
            return await _dbContext.CatalogItems.FindAsync(id);
        }

        public async Task<IEnumerable<CatalogItem>> GetItemsByBrandAsync(Guid brandId)
        {
            return await _dbContext.CatalogItems
               .Include(item => item.CatalogBrand)
               .Include(item => item.CatalogType)
               .Where(item => item.CatalogBrandId == brandId)
               .ToListAsync();
        }

        public async Task<IEnumerable<CatalogItem>> GetItemsByTypeAsync(Guid typeId)
        {
            return await _dbContext.CatalogItems
               .Include(item => item.CatalogBrand)
               .Include(item => item.CatalogType)
               .Where(item => item.CatalogTypeId == typeId)
               .ToListAsync();
        }

        public async Task<Guid?> AddAsync(AddCatalogItemRequest itemToAdd)
        {
            var item = await _dbContext.AddAsync(new CatalogItem
            {
                CatalogBrandId = itemToAdd.CatalogBrandId,
                CatalogTypeId = itemToAdd.CatalogTypeId,
                Description = itemToAdd.Description,
                Name = itemToAdd.Name,
                PictureFileName = itemToAdd.PictureFileName,
                Price = itemToAdd.Price
            });

            await _dbContext.SaveChangesAsync();
            
            return item.Entity.Id;
        }

        public async Task<CatalogItem> UpdateAsync(UpdateCatalogItemRequest itemToUpdate)
        {
            var item = await _dbContext.CatalogItems.AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == itemToUpdate.Id);

            if (item == null)
            {
                throw new KeyNotFoundException("Item not found");
            }

            item.CatalogBrandId = itemToUpdate.CatalogBrandId;
            item.CatalogTypeId = itemToUpdate.CatalogTypeId;
            item.Description = itemToUpdate.Description;
            item.Name = itemToUpdate.Name;
            item.PictureFileName = itemToUpdate.PictureFileName;
            item.Price = itemToUpdate.Price;

            _dbContext.CatalogItems.Update(item);
            await _dbContext.SaveChangesAsync();

            item = await _dbContext.CatalogItems
                .Include(i => i.CatalogBrand)
                .Include(i => i.CatalogType)
                .FirstOrDefaultAsync(i => i.Id == itemToUpdate.Id);

            return item;
        }

        public async Task DeleteAsync(DeleteCatalogItemRequest itemToDelete)
        {
            var item = await _dbContext.CatalogItems.FindAsync(itemToDelete.Id);
            if (item == null)
            {
                throw new KeyNotFoundException("Item not found");
            }

            _dbContext.CatalogItems.Remove(item);
            await _dbContext.SaveChangesAsync();
        }
}