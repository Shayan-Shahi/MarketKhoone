using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Products;
using MarketKhoone.ViewModels.Search;
using MarketKhoone.ViewModels.Variants;

namespace MarketKhoone.Services.Contracts
{
    public interface IProductService : IGenericService<Product>
    {
        Task<int> GetTotalProductCounts();
        Task<ShowProductsViewModel> GetProducts(ShowProductsViewModel model);
        Task<ProductDetailsViewModel> GetProductDetails(long productId);
        Task<Product> GetProductToRemoveInManagingProducts(long id);
        Task<List<string>> GetPersianTitlesForAutocomplete(string term);
        /// <summary>
        /// گرفتن محصولات برای صفحه مقایسه
        /// </summary>
        /// <param name="productCode"></param>
        /// <returns></returns>
        Task<List<ShowProductInCompareViewModel>> GetProductForCompare(params int[] productCode);

        /// <summary>
        /// گرفتن محصولات برای مودال افزودن محصول
        /// شامل صفحه بندی و جستجوی متنی
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="searchValue"></param>
        /// <param name="productCodesToHide">چه محصولاتی باید نمایش داده نشوند، برای مثال اگر در
        /// داخل صفحه مقایسه محثول اول اضافه شده دیگر نباید در داخل مودال افزودن محصول، محصول اول را نمایش دهیم</param>
        /// <returns></returns>

        Task<ShowProductInComparePartialViewModel> GetProductsForAddProductInCompare(int pageNumber, string searchValue, int[] productCodesToHide);

        /// <summary>
        /// گرفتن آیدی دسته بندی محصول
        /// </summary>
        /// <param name="productCode"></param>
        /// <returns></returns>
        Task<long> GetProductCategoryId(long productCode);

        Task<ShowProductInfoViewModel> GetProductInfo(int productCode);
        Task<ShowProductsInSellerPanelViewModel> GetProductsInSellerPanel(ShowProductsInSellerPanelViewModel model);
        Task<int> GetProductCodeForCreateProduct();
        Task<List<Product>> GetProductsForChangeStatus(List<long> productIds);
        Task<ShowProductsInSearchOnCategoryViewModel> GetProductsByPaginationForSearch(SearchOnCategoryInputsViewModel inputs);
        Task<AddVariantViewModel> GetProductInfoForAddVariant(long productId);
        Task<ShowAllProductsInSellerPanelViewModel> GetAllProductsInSellerPanel(ShowAllProductsInSellerPanelViewModel model);

        Task<(int productCode, string slug)> FindByShortLink(string shortLinkToCompare);
        /// <summary>
        /// جستجو محصولات براساس نام فارسی محصول
        ///  استفاده شده در صفحه مدیریت محصولات فروشنده
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<string>> GetPersianTitlesForAutocompleteInSellerPanel(string input);
    }

}


