using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using MyCommerce.Common.Entities;

namespace MyCommerce.DataAccess.Data.Configurations
{
    internal class OrderTypeConfiguration : EntityTypeConfiguration<Order>
    {
        public OrderTypeConfiguration()
        {
            ToTable("Orders");
            HasKey(c => c.Id);
            Property(c => c.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(c => c.CreationDate)
                .IsRequired().HasColumnType("datetime2");

            Property(c => c.UserName)
                .IsRequired()
                .HasMaxLength(50);

            HasMany(e => e.Products)
                 .WithRequired(e => e.Order)
                 .HasForeignKey(e => e.IdOrder)
                 .WillCascadeOnDelete(false);
        }
    }
}
