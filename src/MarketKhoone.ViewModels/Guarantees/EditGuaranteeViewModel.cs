using MarketKhoone.Common.Attributes;
using MarketKhoone.Common.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Guarantees
{
    public class EditGuaranteeViewModel
    {
        [HiddenInput]
        public long Id { get; set; }

        [Display(Name = "عنوان گارانتی")]
        [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
        [MaxLength(200, ErrorMessage = AttributesErrorMessages.MaxLengthMessage)]
        public string Title { get; set; }

        [Display(Name = "تعداد ماه گارانتی")]
        public byte MonthsCount { get; set; }

        [Display(Name = "تصویر گارانتی از قبل بارگذاری شده")]
        public string Picture { get; set; }

        [Display(Name = "تصویر گارانتی جدید ")]
        [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
        [IsImage]
        [MaxFileSize(3)]
        public IFormFile NewPicture { get; set; }
    }
}
