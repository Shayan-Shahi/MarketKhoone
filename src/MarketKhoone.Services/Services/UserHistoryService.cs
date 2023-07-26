using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Entities.Enums.Product;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels.UserHistories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class UserHistoryService : CustomGenericService<UserHistory>, IUserHistoryService
    {
        #region Constructor
        private readonly DbSet<UserHistory> _userHistories;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserHistoryService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(uow)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userHistories = uow.Set<UserHistory>();
        }
        #endregion

        public Task<List<ShowUserHistoryViewModel>> GetUserHistories()
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetLoggedInUserId();

            return _userHistories.Where(x => x.UserId == userId)
                .AsNoTracking()
                .Where(x => x.Product.ProductStockStatus == ProductStockStatus.Available)
                .OrderBy(x => x.CreatedDateTime)
                .Take(10)
                .ProjectTo<ShowUserHistoryViewModel>(
                    configuration: _mapper.ConfigurationProvider,
                    parameters: new { userId })
                .ToListAsync();
        }
    }
}
