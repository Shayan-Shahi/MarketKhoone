using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Consignments;

namespace MarketKhoone.Services.Contracts
{
    public interface IConsignmentService : IGenericService<Consignment>
    {
        Task<ShowConsignmentsViewModel> GetConsignments(ShowConsignmentsViewModel model);
        Task<ShowConsignmentDetailsViewModel> GetConsignmentDetails(long consignmentId);
        Task<Consignment> GetConsignmentForConfirmation(long consignmentId);
        Task<Consignment> GetConsignmentToChangesStatusToReceived(long consignmentId);
        Task<bool> IsExistsConsignmentWithReceivedStatus(long consignmentId);
        Task<Consignment> GetConsignmentWithReceivedStatus(long consignmentId);
        /// <summary>
        /// موقعی که انباردار موجودی یک کالا را از طریق محموله افزایش میدهد
        /// باید وضعیت آن محموله در حالت دریافت شده باشد و نه هیچ وضعیت دیگری
        /// این متود بررسی میکند که آیا محموله مورد نظر در وضعیت دریافت شده قار دارد یا خیر
        /// استفاده شده در صفحه افزایش موجودی توسط انباردار
        /// </summary>
        /// <param name="consignmentId"></param>
        /// <returns></returns>
        Task<bool> CanAddStockForConsignmentItems(long consignmentId);


    }
}
