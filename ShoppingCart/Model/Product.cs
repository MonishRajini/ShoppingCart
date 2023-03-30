using System;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Model
{
    public class Product
    {
        public long ProductID { get; set; }

        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal UnitDiscount { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime   CreatedDate { get; set; }

        public DateTime  ModifiedDate { get; set; }
    }
}
