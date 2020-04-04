using Newtonsoft.Json;
using System;

namespace Warehouses.IntegrationTests.Extensions
{
    public static class JSONStringExtension 
    {
        public static string ToJsonString(this object model)
        {
            if (model is string) throw new ArgumentException("mode should not be a string");
            return JsonConvert.SerializeObject(model);
        }
    }
}
