using AutoMapper;
using MarketKhoone.Common.Helpers;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels.ProductShortLinks;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class ProductShortLinkService : GenericService<ProductShortLink>, IProductShortLinkService
    {
        private readonly DbSet<ProductShortLink> _productShortLinks;
        private readonly IMapper _mapper;
        public ProductShortLinkService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _productShortLinks = uow.Set<ProductShortLink>();
        }

        public async Task<ShowProductShortLinksViewModel> GetProductShortLinks(ShowProductShortLinksViewModel model)
        {
            var productShortLinks = _productShortLinks.AsNoTracking().AsQueryable();

            #region Search

            productShortLinks = ExpressionHelpers
                .CreateSearchExpressions(productShortLinks, model.SearchProductShortLinks, callDeletedStatusExpression: false);

            #endregion

            #region OrderBy

            productShortLinks = productShortLinks.CreateOrderByExpression(model.SearchProductShortLinks.Sorting.ToString(),
                model.SearchProductShortLinks.SortingOrder.ToString());

            #endregion

            var paginationResult = await GenericPaginationAsync(productShortLinks, model.Pagination);

            return new()
            {
                ProductShortLinks = await _mapper.ProjectTo<ShowProductShortLinkViewModel>(
                    paginationResult.Query
                ).ToListAsync(),
                Pagination = paginationResult.Pagination
            };
        }

        public Task<ProductShortLink> GetForDelete(long shortLinkId)
        {
            return _productShortLinks
                .Where(x => !x.IsUsed)
                .SingleOrDefaultAsync(x => x.Id == shortLinkId);
        }

        public async Task<ProductShortLink> GetProductShortLinkForCreateProduct()
        {
            return await _productShortLinks.Where(x => !x.IsUsed)

                //برای جستجو رندم
                .OrderBy(x => Guid.NewGuid())
                .FirstAsync();
        }
    }
}
