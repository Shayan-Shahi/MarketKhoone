using MarketKhoone.Entities.AuditableEntity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketKhoone.Entities;

/// <summary>
/// جدول واسط بین محصولات و لیست محصولات کاربر
/// </summary>
[Table("UserListsProducts")]
public class UserListProduct : IAuditableEntity
{
    #region Properties

    public long UserListId { get; set; }

    public long ProductId { get; set; }

    #endregion

    #region Relations

    public UserList UserList { get; set; }

    public Product Product { get; set; }

    #endregion
}