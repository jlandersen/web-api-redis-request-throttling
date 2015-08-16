using Microsoft.AspNet.Mvc;

namespace ThrottlingApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IActionResult Get()
        {
            return Json(null);
        }
    }
}
