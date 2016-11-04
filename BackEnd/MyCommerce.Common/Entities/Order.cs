using System;
using System.Collections.Generic;

namespace MyCommerce.Common.Entities
{
    public class Order
    {
        public Order()
        {
        }

        public int Id { get; set; }

        public DateTime CreationDate { get; set; }

        public string UserName { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<OrderProduct> Products { get; set; } = new List<OrderProduct>();
    }
}
