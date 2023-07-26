using MarketKhoone.Entities.AuditableEntity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketKhoone.Entities;

[Index(nameof(Guarantee.Title), IsUnique = true)]
[Table("Guarantees")]
public class Guarantee : EntityBase, IAuditableEntity
{
    //اگر بخواهیم بدونیم که این گارانتی توسط کدوم فروشنده
    // ایجاد شده، میتونیم پراپرتی 
    //UserId
    //رو هم به این انتیتی اضافه کنیم، مثل انتیتی 
    //Brand
    #region Properties

    [Required]
    [MaxLength(200)]
    public string Title { get; set; }

    [NotMapped]
    public string FullTitle => $"گارانتی {MonthsCount} ماهه {Title}";

    public byte MonthsCount { get; set; }

    public bool IsConfirmed { get; set; }

    [Required]
    [MaxLength(50)]
    public string Picture { get; set; }

    #endregion

    #region Relations

    #endregion
}
