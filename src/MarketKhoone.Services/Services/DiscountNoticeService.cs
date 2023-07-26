using AutoMapper;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels.DiscountNotices;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class DiscountNoticeService : CustomGenericService<DiscountNotice>, IDiscountNoticeService
    {
        #region Constructor

        private readonly DbSet<DiscountNotice> _discountNotices;
        private readonly IMapper _mapper;
        public DiscountNoticeService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _discountNotices = uow.Set<DiscountNotice>();
        }
        #endregion

        public Task<AddDiscountNoticeViewModel> GetDataForAddDiscountNotice(long productId, long userId)
        {
            return _mapper.ProjectTo<AddDiscountNoticeViewModel>(_discountNotices
                .Where(x => x.UserId == userId))
                .SingleOrDefaultAsync(x => x.ProductId == productId);
        }
    }
}
