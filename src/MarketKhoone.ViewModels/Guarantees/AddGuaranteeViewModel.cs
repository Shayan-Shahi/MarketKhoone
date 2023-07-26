using MarketKhoone.Common.Attributes;
using MarketKhoone.Common.Constants;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Guarantees
{
    public class AddGuaranteeViewModel
    {
        [Display(Name = "عنوان گارانتی")]
        [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
        [MaxLength(200, ErrorMessage = AttributesErrorMessages.MaxLengthMessage)]
        public string Title { get; set; }

        [Display(Name = "تعداد ماه گارانتی")]
        public byte MonthsCount { get; set; }

        [Display(Name = "تصویر گارانتی")]
        [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
        [IsImage]
        [MaxFileSize(3)]
        public IFormFile Picture { get; set; }
    }
}
