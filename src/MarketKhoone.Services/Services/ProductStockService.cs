using AutoMapper;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class ProductStockService : GenericService<ProductStock>, IProductStockService
    {
        #region Constructor

        private readonly DbSet<ProductStock> _productStocks;
        private readonly IMapper _mapper;
        public ProductStockService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _productStocks = uow.Set<ProductStock>();
        }
        #endregion

        public Task<Dictionary<long, int>> GetProductStocksForAddProductVariantsCount(long consignmentId)
        {
            return _productStocks.Where(x => x.ConsignmentId == consignmentId)
                .ToDictionaryAsync(x => x.ProductVariantId, x => x.Count);
        }

        async Task<ProductStock> IProductStockService.GetByProductVariantIdAndConsignmentId(long productVariantId, long consignmentId)
        {
            return await _productStocks.AsNoTracking().AsSplitQuery()
                .Where(x => x.ProductVariantId == productVariantId)
                .SingleOrDefaultAsync(x => x.ConsignmentId == consignmentId);
        }


    }
}
