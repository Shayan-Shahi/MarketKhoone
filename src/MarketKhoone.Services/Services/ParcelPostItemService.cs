using AutoMapper;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Entities.Enums.Order;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class ParcelPostItemService : CustomGenericService<ParcelPostItem>, IParcelPostItemService
    {

        #region Constructor

        private readonly DbSet<ParcelPostItem> _parcelPostItems;
        private readonly IUnitOfWork _uow;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public ParcelPostItemService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(uow)
        {
            _uow = uow;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _parcelPostItems = uow.Set<ParcelPostItem>();
        }

        #endregion

        public async Task<ShowProductsInProfileCommentViewModel> GetProductsInProfileComment(ShowProductsInProfileCommentViewModel model)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetLoggedInUserId();

            var parcelPostItems = _parcelPostItems.AsNoTracking()
                .Where(x => x.ParcelPost.Order.UserId == userId)
                .Where(x => x.ParcelPost.Order.Status == OrderStatus.DeliveredToClient)
                .Where(x => x.ProductVariant.Product.ProductComments.Any(pc => pc.UserId == userId) == false)
                .GroupBy(x => x.ProductVariant.ProductId)
                .OrderByDescending(x => x.First().CreatedDateTime);

            var paginationResult = await GenericPagination2Async(parcelPostItems, model.Pagination);

            return new()
            {
                Items = await _mapper.ProjectTo<ShowProductInProfileCommentViewModel>(paginationResult.Query)
                    .ToListAsync(),
                Pagination = paginationResult.Pagination
            };
        }

        public Task<ShowProductsInProfileCommentViewModel> GetProductsInProfileComment(int pageNumber)
        {
            var model = new ShowProductsInProfileCommentViewModel();
            model.Pagination.CurrentPage = pageNumber;
            return GetProductsInProfileComment(model);
        }
    }
}
