using MarketKhoone.Common.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.FeatureConstantValues;

public class AddFeatureConstantValueViewModel
{
    [Display(Name = "دسته بندی")]
    [Range(1, long.MaxValue, ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    public long CategoryId { get; set; }
    //در
    //OnGetAddFeatureConstantValue
    //یه سلکت باکس داریم، که در لودینگ مودال در سلکت باکس اولی، تمامی دسته یندی ها
    //را به کاربر نشون میدیم تا انتخاب کنه یه دستی بندی رو
    //براساس دسته بندی انتخاب شده توسط کاربر، لیست ویژگی های مربوط به اون دسته بندی رو 
    //در سلکت باکس دومی پر میکنیم
    // تا در اینپوت سوم، کاربر مقدار عددی ثابت را وارد کند
    public List<SelectListItem> Categories { get; set; }

    [Display(Name = "ویژگی")]
    [Range(1, long.MaxValue, ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    public long FeatureId { get; set; }

    [Display(Name = "مقدار")]
    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    [MaxLength(200, ErrorMessage = AttributesErrorMessages.MaxLengthMessage)]
    public string Value { get; set; }
}