using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ShoppingCart.Model;
using System;
using System.Data;

namespace ShoppingCart.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReturnController : Controller
    {
        private readonly IConfiguration _configuration;
        public ReturnController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetReture")]

        public ActionResult<IList<GetReturn>> get()
        {
            string connection = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
            List<GetReturn> returns = new List<GetReturn>();
            using (SqlConnection connect = new SqlConnection(connection))
            {
                string Query = "RETURNSTORE";
                using (SqlCommand com = new SqlCommand(Query))
                {
                    com.Connection = connect;
                    connect.Open();
                    using (SqlDataReader read = com.ExecuteReader())
                    {
                        while (read.Read())
                        {
                            returns.Add(new GetReturn
                            {
                                ReturnID = Convert.ToInt32(read["ReturnID"]),
                                PurchaseID = Convert.ToInt32(read["PurchaseID"]),
                                ProductID = Convert.ToInt32(read["ProductID"]),
                                ReturnedQuantity = Convert.ToInt32(read["ReturnedQuantity"]),
                                ReturnedTotalDiscount = Convert.ToDecimal(read["ReturnedTotalDiscount"]),
                                ReturnedTotalAmount = Convert.ToDecimal(read["ReturnedTotalAmount"]),
                                ReturnedDate = (DateTime)(read["ReturnedDate"]),
                                UnitDiscount = Convert.ToDecimal(read["UnitDiscount"]),
                                UnitPrice = Convert.ToDecimal(read["UnitPrice"]),
                                ProductName = read["ProductName"].ToString()
                            });
                        }
                    }
                    connect.Close();
                }
            }
            return returns;
        }

        [HttpPost]
        [Route("POSTReturn")]

        public ActionResult<IList<Return>> Post(Return RE)
        {
            string connection = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
            List<Return> returns = new List<Return>();
            int purchaseQuantity = 0;
            int stocksQuantity = 0;
            using (SqlConnection con = new SqlConnection(connection))
            {
                string qurey2 = "select quantity from Purchase where productID=" + RE.ProductID + @"";

                using (SqlCommand com = new SqlCommand(qurey2))
                {
                    com.Connection = con;
                    con.Open();
                    using (SqlDataReader reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            purchaseQuantity = Convert.ToInt32(reader["Quantity"]);
                        }
                        con.Close();
                    }
                }
            }
            using SqlConnection connect = new SqlConnection(connection);
            connect.Open();
            bool exists = false;
            using (SqlCommand cmd = new SqlCommand("select COUNT(*) from PURCHASE where ProductID = '" + RE.ProductID + @"'AND PurchaseID='"+RE.PurchaseID +@"'", connect))
            {
                cmd.Parameters.AddWithValue("ProductID", RE.ProductID);
                exists = (int)cmd.ExecuteScalar() > 0;
            }
            if (RE.ReturnedQuantity > purchaseQuantity)
            {
                return BadRequest("INVAILD QUANTITY");
            }
            else if (exists)
            {
                string query = @"INSERT INTO _Return(
                                PurchaseID,
                                ProductID,
                                ReturnedQuantity,
                                ReturnedTotalDiscount,
                                ReturnedTotalAmount,
                                ReturnedDate,
                                CreatedBy,
                                ModifiedBy,
                                CreatedDate,
                                ModifiedDate)
                                VALUES('" + RE.PurchaseID + @"',
                                       '" + RE.ProductID + @"',
                                       '" + RE.ReturnedQuantity + @"',
                                       '" + RE.ReturnedTotalDiscount + @"',
                                       '" + RE.ReturnedTotalAmount + @"',
                                       '" + RE.ReturnedDate.ToString("yyyy-MM-dd HH:mm:ss") + @"',
                                       '" + RE.CreatedBy + @"',
                                       '" + RE.ModifiedBy + @"',
                                       '" + RE.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss") + @"',
                                       '" + RE.ModifiedDate.ToString("yyyy-MM-dd HH:mm:ss") + @"'
                                       )";
                using (SqlCommand com = new SqlCommand(query))
                {
                    com.Connection = connect;
                    com.ExecuteNonQuery();
                    connect.Close();
                }
            }
            else
            {
                return BadRequest("INVAILD PRODUCTID");
            }
            using (SqlConnection con = new SqlConnection(connection))
            {
                string qurey2 = "select quantity from stock where productID=" + RE.ProductID + @"";

                using (SqlCommand com = new SqlCommand(qurey2))
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
            var remainQuantity = stocksQuantity + RE.ReturnedQuantity;
            string query1 = @"UPDATE stock set
                                    Quantity ='" + remainQuantity + @"'
                                    WHERE ProductID='" + RE.ProductID + @"'";
            using (SqlConnection con = new SqlConnection(connection))
            {
                using (SqlCommand com = new SqlCommand(query1))
                {
                    com.Connection = con;
                    con.Open();
                    com.ExecuteNonQuery();
                    con.Close();
                }

            }
            return returns;
        }

        [HttpPut]
        [Route("PUT")]

        public ActionResult<IList<Return>> Put(Return re)
        {
            string connection = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
            List<Return> returns= new List<Return>();
            using (SqlConnection connect= new(connection))
            {
                string query = "UPSERTRETURN";
                using (SqlCommand cmd = new(query))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ReturnID", re.ReturnID);
                    cmd.Parameters.AddWithValue("@PurchaseID", re.PurchaseID);
                    cmd.Parameters.AddWithValue("@ProductID", re.ProductID);
                    cmd.Parameters.AddWithValue("@ReturnedQuantity", re.ReturnedQuantity);
                    cmd.Parameters.AddWithValue("@ReturnedTotalDiscount", re.ReturnedTotalDiscount);
                    cmd.Parameters.AddWithValue("@ReturnedTotalAmount", re.ReturnedTotalAmount);
                    cmd.Parameters.AddWithValue("@ReturnedDate", re.ReturnedDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@CreatedBy", re.CreatedBy);
                    cmd.Parameters.AddWithValue("@ModifiedBy", re.ModifiedBy);
                    cmd.Connection = connect;
                    connect.Open();
                    cmd.ExecuteNonQuery();
                    connect.Close();
                }
            }
            return returns;
        }

        [HttpDelete]
        [Route("DELETE")]

        public ActionResult<IList<Return>> Delete(int id, Return re)
        {
            try
            {
                string connection = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
                List<Return> returns = new List<Return>();
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = "DELETERETURN";
                    using (SqlCommand com = new SqlCommand(query))
                    {
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@ReturnID", id);
                        com.Connection = connect;
                        connect.Open();
                        com.ExecuteNonQuery();
                        connect.Close();
                    }

                }
                return returns;
            }
            catch (Exception)
            {

                return BadRequest("purchaseID IS NOT exsist");

            }

        }
    }
}