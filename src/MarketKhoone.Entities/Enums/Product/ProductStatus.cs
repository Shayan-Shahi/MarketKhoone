using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.Entities.Enums.Product
{
    public enum ProductStatus : byte
    {
        [Display(Name = "در انتظار تایید اولیه")]
        AwaitingInitialApproval,

        [Display(Name = "تایید شده")]
        Confirmed,

        [Display(Name = "رد شده در حالت اولیه")]
        Rejected
    }
}
