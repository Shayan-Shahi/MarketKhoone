using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Enums
{
    public enum SortingProducts
    {
        [Display(Name = "شناسه")]
        Id,

        [Display(Name = "عنوان")]
        PersianTitle,


        //این موارد چون داخل انتیتیه پروداکت نیست
        //باید دستی 
        //OrderBy
        //بنویسیم
        #region Write handSearch

        [Display(Name = "نام فروشگاه")]
        ShopName,

        [Display(Name = "برند فارسی")]
        BrandTitleFa,

        [Display(Name = "برند انگلیسی")]
        BrandTitleEn

        #endregion
    }
}
