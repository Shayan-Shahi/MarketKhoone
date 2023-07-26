using MarketKhoone.Common.Constants;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.ParcelPosts;

public class DeliveryParcelPostToPostViewModel
{
    [HiddenInput]
    public long Id { get; set; }

    [Display(Name = "کد رهگیری اداره پست")]
    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    [MaxLength(30, ErrorMessage = AttributesErrorMessages.MaxLengthMessage)]
    public string PostTrackingCode { get; set; }
}
