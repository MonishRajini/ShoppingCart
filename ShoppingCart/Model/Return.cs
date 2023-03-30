namespace ShoppingCart.Model
{
    public class Return 
    {
        public long  ReturnID { get; set; }
        public long  PurchaseID { get; set; }
        public long  ProductID { get;set ; }   
        public int  ReturnedQuantity { get; set; }
        public decimal  ReturnedTotalDiscount { get; set; }
        public decimal  ReturnedTotalAmount { get;set ; }
        public DateTime  ReturnedDate { get; set;}
        public string CreatedBy { get; set;}
        public string ModifiedBy { get; set;}
        public DateTime CreatedDate { get; set;}
        public DateTime  ModifiedDate { get; set;} 
    }
}
