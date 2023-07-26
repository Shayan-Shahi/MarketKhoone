using MarketKhoone.Entities.AuditableEntity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketKhoone.Entities;

[Table("CategoryVariants")]
public class CategoryVariant : IAuditableEntity
{
    #region Properties

    public long CategoryId { get; set; }

    public long VariantId { get; set; }

    #endregion

    #region Relations

    public Category Category { get; set; }

    public Variant Variant { get; set; }

    #endregion
}