using MarketKhoone.Common.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.FeatureConstantValues;

public class EditFeatureConstantValueViewModel
{
    [HiddenInput]
    public long Id { get; set; }

    [Display(Name = "دسته بندی")]
    [Range(1, long.MaxValue, ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    public long CategoryId { get; set; }

    public List<SelectListItem> Categories { get; set; }

    [Display(Name = "ویژگی")]
    [Range(1, long.MaxValue, ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    public long FeatureId { get; set; }

    public List<SelectListItem> Features { get; set; }

    [Display(Name = "مقدار")]
    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    [MaxLength(200, ErrorMessage = AttributesErrorMessages.MaxLengthMessage)]
    public string Value { get; set; }
}