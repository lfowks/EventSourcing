using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class CartItem:Auditable
    {
        public int Product_Id { get; set; }

        public int Quantity { get; set; }
    }
}
