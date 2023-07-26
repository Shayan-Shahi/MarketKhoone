using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class ParcelPostService : GenericService<ParcelPost>, IParcelPostService
    {
        #region Constructor
        private readonly DbSet<ParcelPost> _parcelPosts;
        public ParcelPostService(IUnitOfWork uow) : base(uow)
        {
            _parcelPosts = uow.Set<ParcelPost>();
        }
        #endregion
    }
}
