namespace MyCommerce.Common.Entities
{
    public partial class Product
    {
        public int Id { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public bool IsAvailable { get; set; }

        public decimal UnitPrice { get; set; }

        public int UnitInStore { get; set; }
    }
}
