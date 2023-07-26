using AutoMapper;
using EShopMarket.Common.Helpers;
using MarketKhoone.Common.Constants;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.ProductStocks;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.Inventory.ProductStock
{
    public class AddProductStockByConsignmentModel : InventoryPanelBase
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public AddProductStockByConsignmentModel(IFacadeServices facadeServices, IMapper mapper, IUnitOfWork uow)
        {
            _facadeServices = facadeServices;
            _mapper = mapper;
            _uow = uow;
        }

        #endregion

        [BindProperty]
        public AddProductStockByConsignmentViewModel AddProductStock { get; set; }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPost()
        {
            if (!await _facadeServices.ConsignmentService.CanAddStockForConsignmentItems(AddProductStock.ConsignmentId))
            {
                return Json(new JsonResultOperation(false, "موجودی این محموله قادر به افزایش و تغییر نمی باشد"));
            }

            if (!await _facadeServices.ConsignmentItemService.IsExistsByProductVariantIdAndConsignmentId(
                    AddProductStock.ProductVariantId,
                    AddProductStock.ConsignmentId))
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }
            //قضیه 100 تا گوشی، 99 تاش جلوی محموله و یکیش انتهای محموله هست  
            var addOrUpdate = string.Empty;
            var productStock = await _facadeServices.ProductStockService.GetByProductVariantIdAndConsignmentId(
                AddProductStock.ProductVariantId,
                AddProductStock.ConsignmentId);

            if (productStock is null)
            {
                addOrUpdate = " افزایش";
                productStock = _mapper.Map<MarketKhoone.Entities.ProductStock>(AddProductStock);
                await _facadeServices.ProductStockService.AddAsync(productStock);
            }
            else
            {
                addOrUpdate = " ویرایش";
                productStock.Count = AddProductStock.Count;
            } 

            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, $"موجودی محصول مورد نظر با موفقیت{addOrUpdate} یافت "));
        }
    }
}
