using AutoMapper;
using AutoMapper.QueryableExtensions;
using DNTCommon.Web.Core;
using DNTPersianUtils.Core;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels.ProductVariants;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class ProductVariantService : GenericService<ProductVariant>, IProductVariantService
    {

        #region Constructor
        private readonly DbSet<ProductVariant> _productVariants;
        private readonly DbSet<Seller> _sellers;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductVariantService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(uow)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _productVariants = uow.Set<ProductVariant>();
            _sellers = uow.Set<Seller>();
        }
        #endregion


        public async Task<int> GetProductsSellingNow()
        {
            var count = await _productVariants.AsNoTracking()
                .AsQueryable().CountAsync(x => x.Count > 0);
            return count;
        }

        public async Task<List<GetProductVariantInCreateConsignmentViewModel>> GetProductVariantsForCreateConsignment(List<int> variantCodes)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetUserId();

            var sellerId = await _sellers.Where(x => x.UserId.ToString() == userId)
                .Select(x => x.Id).SingleOrDefaultAsync();

            return await _mapper.ProjectTo<GetProductVariantInCreateConsignmentViewModel>(
                _productVariants
                    .Where(x => x.SellerId == sellerId)
                    .Where(x => variantCodes.Contains(x.VariantCode))
            ).ToListAsync();
        }

        public async Task<ShowProductVariantInCreateConsignmentViewModel> GetProductVariantForCreateConsignment(int variantCode)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetUserId();

            var sellerId = await _sellers.Where(x => x.UserId.ToString() == userId)
                .Select(x => x.Id).SingleOrDefaultAsync();

            // این دو تا لاین 60 و 61 مشابه همند
            //var sellerId = _sellerService.GetSellerId().GetAwaiter().GetResult();
            //var sellerI = await _sellerService.GetSellerId();
            
            return await _mapper.ProjectTo<ShowProductVariantInCreateConsignmentViewModel>(_productVariants
                .Where(x => x.SellerId == sellerId))
                .SingleOrDefaultAsync(x => x.VariantCode == variantCode);
        }

        public Task<List<ShowProductVariantViewModel>> GetProductVariants(long productId)
        {
            return _mapper.ProjectTo<ShowProductVariantViewModel>(_productVariants
                .Where(x => x.ProductId == productId)).ToListAsync();
        }

        public async Task<EditProductVariantViewModel> GetDataForEdit(long productVariantId)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetUserId();

            var sellerId = await _sellers.Where(x => x.UserId.ToString() == userId)
                .Select(x => x.Id).SingleOrDefaultAsync();

            return await _productVariants.AsNoTracking()
                .Where(x => x.SellerId == sellerId)
                .ProjectTo<EditProductVariantViewModel>
                (
                    _mapper.ConfigurationProvider,
                    parameters: new { now = DateTime.Now })
                .SingleOrDefaultAsync(x => x.Id == productVariantId);
        }

        public async Task<ProductVariant> GetForEdit(long id)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetUserId();

            var sellerId = await _sellers.Where(x => x.UserId.ToString() == userId)
                .Select(x => x.Id).SingleOrDefaultAsync();


            return await _productVariants
                .Where(x => x.SellerId == sellerId)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<AddEditDiscountViewModel> GetDataForAddEditDiscount(long id)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetUserId();

            var sellerId = await _sellers.Where(x => x.UserId.ToString() == userId)
                .Select(x => x.Id).SingleOrDefaultAsync();

            var result = await _mapper.ProjectTo<AddEditDiscountViewModel>(_productVariants)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (result?.OffPercentage > 0)
            {
                var parsedDateTime = DateTime.Parse(result.StartDateTime);
                result.StartDateTimeEn = parsedDateTime.ToString("yyyy/MM/dd HH:mm");
                result.StartDateTime = parsedDateTime.ToShortPersianDateString().ToPersianNumbers();

                var parsedDateTime2 = DateTime.Parse(result.EndDateTime);
                result.EndDateTimeEn = parsedDateTime2.ToString("yyyy/MM/dd HH:mm");
                result.EndDateTime = parsedDateTime2.ToShortPersianDateString().ToPersianNumbers();
            }

            return result;
        }

        public Task<List<ProductVariant>> GetProductVariantsToAddCount(List<long> productVariantIds)
        {
            return _productVariants.Where(x => productVariantIds.Contains(x.Id))
                .ToListAsync();
        }

        public async Task<bool> IsThisVariantAddedForSeller(long? variantId, long productId)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetUserId();

            var sellerId = await _sellers.Where(x => x.UserId.ToString() == userId)
                .Select(x => x.Id).SingleOrDefaultAsync();

            return await _productVariants
                .Where(x => x.SellerId == sellerId)
                .Where(x => x.ProductId == productId)
                .AnyAsync(x => x.VariantId == variantId);
        }

        public async Task<int> GetVariantCodeForCreateProductVariant()
        {
            var LastProductVariantCode = await _productVariants.OrderByDescending(x => x.Id)
                .Select(x => x.VariantCode)
                .FirstOrDefaultAsync();

            return LastProductVariantCode + 1;
        }

        public Task<List<long>> GetAddedVariantToProductVariants(List<long> selectedVariants, long categoryId)
        {
            return  _productVariants
                .Where(x => x.VariantId != null && selectedVariants.Contains(x.VariantId.Value))
                .Where(x => x.Product.MainCategoryId == categoryId)
                .GroupBy(x => x.VariantId)
                .Select(x => x.First().VariantId.Value)
                .ToListAsync();

        }
    }
}
