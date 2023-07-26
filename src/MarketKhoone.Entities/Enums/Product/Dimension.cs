using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.Entities.Enums.Product
{
    public enum Dimension : byte
    {
        [Display(Name = "عادی")]
        Normal,

        [Display(Name = "کالا های بزرگ و سنگین")]
        Heavy,

        [Display(Name = "کالا های فوق سنگین")]
        UltraHeavy
    }
}
