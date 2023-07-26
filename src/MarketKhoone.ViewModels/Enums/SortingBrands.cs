using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Enums
{
    public enum SortingBrands
    {
        [Display(Name = "شناسه")]
        Id,

        [Display(Name = "نام فارسی برند")]
        TitleFa,

        [Display(Name = "نام انگلیسی برند")]
        TitleEn,

        [Display(Name = "نوع برند")]
        IsIranianBrand,

        [Display(Name = "لینک سایت قوه قضاییه")]
        JudiciaryLink,

        [Display(Name = "لینک سایت معتبر خارجی")]
        BrandLinkEn
    }
}
