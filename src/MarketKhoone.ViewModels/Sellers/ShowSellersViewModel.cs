using MarketKhoone.Common.Constants;
using MarketKhoone.Common.Helpers;
using MarketKhoone.Entities.Enums.Seller;
using MarketKhoone.ViewModels.Enums.Seller;
using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Sellers;

public class ShowSellersViewModel
{
    public List<ShowSellerViewModel> Sellers { get; set; }

    public SearchSellersViewModel SearchSellers { get; set; }
        = new();
    public PaginationViewModel Pagination { get; set; }
        = new();
}

public class SearchSellersViewModel
{

    [Display(Name = "کد فروشنده")]
    [ContainsSearch]
    public int? SellerCode { get; set; }

    [Display(Name = "نام فروشنده")]
    [MaxLength(500, ErrorMessage = AttributesErrorMessages.MaxLengthMessage)]
    public string UserFullName { get; set; }

    [Display(Name = "نام فروشگاه")]
    [MaxLength(200, ErrorMessage = AttributesErrorMessages.MaxLengthMessage)]
    public string ShopName { get; set; }

    [Display(Name = "شخص حقوقی / شخص حقیقی")]
    public IsRealPersonStatus IsRealPersonStatus { get; set; }

    [Display(Name = "فعال / غیر فعال")]
    public IsActiveStatus IsActiveStatus { get; set; }

    [Display(Name = "وضعیت مدارک")]
    public DocumentStatus? DocumentStatus { get; set; }

    public DeletedStatus DeletedStatus { get; set; }

    [Display(Name = "نمایش بر اساس")]
    public SortingSellers SortingSellers { get; set; }

    [Display(Name = "مرتب سازی بر اساس")]
    public SortingOrder SortingOrder { get; set; }
}



public class ShowSellerViewModel
{
    [Display(Name = "شماره همراه")]
    public string UserPhoneNumber { get; set; }

    [Display(Name = "شناسه")]
    public long Id { get; set; }

    [Display(Name = "شخص حقوقی / شخص حقیقی")]
    public bool IsRealPerson { get; set; }

    [Display(Name = "نام فروشگاه")]
    public string ShopName { get; set; }

    [Display(Name = "نام فروشنده")]
    public string UserFullName { get; set; }

    [Display(Name = "کد فروشنده")]

    public int SellerCode { get; set; }

    [Display(Name = "استان و شهرستان")]
    public string ProvinceAndCity { get; set; }

    [Display(Name = "وضعیت")]
    public DocumentStatus DocumentStatus { get; set; }

    [Display(Name = "فعال / غیر فعال")]
    public bool IsActive { get; set; }

    [Display(Name = "تاریخ ثبت نام")]
    public string CreatedDateTime { get; set; }
}