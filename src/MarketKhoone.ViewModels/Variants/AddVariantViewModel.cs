using MarketKhoone.Common.Attributes;
using MarketKhoone.Common.Constants;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Variants;

public class AddVariantViewModel
{
    [HiddenInput]
    //برای کدوم محصول این تنوع قراره اضافه بشه
    public long ProductId { get; set; }

    public string Slug { get; set; }

    public int ProductCode { get; set; }

    //از چه تنوعی برای این محصول قراره استفاده یشه
    [Display(Name = "تنوع")]
    [HiddenInput]
    [Range(1, long.MaxValue, ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    public long VariantId { get; set; }

    //از چه گارانتی برای این محصول قراره استفاده یشه
    [Display(Name = "گارانتی")]
    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    [Range(1, long.MaxValue, ErrorMessage = AttributesErrorMessages.RegularExpressionMessage)]
    public long GuaranteeId { get; set; }
    //این تنوعی که قراره اضافه بشه، قیمتش چنده؟  
    [Display(Name = "قیمت")]
    [Range(1, 2000000000, ErrorMessage = AttributesErrorMessages.RangeMessage)]
    [DivisibleBy10]
    public int Price { get; set; }

    [Display(Name = "حداکثر تعداد در سبد خرید")]
    [Range(1, short.MaxValue, ErrorMessage = AttributesErrorMessages.RangeMessage)]
    public short MaxCountInCart { get; set; }

    public string ProductTitle { get; set; }

    public string CategoryTitle { get; set; }

    public bool? CategoryIsVariantColor { get; set; }

    public string BrandFullTitle { get; set; }

    public byte CommissionPercentage { get; set; }

    public string MainPicture { get; set; }

    public List<ShowCategoryVariantInAddVariantViewModel> Variants { get; set; }

    /// <summary>
    /// این فروشنده برای این محصول چه تنوع هایی را اضافه کرده است ؟
    /// اگر برای مثال تنوع قرمز رو برای یک محصول
    /// از قبل اضافه کرده نباید اجازه دهیم دوباره تنوع قرمز رو برای
    /// این محصول اضافه کنه
    /// </summary>
    public List<long?> AddedVariantsIds { get; set; }
}