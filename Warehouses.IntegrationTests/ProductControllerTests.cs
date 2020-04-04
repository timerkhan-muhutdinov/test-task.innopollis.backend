using FluentAssertions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Warehouses.Domain.Entities;
using Xunit;
using System.Net.Http;
using Warehouses.IntegrationTests.Extensions;
using System;
using Warehouses.Contracts;

namespace Warehouses.IntegrationTests
{
    public class ProductControllerTests : IntegrationTest
    {
        [Fact]
        public async Task Get_All()
        {
            var response = await TestClient.GetAsync(ApiRoutes.Products.GetAll);

            response.EnsureSuccessStatusCode();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get()
        {
            var product = new Product
            {
                VendorCode = (new Random()).Next(1, 10000000).ToString(),
                Name = "Баклажан",
                Description = "Баклажан натуральный",
                Price = 20,
                Warehouses = new List<WarehouseProduct>()
            };

            // создаем продукт для тестирования
            var createdProduct = await CreateProductAsync(product);

            // Отправляем запрос на получение информации по существующему продукту
            var response = await TestClient.GetAsync(ApiRoutes.Products.Get.Replace("{productId}", createdProduct.Id.ToString()));
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Отправляем запрос на получение информации по несуществующему продукту
            response = await TestClient.GetAsync($"{ApiRoutes.Warehouses.Get}({Guid.NewGuid().ToString()})?$expand=Products");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Post()
        {
            var product = new Product
            {
                VendorCode = (new Random()).Next(1, 10000000).ToString(),
                Name = "Баклажан",
                Description = "Баклажан натуральный",
                Price = 20,
                Warehouses = new List<WarehouseProduct>()
            };

            // создания товара
            var createdProduct = await CreateProductAsync(product);
            createdProduct.Should().NotBeNull(); //Проверка, что товар был создан

            
            var response = await TestClient.GetAsync(ApiRoutes.Products.Get.Replace("{productId}", createdProduct.Id.ToString()));
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var jsonAsString = (await response.Content.ReadAsStringAsync());
            var returnedPost = JsonConvert.DeserializeObject<Product>(jsonAsString);
            returnedPost.Should().BeEquivalentTo(createdProduct);

            // Проверка на передачу null
            response = await TestClient.PostAsync(ApiRoutes.Products.Create, null);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
               
        [Fact]
        public async Task Put()
        {
            var product = new Product
            {
                VendorCode = (new Random()).Next(1, 10000000).ToString(),
                Name = "Доширак",
                Description = "еда богов",
                Price = 20,
                Warehouses = new List<WarehouseProduct>()
            };

            // создание товара
            var createdProduct = await CreateProductAsync(product);
            createdProduct.Should().NotBeNull();

            createdProduct.Name = "BigBon";
            createdProduct.VendorCode = (new Random()).Next(1, 10000000).ToString();
            createdProduct.Warehouses = new List<WarehouseProduct>();

            var updatedContent = new StringContent(createdProduct.ToJsonString(), Encoding.UTF8, "application/json");

            // Проверка измение созданных данных
            var response = await TestClient.PutAsync(ApiRoutes.Products.Update.Replace("{productId}", createdProduct.Id.ToString()), updatedContent);
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var jsonAsString = (await response.Content.ReadAsStringAsync());

            var returnedPost = JsonConvert.DeserializeObject<Product>(jsonAsString);
            returnedPost.Name.Should().BeEquivalentTo("BigBon");
            returnedPost.VendorCode.Should().BeEquivalentTo(createdProduct.VendorCode);

            // Проверка на передачу null
            response = await TestClient.PutAsync(ApiRoutes.Products.Update.Replace("{productId}", createdProduct.Id.ToString()), null);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Проверка на не существование товара
            createdProduct.Id = Guid.NewGuid();
            response = await TestClient.PutAsync(ApiRoutes.Products.Update.Replace("{productId}", createdProduct.Id.ToString()),
                                                        new StringContent(createdProduct.ToJsonString(), Encoding.UTF8, "application/json"));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
