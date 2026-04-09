using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class FrequentlyAskedQuestionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
