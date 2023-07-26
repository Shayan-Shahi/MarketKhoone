using AutoMapper;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class UsedDiscountCodeService : CustomGenericService<UsedDiscountCode>, IUsedDiscountCodeService
    {
        #region Constructor

        private readonly DbSet<UsedDiscountCode> _usedDiscountCodes;
        private readonly IMapper _mapper;
        public UsedDiscountCodeService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _usedDiscountCodes = uow.Set<UsedDiscountCode>();
        }
        #endregion

    }
}
