using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Warehouses.Domain.Entities
{
    public class WarehouseProduct
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Уникальный идентификатор товара
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Уникальный идентификатор склада
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// Количество товара на складе
        /// </summary>
        public uint Count { get; set; }



        public Product Product { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
