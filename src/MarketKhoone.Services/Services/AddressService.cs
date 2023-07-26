using AutoMapper;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels.Addresses;
using MarketKhoone.ViewModels.Carts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class AddressService : GenericService<Address>, IAddressService
    {
        #region Constructor

        private readonly DbSet<Address> _addresses;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddressService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(uow)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _addresses = uow.Set<Address>();
        }
        #endregion

        public Task<List<ShowAddressInProfileViewModel>> GetAllUserAddresses()
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetLoggedInUserId();
            return _mapper.ProjectTo<ShowAddressInProfileViewModel>(_addresses.AsNoTracking()
                    .Where(x => x.UserId == userId))
                .ToListAsync();
        }

        public async Task<bool> RemoveUserAddress(long id)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetLoggedInUserId();
            var addressToRemove = await _addresses.Where(x => x.UserId == userId)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (addressToRemove is null)
                return false;

            _addresses.Remove(addressToRemove);
            return true;

        }

        public Task<AddressInCheckoutPageViewModel> GetAddressForCheckoutPage(long userId)
        {
            return _mapper.ProjectTo<AddressInCheckoutPageViewModel>(_addresses
                .Where(x => x.UserId == userId)).SingleOrDefaultAsync();
        }

        public async Task<(bool hasUserAddress, long AddressId)> GetAddressForCreateOrderAndyPay(long userId)
        {
            var address = await _addresses
                //.Where(x => x.UserId == userId)
                .Where(x => x.IsDefault)
                .Select(x => new
                {
                    x.Id,
                    x.UserId

                }).SingleOrDefaultAsync(x => x.UserId == userId);

            if (address is null)
                return (false, default);
            return (true, address.Id);
        }

        public Task<EditAddressInProfileViewModel> GetForEdit(long id)
        {

            var userId = _httpContextAccessor.HttpContext.User.Identity.GetUserId();
            return _mapper.ProjectTo<EditAddressInProfileViewModel>(_addresses.Where(x => x.UserId == userId)
                    .Where(x => x.Id == id)).SingleOrDefaultAsync();

        }
    }
}
