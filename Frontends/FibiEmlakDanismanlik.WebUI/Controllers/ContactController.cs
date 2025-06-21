using FibiEmlakDanismanlik.Dto.ContactDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            var dto = TempData["FormResult"] != null
                ? JsonConvert.DeserializeObject<ResultCustomerContactDto>((string)TempData["FormResult"])
                : new ResultCustomerContactDto();

            return View(dto);
        }


        [HttpPost]
        public async Task<IActionResult> CustomerContactForm(ResultCustomerContactDto dto)
        {
            TempData["FormResult"] = JsonConvert.SerializeObject(dto);
            return RedirectToAction("Index");
        }
    }
}
