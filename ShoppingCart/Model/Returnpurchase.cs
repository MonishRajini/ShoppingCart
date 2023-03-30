namespace ShoppingCart.Model
{
    public class Returnpurchase
    {
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public long PurchaseID { get; set; }
        public DateTime PurchasedDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set;}
        public decimal UnitDiscount { get; set; }
        public decimal TotalDiscount { get; set;}
        public decimal TotalAmount { get; set; }
    }
}
