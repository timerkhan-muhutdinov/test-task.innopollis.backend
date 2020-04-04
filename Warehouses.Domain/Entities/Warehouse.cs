using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Warehouses.Domain.Entities
{
    public class Warehouse
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Номер склада
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Адрес склада
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Список продуктов на складе
        /// </summary>
        public virtual ICollection<WarehouseProduct> Products { get; set; }
    }
}
