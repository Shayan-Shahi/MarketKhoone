using MarketKhoone.Entities;
using MarketKhoone.ViewModels.ProductShortLinks;

namespace MarketKhoone.Services.Contracts
{
    public interface IProductShortLinkService : IGenericService<ProductShortLink>
    {
        Task<ShowProductShortLinksViewModel> GetProductShortLinks(ShowProductShortLinksViewModel model);
        Task<ProductShortLink> GetForDelete(long shortLinkId);

        /// <summary>
        /// گرفتن یک لینک کوتاه بهص ورت شانسی برای زمانیکه
        /// یک محصول ایجاد میکنیم که محثول را به این لینک کوتاه متصل کنیم
        /// </summary>
        /// <returns></returns>
        Task<ProductShortLink> GetProductShortLinkForCreateProduct();
    }
}
