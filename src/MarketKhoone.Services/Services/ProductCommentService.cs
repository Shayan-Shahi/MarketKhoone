using AutoMapper;
using MarketKhoone.Common.Helpers;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels;
using MarketKhoone.ViewModels.ProductComments;
using MarketKhoone.ViewModels.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class ProductCommentService : GenericService<ProductComment>, IProductCommentService
    {

        #region Constructor
        private readonly DbSet<ProductComment> _productComments;
        private readonly DbSet<Seller> _sellers;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductCommentService(IUnitOfWork uow, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(uow)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _productComments = uow.Set<ProductComment>();
            _sellers = uow.Set<Seller>();
        }

        public async Task<int> GetAllNewProductComments()
        {
            var allNewProductComments = await _productComments.AsNoTracking()
                .Where(x => x.IsConfirmed == false).CountAsync();

            return allNewProductComments;
        }

        public async Task<ShowProductCommentsInProfile> GetCommentsInProfileComment(ShowProductCommentsInProfile model)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetLoggedInUserId();
            var seller = await _sellers
                .Select(x => new
                {
                    x.Id,
                    x.UserId
                })
                .SingleOrDefaultAsync(x => x.UserId == userId);

            var parcelPostItems = _productComments.AsNoTracking()
                .Where(x => x.UserId == userId || x.SellerId == seller.Id)
                .OrderByDescending(x => x.Id);

            var paginationResult = await GenericPagination2Async(parcelPostItems, model.Pagination);

            return new()
            {
                Items = await _mapper.ProjectTo<ShowProductCommentInProfile>(
                    paginationResult.Query
                ).ToListAsync(),
                Pagination = paginationResult.Pagination
            };

        }

        public Task<ShowProductCommentsInProfile> GetCommentsInProfileComment(int pageNumber)
        {
            var model = new ShowProductCommentsInProfile();
            model.Pagination.CurrentPage = pageNumber;
            return GetCommentsInProfileComment(model);
        }

        public async Task<List<ProductCommentForProductInfoViewModel>> GetCommentsByPagination(long productId, int pageNumber, CommentsSortingForProductInfo sortBy,
            SortingOrder orderBy)
        {
            var query = _productComments
                .Where(x => x.IsConfirmed.Value)
                .Where(x => x.ProductId == productId);

            #region OrderBy

            if (sortBy == CommentsSortingForProductInfo.MostUseful)
            {
                //براساس تفریق دیسلایک از لایک
                //اگر یک کامنت 15 لایک داشته باشد و 14 دیسلایک حاصل میشه مثبت یک
                //ولی اگر یک کامنت فقط دو لایک بدون دیسلایک داشته باشد حاصل میشه مثبت دو
                //پس کامنتی که دولایک خالی دارد بالاتر از کامنتی که 15 لایک دارد نمایش داداه میشود
                query = query.OrderByDescending(x =>
                    x.CommentsScores.LongCount(c => c.IsLike) - x.CommentsScores.LongCount(c => !c.IsLike));
            }
            else
            {
                query = query.CreateOrderByExpression(sortBy.ToString(), orderBy.ToString());
            }

            #endregion

            query = await GenericPaginationAsync(query, pageNumber, 2);

            return await _mapper.ProjectTo<ProductCommentForProductInfoViewModel>(query)
                .ToListAsync();
        }

        #endregion


    }
}
