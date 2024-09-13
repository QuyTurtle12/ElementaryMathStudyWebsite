using Microsoft.AspNetCore.Mvc;

namespace ElementaryMathStudyWebsite.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
