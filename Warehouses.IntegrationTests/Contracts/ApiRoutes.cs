using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouses.Contracts
{
    public class ApiRoutes
    {
        public const string Base = "/odata";

        public static class Products
        {
            public const string Get = Base + "/Products({productId})";
            public const string GetAll = Base + "/Products";
            public const string Create = Base + "/Products";
            public const string Update = Base + "/Products({productId})";
        }

        public static class Warehouses
        {
            public const string GetAll = Base + "/Warehouses";
            public const string Get = Base + "/Warehouses";
            public const string AddProduct = Base + "/Warehouses({warehouseId})/products({productId})/$ref";
            public const string UpdateCountProduct = Base + "/Warehouses({warehouseId})/products({productId})/$ref";
            public const string DeleteProduct = Base + "/Warehouses({warehouseId})";
        }
    }
}
