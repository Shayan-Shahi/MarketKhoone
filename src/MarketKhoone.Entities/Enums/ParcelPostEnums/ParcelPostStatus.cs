using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.Entities.Enums.ParcelPostEnums
{
    public enum ParcelPostStatus : byte
    {
        [Display(Name = "در انتظار پرداخت")]
        WaitingForPaying,

        [Display(Name = "در حال پردازش")]
        Processing,

        [Display(Name = "پردازش انبار")]
        InventoryProcessing,

        [Display(Name = "تحویل به پست")]
        DeliveredToPost,

        [Display(Name = "تحویل شده")]
        DeliveredToClient
    }
}
