using MarketKhoone.Common.Constants;
using MarketKhoone.Common.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.FeatureConstantValues;

public class ShowFeatureConstantValuesViewModel
{
    public List<ShowFeatureConstantValueViewModel> FeatureConstantValues { get; set; }

    public SearchFeatureConstantValuesViewModel SearchFeatureConstantValues { get; set; }
        = new();

    public PaginationViewModel Pagination { get; set; }
        = new();
}

public class ShowFeatureConstantValueViewModel
{
    [Display(Name = "شناسه")]
    public long Id { get; set; }

    [Display(Name = "عنوان ویژگی")]
    public string FeatureTitle { get; set; }

    [Display(Name = "عنوان دسته بندی")]
    public string CategoryTitle { get; set; }

    [Display(Name = "مقدار")]
    public string Value { get; set; }
}

public class SearchFeatureConstantValuesViewModel
{
    [EqualSearch]
    [Display(Name = "دسته بندی")]
    //چرا ناله؟
    // در لود اول صفحه، مقدار سلکت باکس که روی انتخاب کنید است
    //میخواهیم همه فیچر ها رو در دیتا تیبل نشون بده
    // در 
    //[EqualSearch]
    //گفتیم اگر مقدار نال ببود، کلا سرچی انجام نده
    public long? CategoryId { get; set; }

    public List<SelectListItem> Categories { get; set; }

    //میگیم وقتی یه سلکت باکسی انتحاب شد
    //برو و همه ی ویژگی ای این دسته بندی
    // رو سلکت کن و بیار این فیچر آیدی برای اینه

    //الان چون مقدار فیچر آیدی غیر نال ایبیل هست
    // در سمت ویوو برای سلکت باید حتما مقدار رو صفر ست کنیم

    [Display(Name = "ویژگی")]
    public long FeatureId { get; set; }

    [ContainsSearch]
    [Display(Name = "مقدار")]
    [MaxLength(200, ErrorMessage = AttributesErrorMessages.MaxLengthMessage)]
    public string Value { get; set; }

    [Display(Name = "وضعیت حذف شده ها")]
    public DeletedStatus DeletedStatus { get; set; }

    [Display(Name = "نمایش بر اساس")]
    public SortingBrands Sorting { get; set; }

    [Display(Name = "مرتب سازی بر اساس")]
    public SortingOrder SortingOrder { get; set; }
}

public enum SortingBrands
{
    [Display(Name = "شناسه")]
    Id,

    [Display(Name = "عنوان ویژگی")]
    FeatureTitle,

    [Display(Name = "مقدار")]
    Value
}