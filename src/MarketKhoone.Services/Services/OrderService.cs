using AutoMapper;
using MarketKhoone.Common.Helpers;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Entities.Enums.Order;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class OrderService : GenericService<Order>, IOrderService
    {
        #region Constructor

        private readonly DbSet<Order> _orders;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public OrderService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(uow)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _orders = uow.Set<Order>();
        }
        #endregion


        public async Task<int> GetAllNewOrdersCount()
        {
            var allNewOrdersCount = await _orders.AsNoTracking().CountAsync();
            return allNewOrdersCount;
        }

        public async Task<ShowOrdersInProfileViewModel> GetOrdersInProfile(ShowOrdersInProfileViewModel model)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetLoggedInUserId();

            var orders = _orders
                .Where(x => x.UserId == userId)
                .Where(x => x.Status == OrderStatus.DeliveredToClient)
                .Where(x => x.IsPay)
                .OrderByDescending(x => x.Id)
                .AsNoTracking()
                .AsQueryable();

            var paginationResult = await GenericPagination2Async(orders, model.Pagination);

            return new()
            {
                Orders = await _mapper.ProjectTo<ShowOrderInProfileViewModel>(
                    paginationResult.Query
                ).ToListAsync(),
                Pagination = paginationResult.Pagination
            };
        }

        public Task<ShowOrdersInProfileViewModel> GetOrdersInProfile(int pageNumber)
        {
            var model = new ShowOrdersInProfileViewModel();
            model.Pagination.CurrentPage = pageNumber;
            return GetOrdersInProfile(model);
        }

        public async Task<ShowOrdersInDeliveryOrdersViewModel> GetDeliveryOrders(ShowOrdersInDeliveryOrdersViewModel model)
        {
            var orders = _orders.Where(x => x.IsPay).AsNoTracking().AsQueryable();

            #region Search

            // We can't search (Contains) on [NotMapped] properties.

            var searchedFullTitle = model.SearchOrders.FullName;
            if (!string.IsNullOrWhiteSpace(searchedFullTitle))
            {
                orders = orders.Where(x => (x.Address.FirstName + " " + x.Address.LastName).Contains(searchedFullTitle));
            }

            var searchedProvinceId = model.SearchOrders.ProvinceId;
            if (searchedProvinceId is > 0)
            {
                orders = orders.Where(x => x.Address.ProvinceId == searchedProvinceId);
            }

            var searchedCityId = model.SearchOrders.CityId;
            if (searchedCityId is > 0)
            {
                orders = orders.Where(x => x.Address.CityId == searchedCityId);
            }

            if (model.SearchOrders.Status is null)
            {
                orders = orders.Where(x => x.Status != OrderStatus.WaitingForPaying)
                    .Where(x => x.Status != OrderStatus.Processing);
            }

            orders = ExpressionHelpers.CreateSearchExpressions(orders, model.SearchOrders, false);
            #endregion

            #region OrderBy

            orders = orders.CreateOrderByExpression(model.SearchOrders.SortingOrders.ToString(),
                model.SearchOrders.Sorting.ToString());

            #endregion

            var paginationResult = await GenericPaginationAsync(orders, model.Pagination);

            return new()
            {
                Orders = await _mapper.ProjectTo<ShowOrderInDeliveryOrdersViewModel>(paginationResult.Query)
                    .ToListAsync(),
                Pagination = paginationResult.Pagination
            };
        }

        public Task<OrderDetailsViewModel> GetOrderDetails(long orderId)
        {
            return _mapper.ProjectTo<OrderDetailsViewModel>(_orders
                .AsNoTracking()
                .AsSplitQuery()
            ).SingleOrDefaultAsync(x => x.Id == orderId);
        }
    }
}
