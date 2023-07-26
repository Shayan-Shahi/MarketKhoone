using MarketKhoone.Entities.AuditableEntity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketKhoone.Entities;

[Table("ProductShortLinks")]
[Index(nameof(ProductShortLink.Link), IsUnique = true)]
public class ProductShortLink : EntityBase, IAuditableEntity
{
    #region Properties
    //چرا اینجا برای رابطه زدن ننونشتیم
    //public long ProductId {get;set;}
    //چون اول پروداکت رو میسازیم و در حین ساختن پروداکت شورکات رو استفاده میکنیم
    // اینطوری نیست که وقتی داریم این جدول رو میسازیم بگیم بروی یه چیزی از جدول پروداکت بیار
    //یعنی پروداکت وابسته است به جدول پروداکت شورت لینک
    //وابسته آیدی رونی رو میخواد که بهش وابسته هست
    //مثلا در جدول سلر ما یه یوزر آیدی نوشتیم و گفتیم سلر به یوزر وابسته است
    // چرا نرفتیم آیدی سلر رو بزاریم تو یوزر؟ چون برای ساختن یوزر نیازی به سلر نداشتیم ولی برای ساخت
    //سلر نیاز داریم ببینییم این کدوم کاربره
    // الان در جدول پروداکت باید آیدی این جدولو بنویسم یعنی پراپرتی زیر رو باید بنویسیم
    //public long ProductShortLinkId {get;set;}
    //چون موقع ساخت یه پروداکت، شورت لینک مقدارشو لازم داریم
    [Required]
    [MaxLength(39)]
    public string Link { get; set; }

    //خروجی اسکی کد ASC Code
    //موضوع اینه که در پراپرتیه بالایی ما میاییم ، اسکی کد رو در داخل جدول سیو میکنیم
    //و موفع لود در گرید، به ما عدد رو نشون میده طبیعتا
    //  ما نمیدونیم عدد چیه، باید لینک رو ببینیم تا درکش کنیم
    //ما عملیات سرچ؛ دسته بندی و... رو براساس این پراپرتی انجام میدیم
    [Required]
    [MaxLength(10)]
    public string DisplayLink { get; set; }

    public bool IsUsed { get; set; }

    #endregion

    #region Relations

    public Product Product { get; set; }

    #endregion
}