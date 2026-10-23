using FibiEmlakDanismanlik.Dto.ContactDtos;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.CustomerContactViewComponents
{
    public class _CustomerContactViewComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke() => View(new ResultCustomerContactDto());
    }
}
