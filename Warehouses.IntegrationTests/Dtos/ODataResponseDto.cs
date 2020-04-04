using System;
using System.Collections.Generic;

namespace Warehouses.IntegrationTests.Dtos
{
    internal class ODataResponseDto<T>
    {
        public List<T> Value { get; set; }
    }
}
