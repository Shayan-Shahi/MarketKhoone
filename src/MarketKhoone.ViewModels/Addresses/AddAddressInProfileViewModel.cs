using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Addresses
{
    public class AddAddressInProfileViewModel
    {

        [MaxLength(200)]
        [Display(Name = "نام")]
        public string FirstName { get; set; }

        [MaxLength(200)]
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        [MaxLength(11)]
        [Display(Name = "شماره همراه")]
        public string PhoneNumber { get; set; }

        [Display(Name = "استان")]
        public long ProvinceId { get; set; }

        public List<SelectListItem> Provinces { get; set; }

        [Display(Name = "شهر")]
        public long CityId { get; set; }

        public List<SelectListItem> Cities { get; set; }

        [Display(Name = "آدرس")]
        [MaxLength(1000)]
        public string AddressLine { get; set; }

        [Display(Name = "کد پستی")]
        [MaxLength(10)]
        public string Pob { get; set; }

        [Display(Name = "آدرس پیش فرض")]
        public bool IsDefault { get; set; }
    }
}
