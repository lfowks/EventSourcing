using Application.Common.Mappings;
using Application.Products.Queries.GetProduct;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Products.Queries.GetProducts
{
    public class ProductsDto : IMapFrom<ProductList>
    {
        public ProductsDto()
        {
            Items = new List<ProductDto>();
        }

        //public int Id { get; set; }

        //public string Name { get; set; }

        public IList<ProductDto> Items { get; set; }
    }
}
