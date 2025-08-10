namespace InventoryHub.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class SearchController : Controller
    {

        [HttpGet]
        public IActionResult Index([FromQuery(Name = "q")] string? q)
        {
            ViewData["SearchQuery"] = q;
            return View(model: q ?? string.Empty);
        }
    }
}