using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Orders;

namespace MarketKhoone.Services.Contracts
{
    public interface IOrderService : IGenericService<Order>
    {
        Task<int> GetAllNewOrdersCount();

        Task<ShowOrdersInProfileViewModel> GetOrdersInProfile(ShowOrdersInProfileViewModel model);
        Task<ShowOrdersInProfileViewModel> GetOrdersInProfile(int pageNumber);
        Task<ShowOrdersInDeliveryOrdersViewModel> GetDeliveryOrders(ShowOrdersInDeliveryOrdersViewModel model);
        Task<OrderDetailsViewModel> GetOrderDetails(long orderId);
    }
}
