using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Guarantees
{
    public class GuaranteeDetailsViewModel
    {
        [Display(Name = "شناسه")]
        public long Id { get; set; }

        [Display(Name = "عنوان کامل گارانتی")]
        public string FullTitle { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "وضعیت")]
        public bool IsConfirmed { get; set; }

        [Display(Name = "تصویر")]
        public string Picture { get; set; }

        [Display(Name = "تعداد ماه گارانتی")]
        public byte MonthsCount { get; set; }
    }
}
