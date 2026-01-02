using Microsoft.AspNetCore.Mvc;

namespace IRibeiroForHireAPI.Controllers;

public class AskController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}