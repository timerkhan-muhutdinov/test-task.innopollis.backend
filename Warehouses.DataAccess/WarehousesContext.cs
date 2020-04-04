using Microsoft.EntityFrameworkCore;
using System;
using Warehouses.Domain.Entities;

namespace Warehouses.DataAccess
{
    public class WarehousesContext : DbContext
    {
        public WarehousesContext(DbContextOptions<WarehousesContext> options)
          : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseProduct> WarehouseProduct { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasAlternateKey(u => u.Article);

            modelBuilder.Entity<Product>().HasData(
               new Product[]
               {
                    new Product { Id=Guid.NewGuid(), Name="Ананасы", Article="000001", Description="Банка Ананасов", Price=100},
                    new Product { Id=Guid.NewGuid(), Name="Абрикосы", Article="000002", Description="Банка Абрикосов", Price=100},
                    new Product { Id=Guid.NewGuid(), Name="Бананы", Article="000005", Description="Банана", Price=100},
               });

            modelBuilder.Entity<Warehouse>().HasData(
               new Warehouse[]
               {
                    new Warehouse { Id=Guid.NewGuid(), Name="Склад №1", Address="г.Москва"},
                    new Warehouse { Id=Guid.NewGuid(), Name="Склад №2", Address="г.Казань"},
                    new Warehouse { Id=Guid.NewGuid(), Name="Склад №4", Address="г.Иннополис"},
               });
        }
    }
}
