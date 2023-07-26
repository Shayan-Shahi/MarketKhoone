using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.Entities.Enums.User
{
    public enum Gender : byte
    {
        [Display(Name = "آقا")]
        Man,

        [Display(Name = "خانم")]
        Woman
    }
}
