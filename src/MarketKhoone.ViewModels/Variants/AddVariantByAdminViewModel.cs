using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketKhoone.ViewModels.Variants
{
    public class AddVariantByAdminViewModel
    {

        [Required]
        [MaxLength(200)]
        [Display(Name="سایز یا رنگ ")]
        public string Value { get; set; }

        [MaxLength(7)]
        [Display(Name="هگزا کد رنگ")]
        public string ColorCode { get; set; }


    }
}
