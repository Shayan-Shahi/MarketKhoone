using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketKhoone.Common.Helpers;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Entities.Enums.Product;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels;
using MarketKhoone.ViewModels.Enums;
using MarketKhoone.ViewModels.Products;
using MarketKhoone.ViewModels.Search;
using MarketKhoone.ViewModels.Variants;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace MarketKhoone.Services.Services
{
    public class ProductService : GenericService<Product>, IProductService
    {

        #region Constructor
        private readonly DbSet<Product> _products;
        private readonly DbSet<Seller> _sellers;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(IUnitOfWork uow, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(uow)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _products = uow.Set<Product>();
            _sellers = uow.Set<Seller>();
        }
        #endregion


        public async Task<int> GetTotalProductCounts()
        {
            var productCounts = await _products.AsNoTracking().CountAsync();
            return productCounts;
        }

        public async Task<ShowProductsViewModel> GetProducts(ShowProductsViewModel model)
        {
            var products = _products.AsNoTracking().AsQueryable();


            //shopName
            //در ویوو مدل هست ولی در انیتتیه پروداکت نیست پس سرچ دستی مینویسیم
            #region Search

            var searchedShopName = model.SearchProducts.ShopName;
            if (!string.IsNullOrWhiteSpace(searchedShopName))
            {
                products = products.Where(x => x.Seller.ShopName.Contains(searchedShopName));
            }

            var searchedStatus = model.SearchProducts.Status;
            if (searchedStatus is not null)
            {
                products = products.Where(x => x.Status == searchedStatus);
            }

            products = ExpressionHelpers.CreateSearchExpressions(products, model.SearchProducts);

            #endregion

            #region OrderBy

            //سه تا از پراپرتی هایی که در ویو مدل 
            //ProductEnum
            //تعریف کردیم، دلخل انتیتیه پروداکت نیستند
            //پس باید دستی اوردربای کنیم
            var sorting = model.SearchProducts.Sorting;
            var isSortingAsc = model.SearchProducts.SortingOrder == SortingOrder.Asc;
            if (sorting == SortingProducts.ShopName)
            {
                if (isSortingAsc)
                    products = products.OrderBy(x => x.Seller.ShopName);
                else
                    products = products.OrderByDescending(x => x.Seller.ShopName);
            }
            else if (sorting == SortingProducts.BrandTitleFa)
            {
                if (isSortingAsc)
                    products = products.OrderBy(x => x.Brand.TitleFa);
                else
                    products = products.OrderByDescending(x => x.Brand.TitleFa);
            }
            else if (sorting == SortingProducts.BrandTitleEn)
            {
                if (isSortingAsc)
                    products = products.OrderBy(x => x.Brand.TitleEn);
                else
                    products = products.OrderByDescending(x => x.Brand.TitleEn);
            }
            else
            {
                products = products.CreateOrderByExpression(model.SearchProducts.Sorting.ToString(),
                model.SearchProducts.SortingOrder.ToString());
            }

            #endregion

            var paginationResult = await GenericPaginationAsync(products, model.Pagination);

            return new()
            {
                Products = await _mapper.ProjectTo<ShowProductViewModel>(
                        paginationResult.Query)
                    .ToListAsync(),
                Pagination = paginationResult.Pagination
            };
        }

        public Task<ProductDetailsViewModel> GetProductDetails(long productId)
        {
            //در صفحه جزییات محصول به فیچر ها هم نیاز داریم، اینکلود ها به خاطر همینه
            //ProductFeature
            //رو لود میکنه ولی
            //Feature 
            //رو لود نمیکنه در حالت عادی
            //return _mapper.ProjectTo<ProductDetailsViewModel>(_products
            //        .AsNoTracking().AsSplitQuery()
            //        .Include(x=>x.ProductFeatures)
            //        .ThenInclude(x=>x.Feature))
            //    .SingleOrDefaultAsync(x => x.Id == productId);

            //چون اومدیم  و اون دو تا پراپرتی ویوو مدل دار رو به
            //ProductDetailsViewModel
            //اد کردیم، دیکه خود دات نت میره جوین ها رو میزنه و اینکلود میکنه
            // و نیازی در این حالت به اینکلود نیست
            return _mapper.ProjectTo<ProductDetailsViewModel>(_products
                    .AsNoTracking().AsSplitQuery())
                    .SingleOrDefaultAsync(x => x.Id == productId);
        }

        public async Task<Product> GetProductToRemoveInManagingProducts(long id)
        {
            return await _products.Where(x => x.Status == ProductStatus.AwaitingInitialApproval)
                .Include(x => x.ProductMedia)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<string>> GetPersianTitlesForAutocomplete(string term)
        {
            return await _products.AsNoTracking().Take(20)
                .Where(x => x.PersianTitle.Contains(term))
                .Select(x => x.PersianTitle)
                .ToListAsync();
        }

        public Task<List<ShowProductInCompareViewModel>> GetProductForCompare(params int[] productCodes)
        {
            productCodes = productCodes.Where(x => x > 0).ToArray();
            return _mapper.ProjectTo<ShowProductInCompareViewModel>(
                _products.Where(x => productCodes.Contains(x.ProductCode))).ToListAsync();
        }

        public async Task<ShowProductInComparePartialViewModel> GetProductsForAddProductInCompare(int pageNumber, string searchValue, int[] productCodesToHide)
        {
            var result = new ShowProductInComparePartialViewModel();

            searchValue = searchValue?.Trim() ?? string.Empty;

            if (pageNumber < 1)
                pageNumber = 1;

            var firstProductCategoryId = await GetProductCategoryId(productCodesToHide.First());

            var query = _products
                .Where(x => searchValue == "" ||

                            (
                                x.PersianTitle.Contains(searchValue) || x.EnglishTitle.Contains(searchValue)
                            )
                )
                .AsNoTracking()
                .Where(x => productCodesToHide.Contains(x.ProductCode) == false)
                .Where(x => x.MainCategoryId == firstProductCategoryId)
                .OrderBy(x => x.Id);

            var itemsCount = await query.LongCountAsync();

            var take = 3;
            var pagesCount = (int)Math.Ceiling((decimal)itemsCount / take);

            if (pagesCount <= 0)
                pagesCount = 1;

            if (pageNumber >= pagesCount)
            {
                result.IsLastPage = true;
                pageNumber = pagesCount;
            }

            var skip = (pageNumber - 1) * take;
            result.Products = await _mapper.ProjectTo<ProductItemForShowProductInComparePartialViewModel>(
                query.Skip(skip).Take(take)).ToListAsync();

            result.PageNumber = pageNumber;
            result.Count = itemsCount;

            return result;
        }

        public Task<long> GetProductCategoryId(long productCode)
        {
            return _products.Where(x => x.ProductCode == productCode)
                .Select(x => x.MainCategoryId)
                .SingleOrDefaultAsync();
        }

        public Task<ShowProductInfoViewModel> GetProductInfo(int productCode)
        {
            long userId = 0;

            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                userId = _httpContextAccessor.HttpContext.User.Identity.GetLoggedInUserId();
            }

            return _products
                .AsNoTracking()
                .AsSplitQuery()
                .ProjectTo<ShowProductInfoViewModel>(
                    configuration: _mapper.ConfigurationProvider,
                    parameters: new { userId = userId, now = DateTime.Now })
                .SingleOrDefaultAsync(x => x.ProductCode == productCode);
        }

        public async Task<ShowProductsInSellerPanelViewModel> GetProductsInSellerPanel(ShowProductsInSellerPanelViewModel model)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetLoggedInUserId();

            var sellerId = await _sellers.Where(x => x.UserId == userId)
                .Select(x => x.Id)
                .SingleOrDefaultAsync();

            var products = _products.AsNoTracking()
                .Where(x => x.SellerId == sellerId || x.ProductVariants
                .Any(pv => pv.SellerId == sellerId)).AsQueryable();

            // این یا چیه؟
            // ||x.ProductVariants
            // محصولی برای یه فروشنده هست که یا خودش ساخته یا اومده از گرید دکمه شما هم فروشنده شوید رو زده
            // و براش تنوع اضاقه کرده

            #region SearchBy

            var searchedStatus = model.SearchProducts.Status;
            if (searchedStatus is not null)
            {
                products = products.Where(x => x.Status == searchedStatus);
            }

            products = ExpressionHelpers.CreateSearchExpressions(products, model.SearchProducts);

            #endregion


            #region OrderBy

            var sorting = model.SearchProducts.Sorting;
            var isSortingAsc = model.SearchProducts.SortingOrder == SortingOrder.Asc;

            if (sorting == SortingProductsInSellerPanel.BrandFa)
            {
                if (isSortingAsc)
                    products = products.OrderBy(x => x.Brand.TitleFa);
                else
                    products = products.OrderByDescending(x => x.Brand.TitleFa);
            }
            else if (sorting == SortingProductsInSellerPanel.BrandEn)
            {
                if (isSortingAsc)
                    products = products.OrderBy(x => x.Brand.TitleEn);
                else
                    products = products.OrderByDescending(x => x.Brand.TitleEn);
            }
            else
            {
                products = products.CreateOrderByExpression(model.SearchProducts.Sorting.ToString(),
                    model.SearchProducts.SortingOrder.ToString());
            }
            #endregion

            var paginationResult = await GenericPaginationAsync(products, model.Pagination);

            return new()
            {
                Products = await _mapper.ProjectTo<ShowProductInSellerPanelViewModel>(paginationResult.Query
                ).ToListAsync(),
                Pagination = paginationResult.Pagination
            };

        }

        public async Task<int> GetProductCodeForCreateProduct()
        {
            var latestproductCode = await _products
                .OrderBy(x=>x.Id)
                .Select(x => x.ProductCode).LastOrDefaultAsync();

            return latestproductCode + 1;
        }

        public Task<List<Product>> GetProductsForChangeStatus(List<long> productIds)
        {
            return _products
                .Where(x => productIds.Contains(x.Id)).ToListAsync();
        }

        public async Task<ShowProductsInSearchOnCategoryViewModel> GetProductsByPaginationForSearch(SearchOnCategoryInputsViewModel inputs)
        {
            var result = new ShowProductsInSearchOnCategoryViewModel();

            if (inputs.PageNumber < 1)
                inputs.PageNumber = 1;

            var productQuery = _products.AsNoTracking()
                .Where(x => x.Category.Slug == inputs.CategorySlug);

            if (inputs.Brands is { Count: > 0 })
            {
                productQuery = productQuery.Where(x => inputs.Brands.Contains(x.BrandId));
            }

            if (inputs.Variants is { Count: > 0 })
            {
                productQuery = productQuery.Where(x => x.ProductVariants
                    .Where(pv => pv.Count > 0)
                    .Any(pv => inputs.Variants.Contains(pv.VariantId.Value)));
            }
            // min:1000
            // max : 900
            //مین نباید بیت را مکس باشد
            //مین باید حداق مساوی یا کمتر باشد

            if (inputs.MaximumPrice >= inputs.MinimumPrice)
            {
                if (inputs.MinimumPrice > 0)
                {
                    productQuery = productQuery.Where(x => x.Price >= inputs.MinimumPrice);
                }

                if (inputs.MaximumPrice > 0)
                {
                    productQuery = productQuery.Where(x => x.Price <= inputs.MaximumPrice);
                }
            }

            if (inputs.OnlyExistsProducts)
            {
                productQuery = productQuery.Where(x => x.ProductStockStatus == ProductStockStatus.Available);
            }

            productQuery = productQuery.OrderBy(x => x.Id);
            var itemsCount = await productQuery.LongCountAsync();

            const byte take = 2;

            var pagesCount = (int)Math.Ceiling((decimal)itemsCount / take);

            if (pagesCount <= 0)
                pagesCount = 1;

            if (inputs.PageNumber >= pagesCount)
            {
                inputs.PageNumber = pagesCount;
            }

            var skip = (inputs.PageNumber - 1) * take;

            result.Products = await _mapper
                .ProjectTo<ShowProductInSearchOnCategoryViewModel>(productQuery.Skip(skip).Take(take)).ToListAsync();

            result.CurrentPage = inputs.PageNumber;
            result.PagesCount = pagesCount;

            return result;
        }

        public async Task<AddVariantViewModel> GetProductInfoForAddVariant(long productId)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetLoggedInUserId();

            var sellerId = await _sellers.Where(x => x.UserId == userId)
                .Select(x => x.Id)
                .SingleOrDefaultAsync();


            return await _products.AsNoTracking().AsSplitQuery()
                .ProjectTo<AddVariantViewModel>(
                    configuration: _mapper.ConfigurationProvider,
                    parameters: new { sellerId = sellerId }).SingleOrDefaultAsync(x => x.ProductId == productId);
        }

        public async Task<ShowAllProductsInSellerPanelViewModel> GetAllProductsInSellerPanel(ShowAllProductsInSellerPanelViewModel model)
        {
            var products = _products
                .Where(x => x.Status == ProductStatus.Confirmed).AsNoTracking().AsQueryable();

            #region Search

            var searchedStatus = model.SearchProducts.Status;
            if (searchedStatus is not null)
            {
                products = products.Where(x => x.Status == searchedStatus);
            }

            products = ExpressionHelpers.CreateSearchExpressions(products, model.SearchProducts);

            #endregion


            #region OrderBy

            var sorting = model.SearchProducts.SortingProducts;
            var isSortingAsc = model.SearchProducts.SortingOrder == SortingOrder.Asc;
            if (sorting == SortingAllProductsInSellerPanel.BrandFa)
            {
                if (isSortingAsc)
                    products = products.OrderBy(x => x.Brand.TitleFa);
                else
                {
                    products = products.OrderByDescending(x => x.Brand.TitleFa);
                }
            }
            else if (sorting == SortingAllProductsInSellerPanel.BrandEn)
            {
                if (isSortingAsc)
                    products = products.OrderBy(x => x.Brand.TitleEn);
                else
                {
                    products = products.OrderByDescending(x => x.Brand.TitleEn);
                }
            }
            else
            {
                products = products.CreateOrderByExpression(model.SearchProducts.SortingProducts.ToString()
                    , model.SearchProducts.SortingOrder.ToString());
            }

            #endregion


            #region Pagination

            var paginationResult = await GenericPaginationAsync(products, model.Pagination);

            #endregion

            return new()
            {
                Products = await _mapper.ProjectTo<ShowAllProductInSellerPanelViewModel>(paginationResult.Query)
                    .ToListAsync(),
                Pagination = paginationResult.Pagination
            };

        }

        public async Task<(int productCode, string slug)> FindByShortLink(string shortLinkToCompare)
        {
            var productShortLink = await _products
                .Select(x => new
                {
                    x.Slug,
                    x.ProductCode,
                    x.ProductShortLink
                }).SingleOrDefaultAsync(x => x.ProductShortLink.Link == shortLinkToCompare);

            return (productShortLink?.ProductCode ?? 0, productShortLink?.Slug);
        }

        public async Task<List<string>> GetPersianTitlesForAutocompleteInSellerPanel(string input)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetLoggedInUserId();

            var sellerId = await _sellers.Where(x => x.UserId == userId)
                .Select(x => x.Id)
                .SingleOrDefaultAsync();

            return await _products
                .Where(x=>x.SellerId == sellerId)
                .Where(x => x.PersianTitle.Contains(input))
                .AsNoTracking()
                .Take(20)
                .Select(x => x.PersianTitle)
                .ToListAsync();
        }
    }
}
