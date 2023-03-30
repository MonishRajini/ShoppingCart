using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingCart.Controllers
{
    
    [Route("[controller]")]
    [ApiController]
    public class HelloController : Controller
    {
        [HttpGet]
        [Route("GET")]
        public  string Get()
        {
            return " Hello World";
        }
    }
}
