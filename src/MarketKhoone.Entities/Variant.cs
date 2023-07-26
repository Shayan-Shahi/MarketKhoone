using MarketKhoone.Entities.AuditableEntity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketKhoone.Entities;

[Table("Variants")]
[Index(nameof(Variant.Value), nameof(Variant.ColorCode),
    IsUnique = true)]
public class Variant : EntityBase, IAuditableEntity
{
    #region Properties

    [Required]
    [MaxLength(200)]
    //میتونه رنگ باشه، میتنوه سایز 40 باشه
    public string Value { get; set; }

    //حالا رنگه یا سایزه؟
    //اگه نال باشه ؟ فرسی سایزه.
    public bool IsColor { get; set; }

    [MaxLength(7)]
    public string ColorCode { get; set; }

    /// <summary>
    /// اگر فروشنده درخواست تنوع جدید بکند
    /// این مورد فالس میشه تا زمانیکه ادمین این تنوع رو تایید کنه
    /// </summary>
    public bool IsConfirmed { get; set; }

    /// <summary>
    /// فروشنده پیشنهاد دهنده این تنوع
    /// </summary>
    public long? SellerId { get; set; }

    #endregion

    #region Relations

    public Seller Seller { get; set; }

    public ICollection<CategoryVariant> CategoryVariants { get; set; }

    #endregion
}