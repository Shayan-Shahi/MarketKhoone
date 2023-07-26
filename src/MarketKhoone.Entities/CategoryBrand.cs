using MarketKhoone.Entities.AuditableEntity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketKhoone.Entities;

[Table("CategoryBrands")]
[Index(nameof(CategoryBrand.CategoryId), nameof(CategoryBrand.BrandId),
    IsUnique = true)]
public class CategoryBrand : EntityBase, IAuditableEntity
{
    #region Properties

    public long CategoryId { get; set; }

    public long BrandId { get; set; }

    //چرا اینو توی این انتیتی نوشتیم؟
    // چون ما براساس
    //CategoryId, BrandId
    //میاییم و کمیسیون رو چک میکنیم
    public byte CommissionPercentage { get; set; }

    #endregion

    #region Relations

    public Category Category { get; set; }
    public Brand Brand { get; set; }

    #endregion
}