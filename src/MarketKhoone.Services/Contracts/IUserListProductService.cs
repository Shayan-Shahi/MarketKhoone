using MarketKhoone.Entities;

namespace MarketKhoone.Services.Contracts
{
    public interface IUserListProductService : ICustomGenericService<UserListProduct>
    {
        Task<List<UserListProduct>> GetUserListProducts(long productId, List<long> allUserListIds);
    }
}
