using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Warehouses.Contracts;
using Warehouses.Domain.Entities;
using Xunit;

namespace Warehouses.IntegrationTests
{
    public class WarehousesControllerTests : IntegrationTest
    {
        [Fact]
        public async Task Get_All()
        {
            var response = await TestClient.GetAsync(ApiRoutes.Warehouses.GetAll);

            response.EnsureSuccessStatusCode();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get()
        {
            // получаем склад для добавления продукта (т.к. нету метода на создание склада, возьмем первый склад из базы и получим по нему информацию)
            var warehouse = await GetTopWarehouseAsync();

            // Отправляем запрос на получение информации по существуему складу
            var response = await TestClient.GetAsync($"{ApiRoutes.Warehouses.Get}({warehouse.Id.ToString()})?$expand=Products");
            var jsonAsString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            warehouse = JsonConvert.DeserializeObject<Warehouse>(jsonAsString);
            warehouse.Should().NotBeNull();

            // Отправляем запрос на получение информации по несуществующему складу
            response = await TestClient.GetAsync($"{ApiRoutes.Warehouses.Get}({Guid.NewGuid().ToString()})?$expand=Products");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task CreateRef()
        {
            var product = new Product
            {
                Article = (new Random()).Next(1, 10000000).ToString(),
                Name = "Баклажан",
                Description = "Баклажан натуральный",
                Price = 20,
                Warehouses = new List<WarehouseProduct>()
            };

            // создаем товар для добавления на склад
            var createdProduct = await CreateProductAsync(product);

            // получаем склад для добавления продукта
            var warehouse = await GetTopWarehouseAsync();

            var content = new Dictionary<string, string>
            {
                { "count", "5" },
            };

            //Добавляем товар на склад
            var response = await TestClient.PostAsync(
                                                        ApiRoutes.Warehouses.AddProduct
                                                                                .Replace("{warehouseId}", warehouse.Id.ToString())
                                                                                .Replace("{productId}", createdProduct.Id.ToString()),
                                                        new FormUrlEncodedContent(content));
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            //Проверки
            warehouse = await GetWarehouseAsync(warehouse.Id);
            warehouse.Products.Any(x => x.ProductId == createdProduct.Id).Should().BeTrue(); // Проевряем наличие товара на складе
            warehouse.Products.SingleOrDefault(x => x.ProductId == createdProduct.Id).Count.Should().Be(5); // Проевряем количество товара на складе


        }

        [Fact]
        public async Task UpdateRef()
        {
            var product = new Product
            {
                Article = (new Random()).Next(1, 10000000).ToString(),
                Name = "Баклажан",
                Description = "Баклажан натуральный",
                Price = 20,
                Warehouses = new List<WarehouseProduct>()
            };

            // создаем товар для добавления на склад
            var createdProduct = await CreateProductAsync(product);

            // получаем склад для добавления товаров
            var warehouse = await GetTopWarehouseAsync();

            var content = new Dictionary<string, string>
            {
                { "count", "5" },
            };

            //Добавляем товар на склад
            await TestClient.PostAsync(
                                                        ApiRoutes.Warehouses.AddProduct
                                                                                .Replace("{warehouseId}", warehouse.Id.ToString())
                                                                                .Replace("{productId}", createdProduct.Id.ToString()),
                                                        new FormUrlEncodedContent(content));

            // Изменяем количество товара на склад
            var response = await TestClient.PutAsync(
                                                        ApiRoutes.Warehouses.UpdateCountProduct
                                                                                .Replace("{warehouseId}", warehouse.Id.ToString())
                                                                                .Replace("{productId}", createdProduct.Id.ToString()),
                                                        new FormUrlEncodedContent(content));

            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Проверяем количество товара на складе
            warehouse = await GetWarehouseAsync(warehouse.Id);
            warehouse.Products.SingleOrDefault(x => x.ProductId == createdProduct.Id).Count.Should().Be(5);
        }

        [Fact]
        public async Task Delete()
        {
            var product = new Product
            {
                Article = (new Random()).Next(1, 10000000).ToString(),
                Name = "Баклажан",
                Description = "Баклажан натуральный",
                Price = 20,
                Warehouses = new List<WarehouseProduct>()
            };

            // создаем товар для добавления на склад
            var createdProduct = await CreateProductAsync(product);

            // получаем склад для добавления товара
            var warehouse = await GetTopWarehouseAsync();

            var content = new Dictionary<string, string>
            {
                { "count", "5" },
            };

            //Добавляем товар на склад
            await TestClient.PostAsync(
                                                        ApiRoutes.Warehouses.AddProduct
                                                                                .Replace("{warehouseId}", warehouse.Id.ToString())
                                                                                .Replace("{productId}", createdProduct.Id.ToString()),
                                                        new FormUrlEncodedContent(content));

            // Удаляем товар со склада
            var response = await TestClient.DeleteAsync(
                                                        ApiRoutes.Warehouses.UpdateCountProduct
                                                                                .Replace("{warehouseId}", warehouse.Id.ToString())
                                                                                .Replace("{productId}", createdProduct.Id.ToString()));

            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            response = await TestClient.DeleteAsync(
                                                        ApiRoutes.Warehouses.UpdateCountProduct
                                                                                .Replace("{warehouseId}", Guid.NewGuid().ToString())
                                                                                .Replace("{productId}", createdProduct.Id.ToString()));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

    }
}
