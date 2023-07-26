using MarketKhoone.Entities.AuditableEntity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketKhoone.Entities;

[Table("Features")]
[Index(nameof(Feature.Title), IsUnique = true)]
public class Feature : EntityBase, IAuditableEntity
{
    #region Properties

    [Required]
    [MaxLength(150)]
    public string Title { get; set; }

    public bool ShowNextToProduct { get; set; }

    #endregion

    #region Relations

    public ICollection<CategoryFeature> CategoryFeatures { get; set; }
        = new List<CategoryFeature>();

    public ICollection<FeatureConstantValue> FeatureConstantValues { get; set; }

    #endregion
}