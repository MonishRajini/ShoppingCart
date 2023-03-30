namespace ShoppingCart.Model
{
    public class Stock
    { 
        public long StocktID { get; set; }

        public long ProductID { get; set; }

        public int Quantity { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
    }

}
