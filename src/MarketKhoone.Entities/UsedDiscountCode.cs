using MarketKhoone.Entities.AuditableEntity;
using MarketKhoone.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketKhoone.Entities;

/// <summary>
/// موجودیتِ افرادی که از کد تخفیف استفاده کرده اند
/// کدام فرد در داخل کدام سفارش از کدام کد تخفیف استفاده کرده است
/// </summary>
[Table($"{nameof(UsedDiscountCode)}s")]
public class UsedDiscountCode : IAuditableEntity
{
    #region Properties

    public long UserId { get; set; }

    public long OrderId { get; set; }

    public long DiscountCodeId { get; set; }

    #endregion

    #region Relations

    public User User { get; set; }

    public Order Order { get; set; }

    public DiscountCode DiscountCode { get; set; }

    #endregion
}