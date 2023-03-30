using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ShoppingCart.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IO.Pipelines;


namespace ShoppingCart.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class StockController : Controller
    {
        private readonly IConfiguration _configuration;
        public StockController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GETSTOCK")]

        public ActionResult<IList<Stock>> GET()
        {
            string connection = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
            List<Stock> stocks = new List<Stock>();
            using (SqlConnection connect = new(connection))
            {
                string query = "SELECT * FROM Stock";
                using (SqlCommand com = new(query))
                {
                    com.Connection = connect;
                    connect.Open();
                    using (SqlDataReader reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            stocks.Add(new Stock
                            {
                                StocktID = Convert.ToInt32(reader["StocktID"]),
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                CreatedBy = reader["CreatedBy"].ToString(),
                                ModifiedBy = reader["ModifiedBy"].ToString(),
                                CreatedDate = (DateTime)(reader["CreatedDate"]),
                                ModifiedDate = (DateTime)(reader["ModifiedDate"])
                            });
                        }
                        connect.Close();
                    }
                }
            }
            return stocks;
        }

        [HttpPost]
        [Route("post")]
        public ActionResult<IList<Stock>> Post(Stock Stock)
        {
            string connection = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
            List<Stock> stocks = new List<Stock>();
            using SqlConnection connect = new(connection);
            {
                connect.Open();
                bool exists = false;
                using (SqlCommand cmd = new SqlCommand("select count(*) from stock where ProductID = '" + Stock.ProductID + @"'", connect))
                {
                    cmd.Parameters.AddWithValue("ProductID", Stock.ProductID);
                    exists = (int)cmd.ExecuteScalar() > 0;
                }
                if (exists)
                {
                    return BadRequest("ProductName is already exists");
                }
                else
                {
                    string query = @"INSERT INTO Stock(
                                ProductID,
                                Quantity,
                                CreatedBy,
                                ModifiedBy,
                                CreatedDate,
                                ModifiedDate
                                ) 
                                VALUES('" + Stock.ProductID + @"',
                                       '" + Stock.Quantity + @"',
                                       '" + Stock.CreatedBy + @"',
                                       '" + Stock.ModifiedBy + @"',
                                       '" + Stock.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss") + @"',
                                       '" + Stock.ModifiedDate.ToString("yyyy-MM-dd HH:mm:ss") + @"'
                                       )";

                    using (SqlCommand com = new(query))
                    {
                        com.Connection = connect;
                        com.ExecuteNonQuery();
                        connect.Close();
                    }
                }
            }
            return stocks;
        }

        [HttpPut]
        [Route("Upsert")]
        public List<Stock> UPSERT(Stock ST)
        {
            string constr = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
            List<Stock> stocks = new List<Stock>();
            using (SqlConnection con = new(constr))
            {
                string query = "UPSERT";
                using (SqlCommand cmd = new(query))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StocktID", ST.StocktID);
                    cmd.Parameters.AddWithValue("@ProductID", ST.ProductID);
                    cmd.Parameters.AddWithValue("@Quantity", ST.Quantity);
                    cmd.Parameters.AddWithValue("@CreatedBy", ST.CreatedBy);
                    cmd.Parameters.AddWithValue("@ModifiedBy", ST.ModifiedBy);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return stocks;
        }


        [HttpDelete]
        [Route("Delete")]
        public ActionResult<IList<Stock>> DeleteStock(int id)
        {
            string connection = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
            List<Stock> stocks = new List<Stock>();
            using (SqlConnection connect = new(connection))
            {
                string query = @"DELETE  FROM Stock
                               WHERE StocktID = '" + id + @"' 
                               ";
                using (SqlCommand com = new(query))
                {
                    com.Connection = connect;
                    connect.Open();
                    com.ExecuteNonQuery();
                    connect.Close();
                }
            }
            return stocks;
        }

        [HttpGet]
        [Route("Stock")]
        public List<GetSample> PROCEDUCE()
        {
            string constr = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
            List<GetSample> stocks = new List<GetSample>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "TOTALAMOUNT";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            stocks.Add(new GetSample
                            {
                                StocktID = Convert.ToInt32(sdr["StocktID"]),
                                ProductID = Convert.ToInt32(sdr["ProductID"]),
                                ProductName = sdr["ProductName"].ToString(),
                                Quantity = Convert.ToInt32(sdr["Quantity"]),
                                UnitPrice = Convert.ToDecimal(sdr["UnitPrice"]),
                                Totalamount = Convert.ToInt32(sdr["totalamount"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return stocks;
        }
    }
}
