using System.Data.Entity;
using MyCommerce.Common.Entities;
using MyCommerce.DataAccess.Data.Configurations;

namespace MyCommerce.DataAccess.Data
{
    [DbConfigurationType(typeof(MyCommerceConfiguration))]
    internal class MyCommerceContext : DbContext
    {
        public MyCommerceContext()
            : base("name=MyCommerceContext")
        {
        }

        public MyCommerceContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<OrderProduct> OrderProducts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ShoppingCartProduct> ShoppingCartProducts { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CustomerTypeConfiguration());
            modelBuilder.Configurations.Add(new OrderTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductTypeConfiguration());
            modelBuilder.Configurations.Add(new ShoppingCartTypeConfiguration());
            modelBuilder.Configurations.Add(new ShoppingCartProductTypeConfiguration());
            modelBuilder.Configurations.Add(new OrderProductTypeConfiguration() );

        }
    }
}
