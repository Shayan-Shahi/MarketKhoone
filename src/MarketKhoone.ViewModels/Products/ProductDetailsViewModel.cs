using MarketKhoone.Common.Attributes;
using MarketKhoone.Entities.Enums.Product;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using MarketKhoone.Entities;

namespace MarketKhoone.ViewModels.Products;

public class ProductDetailsViewModel
{
    [HiddenInput]
    public long Id { get; set; }

    [Display(Name = "ابعاد")]
    public Dimension Dimension { get; set; }

    [Display(Name = "دلیل رد شدن محصول")]
    [MakeTinyMceRequired]
    public string RejectReason { get; set; }

    [Display(Name = "نام فارسی محصول")]
    public string PersianTitle { get; set; }

    [Display(Name = "نام انگلیسی محصول")]
    public string EnglishTitle { get; set; }

    public string Slug { get; set; }

    public int ProductCode { get; set; }

    public bool IsFake { get; set; }

    [Display(Name = "وزن بسته بندی")]
    public int PackWeight { get; set; }

    [Display(Name = "طول بسته بندی")]
    public int PackLength { get; set; }

    [Display(Name = "عرض بسته بندی")]
    public int PackWidth { get; set; }

    [Display(Name = "ارتفاع بسته بندی")]
    public int PackHeight { get; set; }   

    [Display(Name = "توضیحات مختصر محصول")]
    public string ShortDescription { get; set; }

    [Display(Name = "بررسی تخصصی محصول")]
    public string SpecialtyCheck { get; set; }

    [Display(Name = "نام فروشگاه")]
    public string SellerShopName { get; set; }

    [Display(Name = "برند محصول")]
    public string BrandFullTitle { get; set; }

    public ProductStatus Status { get; set; }

    public string CategoryTitle { get; set; }

    //public List<ProductMedia> ProductMedia { get; set; }
    //public List<ProductFeature> ProductFeatures { get; set; }

    //چرا این دو تا پراپرتی رو اینچوری نوشتیم؟
    //و مثلا به صورت دو تا پراپرتی بالایی که کامنت شده ننوشیتیم؟
    // تازه مجبو شدیم ویوو مدل هم تعریف کینیم؟

    //پاسخ: وقتی داریم به کل دو تا پراپرتی بالایی عمل میکنیم
    // در اصل داریم دو تا انیتتی رو لود میکنیم
    // در صورتی که هدف از ویوو مدل اینه که بره بایند کنه
    // اون اطلاعاتی از اون انتیتی رو که ما بهش  نیاز داریم. پس چون ما نمیخوایم کل انیتتی لود بشه
    //اومدیم ویوو مدل کاستوم شده تعریف کردیم.
    public List<ProductMediaForProductDetailsViewModel> ProductMedia { get; set; }

    public List<ProductFeatureForProductDetailsViewModel> ProductFeatures { get; set; }
}