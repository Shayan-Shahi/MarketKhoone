using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.Entities.Enums.Consignment
{
    public enum ConsignmentStatus : byte
    {
        [Display(Name = "در انتظار تایید")]
        AwaitingApproval,

        [Display(Name = "تایید شده و در انتظار ارسال محموله")]
        ConfirmAndAwaitingForConsignment,

        [Display(Name = "دریافت شده")]
        Received,

        [Display(Name = "دریافت شده و موجودی افزایش یافته")]
        ReceivedAndAddStock,

        [Display(Name = "رد شده")]
        Rejected,

        [Display(Name = "لغو شده")]
        Canceled
    }
}
