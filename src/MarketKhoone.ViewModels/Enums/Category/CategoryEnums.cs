using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Enums.Category
{
    public enum SortingCategories
    {
        [Display(Name = "شناسه")]
        Id,

        [Display(Name = "عنوان")]
        Title,

        [Display(Name = "آدرس دسته بندی")]
        Slug,

        [Display(Name = "نمایش در منو های اصلی")]
        ShowInMenusStatus,

        [Display(Name = "حذف شده ها")]
        IsDeleted
    }

    public enum ShowInMenusStatus
    {
        [Display(Name = "همه")]
        All,
        [Display(Name = "بله")]
        True,
        [Display(Name = "خیر")]
        False
    }
}
