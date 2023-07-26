using MarketKhoone.Entities.AuditableEntity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketKhoone.Entities;

//چرا
//ConsignmentId, ProductVariantId are are unique at the same time? 
//ما  در یک محموله گوشی موبایل سامسونگ اس بیست با رنگ قرمز داریم
//و باز هم یک گوشی موبایل سامسونگ اس بیست رنگ قرمز میخوایم اضافه کنیم
//نمیشه، بلکه به اون اولی باید یدونه اضافه کنیم. پس باید کانتش رو افزایش بدیم نه اینکه یکی دیگه بسازیم
[Table("ConsignmentItems")]
[Index(nameof(ConsignmentItem.ConsignmentId),
    nameof(ConsignmentItem.ProductVariantId), IsUnique = true)]
public class ConsignmentItem : EntityBase, IAuditableEntity
{
    #region Properties

    //کدوم تنوع قراره برای انبار های ما ارسال بشه
    public long ProductVariantId { get; set; }

    //این تنوعی که قراره ارسال بشه، متعلق به کدوم محموله هستش
    public long ConsignmentId { get; set; }
     
    // چند تا از این تنوع ها قراره ارسال بشه
    public int Count { get; set; }

    [Required]
    [MaxLength(40)]
    // 4--1
    // long.MaxValue = 19 char
    // ProductVariantId = long (19 char) -- SellerId = long (19 char)
    // 19 + --(2) + 19 = 40 char
    public string Barcode { get; set; }

    #endregion

    #region Relations

    public ProductVariant ProductVariant { get; set; }

    public Consignment Consignment { get; set; }

    #endregion
}