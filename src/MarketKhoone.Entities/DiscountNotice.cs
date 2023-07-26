using MarketKhoone.Entities.AuditableEntity;
using MarketKhoone.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketKhoone.Entities;

/// <summary>
/// اطلاع رسانی تخفیفات
/// اطلاع رسانی شگفت انگیز
/// </summary>
[Table($"{nameof(DiscountNotice)}s")]
public class DiscountNotice : IAuditableEntity
{
    #region Properties

    public long ProductId { get; set; }

    public long UserId { get; set; }

    public bool NoticeViaChat { get; set; }

    public bool NoticeViaEmail { get; set; }

    public bool NoticeViaPhoneNumber { get; set; }

    #endregion

    #region Relations

    public User User { get; set; }

    public Product Product { get; set; }

    #endregion
}