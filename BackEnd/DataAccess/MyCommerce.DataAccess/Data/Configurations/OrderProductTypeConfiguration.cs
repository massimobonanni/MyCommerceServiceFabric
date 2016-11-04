using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using MyCommerce.Common.Entities;

namespace MyCommerce.DataAccess.Data.Configurations
{
    internal class OrderProductTypeConfiguration : EntityTypeConfiguration<OrderProduct>
    {
        public OrderProductTypeConfiguration()
        {
            ToTable("OrderProducts");
            HasKey(c => c.Id);
            Property(c => c.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(e => e.UnitPrice)
                .HasPrecision(18, 0);

            Property(c => c.ShortDescription)
               .IsRequired()
               .HasMaxLength(100);

            Property(c => c.LongDescription)
                .IsRequired();
        }
    }
}
