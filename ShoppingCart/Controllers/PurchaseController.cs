using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.FileProviders.Physical;
using ShoppingCart.Model;
using System;
using System.Data;
using System.IO.Pipelines;

namespace ShoppingCart.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PurchaseController : Controller
    {
        private readonly IConfiguration _configuration;
        public PurchaseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

       
        [HttpGet]
        [Route("GETPURCHASE")]
        public List<Purchase> PROCEDUCE()
        {
            string constr = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
            List<Purchase> purchases = new List<Purchase>();
            using (SqlConnection con = new(constr))
            {
                string query = "PURCHASESTORE";
                using (SqlCommand cmd = new(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            purchases.Add(new Purchase
                            {
                                PurchaseID = Convert.ToInt32(sdr["PurchaseID"]),
                                ProductID = Convert.ToInt32(sdr["ProductID"]),
                                Quantity = Convert.ToInt32(sdr["Quantity"]),
                                UnitPrice = Convert.ToDecimal(sdr["UnitPrice"]),
                                UnitDiscount = Convert.ToDecimal(sdr["UnitDiscount"]),
                                TotalDiscount = Convert.ToDecimal(sdr["TotalDiscount"]),
                                TotalAmount = Convert.ToDecimal(sdr["TotalAmount"]),
                                CreatedBy = sdr["CreatedBy"].ToString(),
                                ModifiedBy = sdr["ModifiedBy"].ToString(),
                                CreatedDate = (DateTime)sdr["CreatedDate"],
                                ModifiedDate = (DateTime)sdr["ModifiedDate"]
                            });
                        }
                    }
                    con.Close();
                }
            }
            return purchases;
        }

        [HttpPost]
        [Route("POST")]

        public ActionResult<IList<Purchase>> PostPurchase(Purchase PUR)
        {
            try
            {
                string connection = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
                List<Purchase> purchases = new List<Purchase>();
                int stocksQuantity = 0;
                using (SqlConnection con = new(connection))
                {
                    string qurey2 = "select quantity from stock where productID=" + PUR.ProductID + @"";

                    using (SqlCommand com = new(qurey2))
                    {
                        com.Connection = con;
                        con.Open();
                        using (SqlDataReader reader = com.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                stocksQuantity = Convert.ToInt32(reader["Quantity"]);
                            }
                            con.Close();
                        }
                    }
                }
                using (SqlConnection connect = new(connection))
                {
                    connect.Open();
                    bool exists = false;
                    using (SqlCommand cmd = new SqlCommand("select count(*) from PRODUCT where ProductID = '" + PUR.ProductID + @"'", connect))
                    {
                        cmd.Parameters.AddWithValue("ProductID", PUR.ProductID);
                        exists = (int)cmd.ExecuteScalar() > 0;
                    }
                    if (PUR.Quantity > stocksQuantity)
                    {
                        return BadRequest("quantity is not availble");
                    }
                    else if (exists)
                    {
                        string query = @"INSERT INTO Purchase(
                                ProductID,
                                Quantity,
                                UnitPrice,
                                UnitDiscount,
                                TotalDiscount,
                                TotalAmount,
                                PurchasedDate,
                                CreatedBy,
                                ModifiedBy,
                                CreatedDate,
                                ModifiedDate)
                                VALUES('" + PUR.ProductID + @"',
                                       '" + PUR.Quantity + @"',
                                       '" + PUR.UnitPrice + @"',
                                       '" + PUR.UnitDiscount + @"',
                                       '" + PUR.TotalDiscount + @"',
                                       '" + PUR.TotalAmount + @"',
                                       '" + PUR.PurchasedDate.ToString("yyyy-MM-dd HH:mm:ss") + @"',
                                       '" + PUR.CreatedBy + @"',
                                       '" + PUR.ModifiedBy + @"',
                                       '" + PUR.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss") + @"',
                                       '" + PUR.ModifiedDate.ToString("yyyy-MM-dd HH:mm:ss") + @"'
                                       )";
                        using (SqlCommand command = new(query))
                        {
                            command.Connection = connect;
                            command.ExecuteNonQuery();
                            connect.Close();
                        }
                    }
                    else
                    {
                        return BadRequest("INVALID PRODUCTID");
                    }
                }
                var remainQuantity = stocksQuantity - PUR.Quantity;
                string Query1 = @"UPDATE stock set
                                    Quantity ='" + remainQuantity + @"'
                                    WHERE ProductID='" + PUR.ProductID + @"'";
                using (SqlConnection con = new(connection))
                {
                    using (SqlCommand com = new(Query1))
                    {
                        com.Connection = con;
                        con.Open();
                        com.ExecuteNonQuery();
                        con.Close();
                    }

                }
                return purchases;
            }
            catch (Exception)
            {
                return BadRequest("Pls Enter the valid productID ");
            }
        }



        [HttpPut]
        [Route("PUT")]

        public ActionResult<IList<Purchase>> Putpurchase(Purchase pur, int id)
        {
            string connection = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
            List<Purchase> purchases = new List<Purchase>();
            using (SqlConnection connect = new(connection))
            {
                string query = @"UPDATE Purchase set
                                ProductID = '" + pur.ProductID + @"'
                                ,Quantity = '" + pur.Quantity + @"'
                                ,UnitPrice = '" + pur.UnitPrice + @"'
                                ,UnitDiscount ='" + pur.UnitDiscount + @"'
                                ,TotalDiscount='" + pur.TotalDiscount + @"'
                                ,TotalAmount='" + pur.TotalAmount + @"'
                                ,PurchasedDate='" + pur.PurchasedDate.ToString("yyyy-MM-dd HH:mm:ss") + @"'
                                ,CreatedBy ='" + pur.CreatedBy + @"'
                                ,ModifiedBy ='" + pur.ModifiedBy + @"'
                                ,CreatedDate ='" + pur.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss") + @"'
                                ,ModifiedDate ='" + pur.ModifiedDate.ToString("yyyy-MM-dd HH:mm:ss") + @"'
                                 WHERE PurchaseID =" + id + @"";
                using (SqlCommand command = new(query))
                {
                    command.Connection = connect;
                    connect.Open();
                    command.ExecuteNonQuery();
                    connect.Close();
                }
            }
            return purchases;
        }

        [HttpDelete]
        [Route("DELETE")]

        public ActionResult<IList<Purchase>> DeletePurchase(int ID)
        {
            string Connection = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
            List<Purchase> Purchases = new List<Purchase>();
            using (SqlConnection Connect = new(Connection))
            {
                string Query = @"DELETE  FROM Purchase
                                         WHERE PurchaseID = '" + ID + @"' 
                                         ";
                using (SqlCommand Command = new(Query))
                {
                    Command.Connection = Connect;
                    Connect.Open();
                    Command.ExecuteNonQuery();
                    Connect.Close();
                }
            }
            return Purchases;

        }


        [HttpGet]
        [Route("PURCHASEDISCOUNT")]
        public List<Returnpurchase> purchases1()
        {
            string constr = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
            List<Returnpurchase> purchases = new List<Returnpurchase>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "PURCHASEDISCOUNT";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            purchases.Add(new Returnpurchase
                            {
                                PurchaseID = Convert.ToInt32(sdr["PurchaseID"]),
                                ProductID = Convert.ToInt32(sdr["ProductID"]),
                                ProductName = sdr["ProductName"].ToString(),
                                Quantity = Convert.ToInt32(sdr["Quantity"]),
                                UnitPrice = Convert.ToDecimal(sdr["UnitPrice"]),
                                UnitDiscount = Convert.ToDecimal(sdr["UnitDiscount"]),
                                TotalDiscount = Convert.ToDecimal(sdr["TotalDiscount"]),
                                TotalAmount = Convert.ToDecimal(sdr["TotalAmount"]),
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

