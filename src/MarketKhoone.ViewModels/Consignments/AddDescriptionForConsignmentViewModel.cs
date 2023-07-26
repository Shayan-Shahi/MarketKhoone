using MarketKhoone.Common.Attributes;
using MarketKhoone.Common.Constants;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Consignments;

public class AddDescriptionForConsignmentViewModel
{
    [Display(Name = "شناسه محموله")]
    [HiddenInput]
    [Range(1, long.MaxValue, ErrorMessage = AttributesErrorMessages.RangeMessage)]
    public long ConsignmentId { get; set; }

    [Display(Name = "توضیحات محموله")]
    [MakeTinyMceRequired]
    public string Description { get; set; }
}