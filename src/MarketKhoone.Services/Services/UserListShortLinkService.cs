using AutoMapper;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class UserListShortLinkService : GenericService<UserListShortLink>, IUserListShortLinkService
    {
        #region Constructor

        private readonly DbSet<UserListShortLink> _listShortLinks;
        private readonly IMapper _mapper;
        public UserListShortLinkService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _listShortLinks = uow.Set<UserListShortLink>();
        }
        #endregion

        public Task<UserListShortLink> GetUserListShortLinkForCreateUserList()
        {
            return _listShortLinks
                .Where(x => !x.IsUsed)
                .OrderByDescending(x => Guid.NewGuid())
                .FirstAsync();
        }
    }
}
