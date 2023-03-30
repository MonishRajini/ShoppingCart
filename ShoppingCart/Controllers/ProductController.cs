using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using ShoppingCart.Model;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;

namespace ShoppingCart.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IConfiguration _configuration;
        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpGet]
        [Route("GET")]
        public List<Product> Get()
        {
            string constr = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
            List<Product> products = new List<Product>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM Product ";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                ProductName = reader["Productname"].ToString(),
                                UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                                UnitDiscount = Convert.ToDecimal(reader["UnitDiscount"]),
                                CreatedBy = reader["CreatedBy"].ToString(),
                                ModifiedBy = reader["ModifiedBy"].ToString(),
                                CreatedDate = (DateTime)(reader["CreatedDate"]),
                                ModifiedDate = (DateTime)(reader["ModifiedDate"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return products;
        }

        [HttpPost]
        [Route("POST")]

        public ActionResult<IList<Product>> Post(Product pro)
        {
            string constr = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
            List<Product> products = new List<Product>();
            using (SqlConnection con = new  (constr))
            {
                con.Open();
                bool exists = false;
                using (SqlCommand cmd = new SqlCommand("select count(*) from Product where ProductName = '" + pro.ProductName + @"'", con))
                {
                    cmd.Parameters.AddWithValue("ProductName", pro.ProductName);
                    exists = (int)cmd.ExecuteScalar() > 0;
                }
                if (exists)
                {
                    return BadRequest("ProductName is already exists");
                }
                else
                {
                    string query = @"INSERT INTO Product (
                        ProductName,
                        UnitPrice,
                        UnitDiscount,
                        CreatedBy,
                        ModifiedBy,
                        CreatedDate,
                        ModifiedDate)
                        VALUES('" + pro.ProductName + @"',
                               '" + pro.UnitPrice + @"',
                               '" + pro.UnitDiscount + @"',
                               '" + pro.CreatedBy + @"',
                               '" + pro.ModifiedBy + @"', 
                               '" + pro.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss") + @"',
                               '" + pro.ModifiedDate.ToString("yyyy-MM-dd HH:mm:ss") + @"'
                                )";
                    using (SqlCommand cmd = new(query))
                    {
                        cmd.Connection = con;
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    return products;
                }
            }
        }
        [HttpPut]
        [Route("PUT")]
        public List<Product> Products(Product pro, int id)
        {
            string constr = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
            List<Product> products = new List<Product>();
            using (SqlConnection connection = new(constr))
            {
                string update = @"UPDATE Product set
                                 ProductName ='" + pro.ProductName + @"'
                                ,UnitPrice = '" + pro.UnitPrice + @"'
                                ,UnitDiscount ='" + pro.UnitDiscount + @"'
                                ,CreatedBy ='" + pro.CreatedBy + @"'
                                ,ModifiedBy ='" + pro.ModifiedBy + @"'
                                ,CreatedDate ='" + pro.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss") + @"'
                                ,ModifiedDate ='" + pro.ModifiedDate.ToString("yyyy-MM-dd HH:mm:ss") + @"'
                                 WHERE ProductID =" + id + @"";

                using (SqlCommand command = new(update))
                {
                    command.Connection = connection;
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return products;
            }

        }

        [HttpDelete]
        [Route("DELETE")]

        public ActionResult<IList<Product>> delete(int id)
        {
            try
            {
                string productDP = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
                List<Product> products = new();
                using (SqlConnection connection = new(productDP))
                {
                    string deleteproduct = @"DELETE  FROM Product
                                         WHERE ProductID = '" + id + @"' 
                                         ";

                    using (SqlCommand command = new SqlCommand(deleteproduct))
                    {
                        command.Connection = connection;
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    using (SqlConnection con = new(productDP))
                    {   
                        string qurey2 = @"DELETE  FROM STOCK
                                         WHERE ProductID = '" + id + @"' 
                                         ";
                        using (SqlCommand com = new(qurey2))
                        {
                            com.Connection = con;
                            con.Open();
                            com.ExecuteNonQuery();
                            con.Close();

                        }
                    }
                    return products;
                }
            }
            catch (Exception)
            {
                return BadRequest("ID IS EXISTS IN STOCK AND PURCHASE TABLE ");
            }
        }


        [HttpGet]
        [Route("ID")]

        public Product getId(int ID)
        {
            string ProductDB = _configuration.GetValue<string>("ConnectionStrings:ShoppingDb");
            Product products = new Product();
            using (SqlConnection connection = new(ProductDB))
            {
                string query = "sp_GetAllProducts";
                using (SqlCommand Cmd = new(query))
                {
                    Cmd.Connection = connection;
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@ProductID", ID);

                    connection.Open();
                    using (SqlDataReader sdr = Cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            products = new Product
                            {
                                ProductID = Convert.ToInt32(sdr["ProductID"]),
                                ProductName = sdr["Productname"].ToString(),
                                UnitPrice = Convert.ToDecimal(sdr["UnitPrice"]),
                                UnitDiscount = Convert.ToDecimal(sdr["UnitDiscount"]),
                                CreatedBy = sdr["CreatedBy"].ToString(),
                                ModifiedBy = sdr["ModifiedBy"].ToString(),
                                CreatedDate = (DateTime)(sdr["CreatedDate"]),
                                ModifiedDate = (DateTime)(sdr["ModifiedDate"])
                            };
                        }
                    }
                    connection.Close();
                }
                return products;
            }
        }
    }
}
