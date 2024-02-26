using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public abstract class BaseController<T> : Controller where T : BaseController<T>
    {


    }
}
