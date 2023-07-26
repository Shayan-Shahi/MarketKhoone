using MarketKhoone.Entities.AuditableEntity;
using MarketKhoone.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketKhoone.Entities;

/// <summary>
/// لیست های محصولات کاربران
/// </summary>
[Table($"{nameof(UserList)}s")]
[Index(nameof(Title), nameof(UserId),
    IsUnique = true)]
public class UserList : EntityBase, IAuditableEntity
{
    #region Properties

    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    public long UserId { get; set; }

    public long UserListShortLinkId { get; set; }

    #endregion

    #region Relations

    public User User { get; set; }

    public UserListShortLink UserListShortLink { get; set; }

    public ICollection<UserListProduct> UserListsProducts { get; set; }

    #endregion
}