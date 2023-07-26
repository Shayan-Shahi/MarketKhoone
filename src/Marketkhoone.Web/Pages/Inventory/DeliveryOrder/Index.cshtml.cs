using AutoMapper;
using EShopMarket.Common.Helpers;
using MarketKhoone.Common;
using MarketKhoone.Common.Attributes;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities.Enums.Order;
using MarketKhoone.Entities.Enums.ParcelPostEnums;
using MarketKhoone.Entities.Enums.Product;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Orders;
using MarketKhoone.ViewModels.ParcelPosts;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.Inventory.DeliveryOrder
{
    [CheckModelStateInRazorPages]
    public class IndexModel : DeliveryOrderBasePanel
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;
        private readonly IMapper _mapper;
        public readonly IUnitOfWork _uow;

        public IndexModel(IFacadeServices facadeServices, IMapper mapper, IUnitOfWork uow)
        {
            _facadeServices = facadeServices;
            _mapper = mapper;
            _uow = uow;
        }

        #endregion
        [BindProperty(SupportsGet = true)]
        public ShowOrdersInDeliveryOrdersViewModel Orders { get; set; } = new();
        public async Task OnGet()
        {
            var provinces = await _facadeServices.ProvinceAndCityService.GetProvincesToShowInSelectBoxAsync();
            Orders.Provinces = provinces.CreateSelectListItem(firstItemValue: string.Empty);
        }

        public async Task<IActionResult> OnGetGetDataTableAsync()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, PublicConstantStrings.ModelStateErrorMessage);
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }

            return Partial("List", await _facadeServices.OrderService.GetDeliveryOrders(Orders));
        }

        public async Task<IActionResult> OnGetGetCities(long provinceId)
        {
            if (provinceId == 0)
            {
                return Json(new JsonResultOperation(true, string.Empty)
                {
                    Data = new Dictionary<long, string>()
                });
            }

            if (provinceId < 1)
            {
                return Json(new JsonResult(false, "استان مورد نظر را به درستی وارد نمایید"));
            }

            if (!await _facadeServices.ProvinceAndCityService.IsExistsBy(
                    nameof(MarketKhoone.Entities.ProvinceAndCity.Id), provinceId))
            {
                return Json(new JsonResultOperation(false, "استان مورد نظر یافت نشد"));
            }

            var cities = await _facadeServices.ProvinceAndCityService.GetCitiesByProvinceIdInSelectBoxAsync(provinceId);

            return Json(new JsonResultOperation(true, string.Empty)
            {
                Data = cities
            });

        }

        public async Task<IActionResult> OnGetGetOrderDetails(long orderId)
        {
            if (orderId < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            var orderDetails = await _facadeServices.OrderService.GetOrderDetails(orderId);
            if (orderDetails is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            return Partial("../Inventory/Order/_OrderDetailsPartial", orderDetails);
        }

        public async Task<IActionResult> OnGetShowDeliveryToPostPartial(long id)
        {
            if (!await _facadeServices.ParcelPostService.IsExistsBy(nameof(MarketKhoone.Entities.ParcelPost.Id),
                    id))
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            return Partial("_DeliveryToPostPartial");
        }

        public async Task<IActionResult> OnPostChangeStatusToDeliveryToPost(DeliveryParcelPostToPostViewModel model)
        {
            var parcelPost = await _facadeServices.ParcelPostService.FindByIdAsync(model.Id);

            if (parcelPost is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            var order = await _facadeServices.OrderService.FindByIdWithIncludesAsync(parcelPost.OrderId,
                nameof(MarketKhoone.Entities.Order.ParcelPosts));

            parcelPost.Status = ParcelPostStatus.DeliveredToPost;
            if (parcelPost.Dimension != Dimension.UltraHeavy)
            {
                parcelPost.PostTrackingCode = model.PostTrackingCode;
            }

            var deliveredParcelPostsToPostCount =
                order.ParcelPosts.Count(x => x.Status == ParcelPostStatus.DeliveredToPost);

            if (order.ParcelPosts.Count == deliveredParcelPostsToPostCount)
            {
                order.Status = OrderStatus.CompletelyParcelsDeliveredToPost;
            }
            else
            {
                order.Status = OrderStatus.SomeParcelsDeliveredToPost;
            }

            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true,
                "وضعیت مرسوله مورد نظر به \" تحویل داده شده به اداره پست\"تغییر یافت"));
        }


        public async Task<IActionResult> OnPostDeliveredToClient(long orderId)
        {
            var order = await _facadeServices.OrderService.FindByIdWithIncludesAsync(orderId,
                nameof(MarketKhoone.Entities.Order.ParcelPosts));

            if (order is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            if (order.Status != OrderStatus.CompletelyParcelsDeliveredToPost)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            order.Status = OrderStatus.DeliveredToClient;

            foreach (var parcelPost in order.ParcelPosts)
            {
                parcelPost.Status = ParcelPostStatus.DeliveredToClient;
            }

            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "وضعیت سفارش مورد نظر به \"تحویل داده شده به مشتری\" تغییر یافت."));
        }
    }
}
