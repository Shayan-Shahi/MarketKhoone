using MarketKhoone.Common.Attributes;
using MarketKhoone.Common.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Products;

public class AddProductViewModel
{
    [HiddenInput]
    public long MainCategoryId { get; set; }

    [Display(Name = "برند محصول")]
    [Range(1, long.MaxValue, ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    public long BrandId { get; set; }

    [Display(Name = "اصالت کالا")]
    //اگه نال نباشه، موقع ارسال اطلاعات در حالتی که محصول اصله
    // اطلاعاتی به سمت اکشن نمیره و ایز مودل ایست ایز ولید نمیشه
    public bool? IsFake { get; set; }

    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    [Display(Name = "وزن بسته بندی")]
    [Range(1, 1000000, ErrorMessage = AttributesErrorMessages.RangeMessage)]
    public int PackWeight { get; set; }

    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    [Display(Name = "طول بسته بندی")]
    [Range(1, 20000, ErrorMessage = AttributesErrorMessages.RangeMessage)]
    public int PackLength { get; set; }

    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    [Display(Name = "عرض بسته بندی")]
    [Range(1, 20000, ErrorMessage = AttributesErrorMessages.RangeMessage)]
    public int PackWidth { get; set; }

    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    [Display(Name = "ارتفاع بسته بندی")]
    [Range(1, 10000, ErrorMessage = AttributesErrorMessages.RangeMessage)]
    public int PackHeight { get; set; }

    [Display(Name = "توضیحات کوتاه")]
    public string ShortDescription { get; set; }

    [Display(Name = "بررسی تخصصی")]
    public string SpecialtyCheck { get; set; }

    [Display(Name = "تصاویر محصول")]
    [FileRequired]
    [MaxFileSize(2, multiplePictures: true)]
    [IsImage(multiplePictures: true)]
    public List<IFormFile> Pictures { get; set; }

    [Display(Name = "ویدیو های محصول")]
    [MaxFileSize(10, multiplePictures: true)]
    [AllowExtensions(new[] { "mp4" }, new[] { "video/mp4" })]
    //چرا نیو گرفتیم؟ چون اگر کاربر برای یه محصولی ویدیویی نفرسته، هنگام اد کردن 
    //خطای آبجکت نال رفرنس میده
    public List<IFormFile> Videos { get; set; }
        = new();

    [Display(Name = "نام فارسی کالا")]
    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    [MaxLength(200, ErrorMessage = AttributesErrorMessages.MaxLengthMessage)]
    public string PersianTitle { get; set; }

    [Display(Name = "نام انگلیسی کالا")]
    [MaxLength(200, ErrorMessage = AttributesErrorMessages.MaxLengthMessage)]
    public string EnglishTitle { get; set; }
}