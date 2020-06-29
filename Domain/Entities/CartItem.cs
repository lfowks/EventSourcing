using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class CartItem:Auditable
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }
    }

    public class Cart : Auditable
    {
        public Cart()
        {
            CartItems = new List<CartItem>();
        }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
