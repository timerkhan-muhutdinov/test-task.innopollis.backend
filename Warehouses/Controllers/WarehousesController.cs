using System;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Warehouses.Domain.Entities;

namespace Warehouses.Controllers
{
    public class WarehousesController : ODataController
    {
        private readonly DbContext _dbContext;

        public WarehousesController(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Получение списка складов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Supported, PageSize = 200)]
        public async Task<IActionResult> Get() => Ok(await _dbContext.Set<Warehouse>().ToListAsync());

        /// <summary>
        /// Получение информации о складе
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get(Guid key) => Ok(await _dbContext.Set<Warehouse>()
                                                                            .Include(p => p.Products)
                                                                            .SingleOrDefaultAsync(c => c.Id == key));

        /// <summary>
        /// Добавление товара на склад
        /// </summary>
        /// <param name="warehouseId">Id склада</param>
        /// <param name="productId">Id товара</param>
        /// <param name="count">Количество товара</param>
        /// <returns></returns>
        [HttpPost]
        [EnableQuery]
        [ODataRoute("warehouses({warehouseId})/products({productId})/$ref")]
        public async Task<IActionResult> CreateRef([FromODataUri]Guid warehouseId, [FromODataUri]Guid productId, [FromForm]uint count)
        {
            var link = await _dbContext.Set<WarehouseProduct>()
                                            .SingleOrDefaultAsync(f => f.ProductId == productId && f.WarehouseId == warehouseId);

            if (link != null) return NoContent();

            var warehouse = await _dbContext.Set<Warehouse>()
                                                .SingleOrDefaultAsync(p => p.Id == warehouseId);

            if (warehouse == null) return NotFound();

            var product = await _dbContext.Set<Product>()
                                            .SingleOrDefaultAsync(f => f.Id == productId);

            if (product == null) return NotFound(); 

            var relation = new WarehouseProduct
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                WarehouseId = warehouse.Id,
                Count = count
            };

            await _dbContext.AddAsync(relation);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Изменение количества товаров на складе
        /// </summary>
        /// <param name="warehouseId">Id склада</param>
        /// <param name="productId">Id товара</param>
        /// <param name="count">Количество товара</param>
        /// <returns></returns>
        [HttpPut]
        [EnableQuery]
        [ODataRoute("warehouses({warehouseId})/products({productId})/$ref")]
        public async Task<IActionResult> UpdateRef([FromODataUri]Guid warehouseId, [FromODataUri]Guid productId, [FromForm]uint count)
        {
            var product = await _dbContext.Set<WarehouseProduct>()
                                            .SingleOrDefaultAsync(p => p.ProductId == productId && p.WarehouseId == warehouseId);

            if (product == null) return NotFound();

            product.Count = count;

            _dbContext.Update(product);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Удаление товара со склада
        /// </summary>
        /// <param name="warehouseId">Id склада</param>
        /// <param name="productId">Id товара</param>
        /// <returns></returns>
        [HttpDelete]
        [EnableQuery]
        [ODataRoute("warehouses({warehouseId})/products({productId})/$ref")]
        public async Task<IActionResult> DeleteRef([FromODataUri] Guid warehouseId, [FromODataUri] Guid productId)
        {
            var product = await _dbContext.Set<WarehouseProduct>()
                                            .SingleOrDefaultAsync(p => p.ProductId == productId && p.WarehouseId == warehouseId);

            if (product == null) return NotFound();

            _dbContext.Remove(product);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}