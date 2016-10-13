using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using MyCommerce.Common.Entities;

namespace MyCommerce.DataAccess.Data.Configurations
{
    internal class ShoppingCartTypeConfiguration : EntityTypeConfiguration<ShoppingCart>
    {
        public ShoppingCartTypeConfiguration()
        {
            ToTable("ShoppingCarts");
            HasKey(c => c.Id);
            Property(c => c.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(c => c.CreationDate)
                .HasColumnType("datetime2");

            Property(c => c.UserName)
                .IsRequired()
                .HasMaxLength(50);

            HasMany(e => e.Products)
                .WithRequired(e => e.ShoppingCart)
                .HasForeignKey(e => e.IdShoppingCart)
                .WillCascadeOnDelete(false);
        }
    }
}
