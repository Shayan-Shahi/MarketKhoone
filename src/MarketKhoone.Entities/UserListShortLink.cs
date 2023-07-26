using MarketKhoone.Entities.AuditableEntity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketKhoone.Entities;

/// <summary>
/// لینک کوتاه لیست های محصولات کاربران
/// </summary>
[Table($"{nameof(UserListShortLink)}s")]
[Index(nameof(Link), IsUnique = true)]
public class UserListShortLink : EntityBase, IAuditableEntity
{
    #region Properties

    [Required]
    [MaxLength(39)]
    public string Link { get; set; }

    [Required]
    [MaxLength(10)]
    public string DisplayLink { get; set; }

    public bool IsUsed { get; set; }

    #endregion

    #region Relations

    public UserList UserList { get; set; }

    #endregion
}