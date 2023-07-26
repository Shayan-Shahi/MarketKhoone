using AutoMapper;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class ConsignmentItemService : GenericService<ConsignmentItem>, IConsignmentItemService
    {
        #region Constructor

        private readonly DbSet<ConsignmentItem> _consignmentItems;
        private readonly IMapper _mapper;
        public ConsignmentItemService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _consignmentItems = uow.Set<ConsignmentItem>();
        }
        #endregion

        public Task<bool> IsExistsByProductVariantIdAndConsignmentId(long productVariantId, long consignmentId)
        {
            return _consignmentItems.AsNoTracking().AsSplitQuery()
                .Where(x => x.ProductVariantId == productVariantId)
                .AnyAsync(x => x.ConsignmentId == consignmentId);
        }
    }
}
