using AutoMapper;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class UserListProductService : CustomGenericService<UserListProduct>, IUserListProductService
    {

        #region Constructor
        private readonly DbSet<UserListProduct> _userListProducts;
        private readonly IMapper _mapper;
        public UserListProductService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _userListProducts = uow.Set<UserListProduct>();
        }

        #endregion

        public Task<List<UserListProduct>> GetUserListProducts(long productId, List<long> allUserListIds)
        {
            return _userListProducts.Where(x => x.ProductId == productId)
                .Where(x => allUserListIds.Contains(x.UserListId))
                .ToListAsync();
        }
    }
}
