namespace InventoryHub.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class SearchController : Controller
    {


        [HttpGet]
        public IActionResult Index(string q)
        {
            ViewData["SearchQuery"] = q;
            return View();
        }
    }
}