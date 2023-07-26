using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Enums
{
    public enum SortingVariants
    {
        [Display(Name = "شناسه")]
        Id,

        [Display(Name = "مقدار")]
        Value
    }
}
