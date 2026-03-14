using FibiEmlakDanismanlik.Dto.PropertyDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class CompareController : Controller
    {
        public IActionResult Index(string? ids, string? type)
        {
            var model = new ComparePageVm
            {
                RawIds = ids ?? string.Empty,
                CompareTypeKey = (type ?? string.Empty).Trim().ToLowerInvariant()
            };

            return View(model);
        }


    }
    }
