using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreEx.Controllers
{
    public class TestController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetData(int index)
        {
            return Json(new { success = true, id = index }) ;
        }
    }
}
