namespace MyCommerce.Common.Entities
{
    public partial class ShoppingCartProduct
    {
        public int Id { get; set; }

        public int IdShoppingCart { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public virtual ShoppingCart ShoppingCart { get; set; }
    }
}
