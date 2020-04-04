using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Warehouses.Domain.Entities
{
    public class Product
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Артикуль товара
        /// </summary>
        public string Article { get; set; }

        /// <summary>
        /// Имя товара
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание товара
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Цена товара
        /// </summary>
        public double Price { get; set; }


        /// <summary>
        /// Список продуктов на складе
        /// </summary>
        public virtual ICollection<WarehouseProduct> Warehouses { get; set; }

    }
}
