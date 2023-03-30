namespace ShoppingCart.Model
{
    public class ListPurchase
    {
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        
        public DateTime  PurchasedDate { get; set; }

        public int Quantity { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime ReturnedDate { get; set; }
        public int ReturnedQuantity { get; set; }
        public decimal ReturnedTotalDiscount { get; set; }
        public decimal ReturnedTotalAmount { get; set; }
    }
}
