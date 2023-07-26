using MarketKhoone.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Consignments;

public class CreateConsignmentViewModel
{
    [Display(Name = "تاریخ تحویل")]
    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    public string DeliveryDate { get; set; }

    public List<string> Variants { get; set; }
        = new();

    //چرا نمونه سازی میکنیم
    // چون میخواهیم در اکشن پست برای ایجاد محموله
    // چی کنیم که ایا
    //CreateConsignment.Varaints
    //ناله یا نه
    //ویدیوی 145 دقیقه 1
}