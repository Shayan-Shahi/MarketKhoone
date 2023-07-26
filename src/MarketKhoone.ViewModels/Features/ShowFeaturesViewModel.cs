using MarketKhoone.ViewModels.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Features;

public class ShowFeaturesViewModel
{
    public List<ShowFeatureViewModel> Features { get; set; }

    public SearchFeaturesViewModel SearchFeatures { get; set; }
        = new();

    public PaginationViewModel Pagination { get; set; }
    = new();
}

public class SearchFeaturesViewModel
{
    [Display(Name = "دسته بندی")]
    public long CategoryId { get; set; }

    [Display(Name = "عنوان")]
    public string Title { get; set; }

    public List<SelectListItem> Categories { get; set; }

    [Display(Name = "نمایش بر اساس")]
    public SortingFeatures SortingFeatures { get; set; }

    public DeletedStatus DeletedStatus { get; set; }

    [Display(Name = "مرتب سازی بر اساس")]
    public SortingOrder Sorting { get; set; }
}

public class ShowFeatureViewModel
{
    [Display(Name = "عنوان")]
    public string Title { get; set; }

    public long CategoryId { get; set; }

    [Display(Name = "شناسه")]
    public long Id { get; set; }
}

