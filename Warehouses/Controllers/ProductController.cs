using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Warehouses.Domain.Entities;

namespace Warehouses.Controllers
{
    public class ProductsController : ODataController
    {
        private readonly DbContext _dbContext;

        public ProductsController(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Получение списка товаров
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Supported, PageSize = 200)]
        public async Task<IActionResult> Get() => Ok(await _dbContext.Set<Product>().ToListAsync());

        /// <summary>
        /// Получение информации о товаре
        /// </summary>
        /// <param name="key">Ид товара</param>
        /// <returns></returns>
        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get(Guid key) => Ok(await _dbContext.Set<Product>().Include(p => p.Warehouses).SingleOrDefaultAsync(c => c.Id == key));

        /// <summary>
        /// Создать товар
        /// </summary>
        /// <param name="product">Данные о товаре</param>
        /// <returns></returns>
        [HttpPost]
        [EnableQuery]
        public async Task<IActionResult> Post([FromBody]Product product)
        {
            if (product == null) return BadRequest();
            if (_dbContext.Set<Product>().Any(x => x.Article == product.Article)) return NotFound();

            product.Id = Guid.NewGuid();

            _dbContext.Set<Product>().Add(product);
            await _dbContext.SaveChangesAsync();

            return Ok(product);
        }

        /// <summary>
        /// Изменение информации о товаре
        /// </summary>
        /// <param name="productId">Ид товара</param>
        /// <param name="product">Новые данные о товаре</param>
        /// <returns></returns>
        [HttpPut]
        [EnableQuery]
        [ODataRoute("products({productId})")]
        public async Task<IActionResult> Put([FromODataUri]Guid productId, [FromBody]Product product)
        {
            if (productId != product?.Id) return BadRequest();

            if (!_dbContext.Set<Product>().Any(x => x.Id == productId)) return NotFound();
            if (_dbContext.Set<Product>().Any(x => x.Article == product.Article)) return NotFound();

            _dbContext.Set<Product>().Update(product);

            await _dbContext.SaveChangesAsync();

            return Ok(product);
        }
    }
}
