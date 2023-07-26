using MarketKhoone.Entities.AuditableEntity;
using MarketKhoone.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketKhoone.Entities;

[Table("UserProductsFavorites")]
public class UserProductFavorite : IAuditableEntity
{
    #region Properties

    public long UserId { get; set; }

    public long ProductId { get; set; }

    #endregion

    #region Relations

    public User User { get; set; }
    public Product Product { get; set; }

    #endregion
}