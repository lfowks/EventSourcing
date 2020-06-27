using Application.Products.Queries.GetProduct;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Products.Queries.GetProducts
{
    public class ProductsVm
    {
        public IList<ProductDto> Data { get; set; }
    }
}
