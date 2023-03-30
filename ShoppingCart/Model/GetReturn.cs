namespace ShoppingCart.Model
{
    public class GetReturn
    {
        public int ReturnID { get; set; }

        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public long PurchaseID { get; set; }
        public DateTime ReturnedDate { get; set; }
        public int  ReturnedQuantity { get; set; }
        public decimal  UnitPrice { get; set;}
        public decimal UnitDiscount { get; set; }
        public decimal ReturnedTotalDiscount { get; set; }
        public decimal ReturnedTotalAmount { get; set; }
    }
}
