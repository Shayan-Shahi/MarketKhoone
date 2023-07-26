using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Enums.Seller
{
    public enum IsActiveStatus
    {
        [Display(Name = "نمایش همه")]
        All,

        [Display(Name = "فعال")]
        Active,

        [Display(Name = "غیر فعال")]
        Disabled
    }

    public enum IsRealPersonStatus
    {
        [Display(Name = "نمایش همه")]
        All,

        [Display(Name = "فقط اشخاص حقیقی")]
        IsRealPerson,

        [Display(Name = "فقط اشخاص حقوقی")]
        IsLegalPerson
    }

    public enum SortingSellers
    {
        [Display(Name = "شناسه")]
        Id,

        [Display(Name = "کد فروشنده")]
        SellerCode,

        [Display(Name = "استان")]
        Province,

        [Display(Name = "شهرستان")]
        City,

        [Display(Name = "نام فروشنده")]
        FullName,

        [Display(Name = "نام فروشگاه")]
        ShopName,

        [Display(Name = "تاریخ ثبت نام")]
        CreatedDateTime
    }
}
