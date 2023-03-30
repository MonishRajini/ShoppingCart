using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ShoppingCart.Model;

namespace ShoppingCart.Controllers
{
    public class ListPurchaseController : Controller
    {

        private readonly IConfiguration _configuration;
        public ListPurchaseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("listpurchase")]
        public List<ListPurchase> LISTPURCHASE()
        {
            string constr = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
            List<ListPurchase> purchases = new List<ListPurchase>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "LISTPURCHASE";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            purchases.Add(new ListPurchase
                            {
                                ProductID = Convert.ToInt32(sdr["ProductID"]),
                                ProductName = sdr["ProductName"].ToString(),
                                Quantity = Convert.ToInt32(sdr["Quantity"]),
                                PurchasedDate = Convert.ToDateTime(sdr["PurchasedDate"]),
                                TotalDiscount = Convert.ToDecimal(sdr["TotalDiscount"]),
                                TotalAmount = Convert.ToDecimal(sdr["TotalAmount"]),
                                ReturnedDate = (DateTime)sdr["ReturnedDate"],
                                ReturnedQuantity = Convert.ToInt32(sdr["ReturnedQuantity"]),
                                ReturnedTotalDiscount = Convert.ToInt32(sdr["ReturnedTotalDiscount"]),
                                ReturnedTotalAmount = Convert.ToInt32(sdr["ReturnedTotalAmount"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return purchases;
        }
    }
}
