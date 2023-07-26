using System.Collections;
using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Brands;

namespace MarketKhoone.Services.Contracts
{
    public interface IBrandService : IGenericService<Brand>
    {
        Task<ShowBrandsViewModel> GetBrands(ShowBrandsViewModel model);
        Task<EditBrandViewModel> GetForEdit(long id);
        Task<List<string>> AutocompleteSearch(string term);
        //بعد اضافه کردن، درصد کمیسیون
        // این متد رو که لیست آیدی های برند رو برگشت میزد، کنار گذاشتیم
        Task<List<long>> GetBrandIdsByList(List<string> selectedBrands);

        // برای سلکت تو هااز دیکشنری ها استفاده میکنیم
        Task<Dictionary<long, string>> GetBrandsByCategoryId(long categoryId);
        Task<BrandDetailsViewModel> GetBrandDetails(long brandId);
        Task<Brand> GetInActiveBrand(long id);
        Task<Dictionary<long, string>> GetBrandsByFullTitle(List<string> brandTitles);
    }
}

