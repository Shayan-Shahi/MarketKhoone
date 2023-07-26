using MarketKhoone.Common.Constants;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Sellers;

public class ConfirmationSellerPhoneNumberViewModel
{
    [Display(Name = "کد فعال سازی")]
    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    [RegularExpression(@"[\d]{6}", ErrorMessage = AttributesErrorMessages.RegularExpressionMessage)]
    [MaxLength(6, ErrorMessage = AttributesErrorMessages.MaxLengthMessage)]
    public string ActivationCode { get; set; }

    [Display(Name = "شماره تلفن")]
    [HiddenInput]
    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    public string PhoneNumber { get; set; }

    public byte SendSmsLastTimeMinute { get; set; }

    public byte SendSmsLastTimeSecond { get; set; }
}