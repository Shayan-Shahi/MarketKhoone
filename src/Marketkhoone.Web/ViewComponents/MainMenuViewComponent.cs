using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.ViewComponents
{
    public class MainMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
