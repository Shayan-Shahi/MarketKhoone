using EShopMarket.Common.Helpers;
using MarketKhoone.Common.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Marketkhoone.Web.Pages
{
    public class PageBase : PageModel
    {
        public JsonResult Json(object input)
        {
            return new(input);
        }

        public JsonResult RecordNotFound(string message = null)
        {
            return Json(new JsonResultOperation(false, message ?? PublicConstantStrings.RecordNotFoundMessage));
        }

        public JsonResult JsonBadRequest(string message = null, object data = null)
        {
            return Json(new JsonResultOperation(false, message ?? "خطایی به وجود آمد")
            {
                Data = data
            });
        }

        public JsonResult JsonOk(string message, object data = null)
        {
            return Json(new JsonResultOperation(true, message)
            {
                Data = data
            });
        }
    }
}
