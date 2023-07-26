using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Enums
{
    public enum SortingConsignments
    {
        [Display(Name = "شناسه")]
        Id,

        [Display(Name = "تاریخ تحویل")]
        DeliveryDate,

        [Display(Name = "نام فروشگاه")]
        ShopName
    }
}
