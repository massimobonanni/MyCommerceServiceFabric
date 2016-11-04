using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyCommerce.Common.Entities
{
    public class Customer
    {
        public Customer()
        {
        }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public bool IsEnabled { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
    }
}
