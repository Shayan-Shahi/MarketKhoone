using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.Entities.Enums.Seller
{
    public enum CompanyType : byte
    {
        [Display(Name = "سهامی عام")]
        PublicStock,

        [Display(Name = "سهامی خاص")]
        PrivateEquity,

        [Display(Name = "مسئولیت محدود")]
        LimitedResponsibility,

        [Display(Name = "تعاونی")]
        Cooperative,

        [Display(Name = "تضامنی")]
        Solidarity
    }
}
