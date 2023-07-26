using MarketKhoone.Entities.AuditableEntity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketKhoone.Entities;

[Table("ProductFeatures")]
[Index(nameof(ProductFeature.FeatureId), nameof(ProductFeature.ProductId), IsUnique = true)]
public class ProductFeature : EntityBase, IAuditableEntity
{
    
    #region Properties

    [Required]
    [MaxLength(2000)]
    public string Value { get; set; }

    public long ProductId { get; set; }

    public long FeatureId { get; set; }

    #endregion

    #region Relations

    public Product Product { get; set; }
    public Feature Feature { get; set; }

    #endregion
}

//ابعاد ۸.۹×۷۸.۱×۱۶۳.۴

// کارت حافظه جانبی عدم پشتیبانی

//وزن ۲۳۴ گرم

//فناوری صفحه‌نمایش  Dynamic AMOLED ۲X  

//بازه‌ی اندازه صفحه نمایش   ۶.۰ اینچ و بزرگتر

// اندازه ۶.۸ اینچ

