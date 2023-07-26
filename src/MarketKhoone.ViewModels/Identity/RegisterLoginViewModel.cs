using MarketKhoone.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Identity;

public class RegisterLoginViewModel
{
    [Display(Name = "شماره تلفن")]
    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    [RegularExpression(@"[\d]{11}", ErrorMessage = AttributesErrorMessages.RegularExpressionMessage)]
    [MaxLength(200, ErrorMessage = AttributesErrorMessages.MaxLengthMessage)]
    public string PhoneNumberOrEmail { get; set; }

    [Display(Name = "قوانین و قرارداد را  به صورت کامل خوانده و قبول دارم")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "شما باید قوانین و مقرررات را تایید نمایید")]
    public bool AcceptToTheTerms { get; set; }
}
