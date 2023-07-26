using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.Entities.Enums.Product
{
    public enum ProductStockStatus : byte
    {
        /// <summary>
        /// همینکه محصول ایجاد بشه
        /// وضعیت محصول در حالت جدید قرار میگیره
        /// </summary>
        [Display(Name = "جدید")]
        New,

        /// <summary>
        /// اگر انباردار موجودی رو افزایش بده
        /// و وضعیت محصول در حالت نا موجود باشه
        /// وضعیت موجودی محصول رو به حالت موجود تغییر میدیم
        /// </summary>
        [Display(Name = "موجود")]
        Available,

        /// <summary>
        /// اگر محصول جدید باشد و یک فروشنده تنوعی برای آن اضافه کند
        /// وضعیت آن از حالت جدید به حالت نا موجود تغییر میکند
        /// </summary>
        [Display(Name = "ناموجود")]
        Unavailable
    }
}
