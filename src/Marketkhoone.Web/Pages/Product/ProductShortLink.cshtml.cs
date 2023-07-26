using MarketKhoone.Common.Constants;
using MarketKhoone.Services.Contracts.IFacadePattern;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.RegularExpressions;

namespace Marketkhoone.Web.Pages.Product
{
    public class ProductShortLinkModel : PageModel
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;
        public ProductShortLinkModel(IFacadeServices facadeServices)
        {
            _facadeServices = facadeServices;
        }

        #endregion

        public async Task<IActionResult> OnGet(string productsShortLink)
        {
            if (!Regex.IsMatch(productsShortLink, @"^[a-zA-Z0-9]{1,10}$"))
            {
                return RedirectToPage(PublicConstantStrings.Error404PageName);
            }

            var shortLinkInBytes = Encoding.UTF8.GetBytes(productsShortLink);
            var shortLinkToCompare = string.Join(".", shortLinkInBytes);
            var product = await _facadeServices.ProductService.FindByShortLink(shortLinkToCompare);
            if (product.slug is null)
            {
                return RedirectToPage(PublicConstantStrings.Error404PageName);
            }

            return RedirectToPagePermanent("/Product/Index", new
            {
                product.slug,
                product.productCode
            });
        }
    }
}
