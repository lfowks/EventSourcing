using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class ProductList: Auditable
    {
        public ProductList()
        {
            Items = new List<Product>();
        }


        public IList<Product> Items { get; set; }
    }
}
