using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace WebApp.Controllers
{
    [Authorize]
    public class HomeController : BaseController<HomeController>
    {
       

        public HomeController()
        {

        }

        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


    }
}
