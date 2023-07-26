using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels.Carts;
using MarketKhoone.ViewModels.Products;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class CartService : CustomGenericService<Cart>, ICartService
    {
        #region Constructor

        private readonly DbSet<Cart> _carts;
        private readonly IMapper _mapper;
        public CartService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _carts = uow.Set<Cart>();
        }
        #endregion

        public Task<List<ProductVariantInCartForProductInfoViewModel>> GetProductVariantsInCart(List<long> productVariantsIds, long userId)
        {
            return _mapper.ProjectTo<ProductVariantInCartForProductInfoViewModel>(_carts
                .Where(x => x.UserId == userId)
                .Where(x => productVariantsIds.Contains(x.ProductVariantId))).ToListAsync();
        }

        public Task<List<ShowCartInDropDownViewModel>> GetCartsForDropDown(long userId)
        {
            return _carts.AsNoTracking()
                .Where(x => x.UserId == userId)
                .ProjectTo<ShowCartInDropDownViewModel>(
                    configuration: _mapper.ConfigurationProvider, parameters: new { now = DateTime.Now }
                ).ToListAsync();
        }

        public Task<List<ShowCartInCartPageViewModel>> GetCartsForCartPage(long userId)
        {
            return _carts.AsNoTracking()
                .Where(x => x.UserId == userId)
                .ProjectTo<ShowCartInCartPageViewModel>(
                    configuration: _mapper.ConfigurationProvider, parameters: new { now = DateTime.Now }).ToListAsync();
        }

        public Task<List<Cart>> GetAllCartItems(long userId)
        {
            return _carts.Where(x => x.UserId == userId).ToListAsync();
        }

        public Task<List<ShowCartInCheckoutPageViewModel>> GetCartsForCheckoutPage(long userId)
        {
            return _mapper.ProjectTo<ShowCartInCheckoutPageViewModel>(_carts
                .AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.UserId == userId)).ToListAsync();
        }

        public Task<List<ShowCartInPaymentPageViewModel>> GetCartsForPaymentPage(long userId)
        {
            return _carts.AsNoTracking()
                .Where(x => x.UserId == userId).ProjectTo<ShowCartInPaymentPageViewModel>(
                    configuration: _mapper.ConfigurationProvider, parameters: new { now = DateTime.Now }).ToListAsync();
        }

        public Task<List<ShowCartForCreateOrderAndPayViewModel>> GetCartForCreateOrderAndPay(long userId)
        {
            return _carts.AsNoTracking()
                .Where(x => x.UserId == userId)
                .ProjectTo<ShowCartForCreateOrderAndPayViewModel>(
                    configuration: _mapper.ConfigurationProvider, parameters: new { now = DateTime.Now }).ToListAsync();
        }
    }
}
