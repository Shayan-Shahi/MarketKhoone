using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Enums
{
    public enum SortingAllProductsInSellerPanel
    {
        [Display(Name = "شناسه")]
        Id,

        [Display(Name = "عنوان")]
        PersianTitle,

        [Display(Name = "برند فارسی")]
        BrandFa,

        [Display(Name = "برند انگلیسی")]
        BrandEn
    }
}
