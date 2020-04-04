using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Warehouses.Contracts;
using Warehouses.DataAccess;
using Warehouses.Domain.Entities;
using Warehouses.IntegrationTests.Extensions;

namespace Warehouses.IntegrationTests
{
    public class IntegrationTest
    {
        protected readonly HttpClient TestClient;

        protected IntegrationTest()
        {

            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {

                    builder.ConfigureAppConfiguration((hostContext, configurationBuilder) =>
                           {
                               configurationBuilder.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
                           });
                }
                );

            TestClient = appFactory.CreateClient();

            TestClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json;odata.metadata=none");
        }

        protected async Task<Product> CreateProductAsync(Product product)
        {
            var content = new StringContent(product.ToJsonString(), Encoding.UTF8, "application/json");

            var response = await TestClient.PostAsync($"{ApiRoutes.Products.Create}", content);

            var jsonAsString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Product>(jsonAsString);
        }

        protected async Task<Warehouse> GetTopWarehouseAsync()
        {
            var responseWarehouse = await TestClient.GetAsync($"{ApiRoutes.Warehouses.GetAll}?$top=1");
            var jsonAsString = await responseWarehouse.Content.ReadAsStringAsync();
            var warehouses = JsonConvert.DeserializeObject<ODataResponse<Warehouse>>(jsonAsString);

            return warehouses.Value.FirstOrDefault();
        }

        protected async Task<Warehouse> GetWarehouseAsync(Guid warehouseId)
        {
            var responseWarehouse = await TestClient.GetAsync($"{ApiRoutes.Warehouses.Get}({warehouseId})?$expand=Products");
            var jsonAsString = await responseWarehouse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Warehouse>(jsonAsString);
        }
    }
    internal class ODataResponse<T>
    {
        public List<T> Value { get; set; }
    }
}
