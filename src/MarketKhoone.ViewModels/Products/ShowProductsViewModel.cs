using MarketKhoone.Common.Helpers;
using MarketKhoone.Entities.Enums.Product;
using MarketKhoone.ViewModels.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Products;

public class ShowProductsViewModel
{
    public List<ShowProductViewModel> Products { get; set; }

    public SearchProductsViewModel SearchProducts { get; set; }
        = new();
    public PaginationViewModel Pagination { get; set; }
        = new();
}

public class ShowProductViewModel
{
    [Display(Name = "شناسه")]
    public long Id { get; set; }

    [Display(Name = "عنوان فارسی")]
    public string PersianTitle { get; set; }

    [Display(Name = "تصویر محصول")]
    public string MainPicture { get; set; }

    [Display(Name = "نام فروشگاه")]
    public string SellerShopName { get; set; }

    [Display(Name = "برند محصول")]
    public string BrandFullTitle { get; set; }

    [Display(Name = "وضعیت محصول")]
    public ProductStatus Status { get; set; }

    [Display(Name = "دسته بندی اصلی")]
    public string CategoryTitle { get; set; }

    [Display(Name = "کد محصول")]
    public int ProductCode { get; set; }
}

public class SearchProductsViewModel
{
    [EqualSearch]
    [Display(Name = "دسته بندی اصلی")]
    public long? MainCategoryId { get; set; }

    [EqualSearch]
    [Display(Name = "کد محصول")]
    //نال ایبل، یعنی اگه مقدارش نال بود
    //سرچ رو انجام نده
    public int? ProductCode { get; set; }
    //براساس اینکه محصول فیک هست یا نه سرچ انجام میدیم
    [EqualSearch]
    [Display(Name = "محصول اصل /غیر اصل")]
    public bool? IsFake { get; set; }

    public List<SelectListItem> Categories { get; set; }

    [ContainsSearch]
    [Display(Name = "عنوان فارسی")]
    [MaxLength(200)]
    public string PersianTitle { get; set; }

    [Display(Name = "نام فروشگاه")]
    [MaxLength(200)]
    public string ShopName { get; set; }

    [Display(Name = "وضعیت محصول")]
    public ProductStatus? Status { get; set; }

    [Display(Name = "وضعیت حذف شده ها")]
    public DeletedStatus DeletedStatus { get; set; }

    [Display(Name = "نمایش بر اساس")]
    public SortingProducts Sorting { get; set; }

    [Display(Name = "مرتب سازی بر اساس")]
    public SortingOrder SortingOrder { get; set; }
}

