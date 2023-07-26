using AutoMapper;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class UserProductFavoriteService : CustomGenericService<UserProductFavorite>, IUserProductFavoriteService
    {
        #region Constructor

        private readonly DbSet<UserProductFavorite> _userProductFavorites;
        private readonly IMapper _mapper;
        public UserProductFavoriteService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _userProductFavorites = uow.Set<UserProductFavorite>();
        }
        #endregion
    }
}
