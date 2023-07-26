using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.Entities.Enums.Seller
{
    public enum DocumentStatus : byte
    {
        [Display(Name = "در انتظار تایید اولیه")]
        AwaitingInitialApproval,

        [Display(Name = "تایید شده")]
        Confirmed,

        [Display(Name = "رد شده در حالت اولیه")]
        Rejected,

        [Display(Name = "در انتظار تایید فروشنده سیستم")]
        AwaitingApprovalSystemSeller,

        [Display(Name = "رد شده برای فروشنده  سیستم")]
        RejectedSystemSeller
    }
}
