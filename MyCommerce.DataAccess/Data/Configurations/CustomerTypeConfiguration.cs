using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using MyCommerce.Common.Entities;

namespace MyCommerce.DataAccess.Data.Configurations
{
    internal class CustomerTypeConfiguration : EntityTypeConfiguration<Customer>
    {
        public CustomerTypeConfiguration()
        {
            ToTable("Customers");
            HasKey(c => c.Username);

            Property(c => c.Username).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).HasMaxLength(50);
            Property(c => c.FirstName).IsRequired().HasMaxLength(50);
            Property(c => c.LastName).IsRequired().HasMaxLength(50);
            Property(e => e.IsEnabled).IsRequired();

            HasMany(e => e.Orders)
                .WithRequired(e => e.Customer)
                .WillCascadeOnDelete(false);

            HasMany(e => e.ShoppingCarts)
                .WithRequired(e => e.Customer)
                .WillCascadeOnDelete(false);
        }
    }
}
