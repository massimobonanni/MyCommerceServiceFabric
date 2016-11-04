namespace MyCommerce.Common.Entities
{
    public partial class OrderProduct
    {
        public int Id { get; set; }

        public int IdOrder { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantita { get; set; }

        public virtual Order Order { get; set; }
    }
}
