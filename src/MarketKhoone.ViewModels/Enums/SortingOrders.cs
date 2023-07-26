using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Enums
{
    public enum SortingOrders
    {
        [Display(Name = "شماره سفارش")]
        OrderNumber,

        [Display(Name = "تاریخ ایجاد")]
        CreatedDateTime
    }
}
