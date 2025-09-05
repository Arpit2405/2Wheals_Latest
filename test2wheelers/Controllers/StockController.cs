using Microsoft.AspNetCore.Mvc;

namespace _2whealers.Controllers
{
    public class StockController : Controller
    {
        public IActionResult AddStock()
        {
            return View();
        }
    }
}
