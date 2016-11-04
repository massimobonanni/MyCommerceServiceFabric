using System;
using System.Collections.Generic;

namespace MyCommerce.Common.Entities
{
    public partial class ShoppingCart
    {
        public ShoppingCart()
        {
        }

        public int Id { get; set; }

        public DateTime CreationDate { get; set; }

        public string UserName { get; set; }

        public virtual Customer Customer { get; set; }

         public virtual ICollection<ShoppingCartProduct> Products { get; set; }=new List<ShoppingCartProduct>();
    }
}
