using MarketKhoone.Common.Helpers;
using MarketKhoone.Entities.Enums.Consignment;
using MarketKhoone.ViewModels.Enums;
using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Consignments;

public class ShowConsignmentsViewModel
{
    public List<ShowConsignmentViewModel> Consignments { get; set; }

    public SearchConsignmentsViewModel SearchConsignments { get; set; }
        = new();

    public PaginationViewModel Pagination { get; set; }
        = new();
}

public class ShowConsignmentViewModel
{
    [Display(Name = "شناسه")]
    public long Id { get; set; }

    [Display(Name = "نام فروشگاه")]
    public string SellerShopName { get; set; }

    [Display(Name = "تاریخ تحویل")]
    public string DeliveryDate { get; set; }

    [Display(Name = "وضعیت محموله")]
    public ConsignmentStatus ConsignmentStatus { get; set; }
}

public class SearchConsignmentsViewModel
{
    [Display(Name = "نام فروشگاه")]
    public string ShopName { get; set; }

    [Display(Name = "تاریخ تحویل")]
    [EqualDateTimeSearch]
    public string DeliveryDate { get; set; }

    [EqualSearch]
    [Display(Name = "وضعیت محموله")]
    public ConsignmentStatus? ConsignmentStatus { get; set; }
    [Display(Name = "وضعیت حذف شده ها")]
    public DeletedStatus DeletedStatus { get; set; }

    [Display(Name = "نمایش بر اساس")]
    public SortingConsignments SortingConsignments { get; set; }

    [Display(Name = "مرتب سازی بر اساس")]
    public SortingOrder Sorting { get; set; }
}

