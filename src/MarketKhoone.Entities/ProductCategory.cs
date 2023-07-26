using MarketKhoone.Entities.AuditableEntity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketKhoone.Entities;

[Table("ProductCategories")]
public class ProductCategory : IAuditableEntity
{
    // کالای دیجیتال=>گوشی موبایل=>اس هفت
    //الان اس هفت والدش چیه؟
    //والدش در اصل گوشی موبایل و همچنین کالای دیجیتاله
    // ما میخوایم وقتی روی کالای دیجیتال کلیک کرد، بره خود اس هفت رو هم بیاره
    //میخواهیم وقتی روی دسته والد کلیک کردیم، بره و چایلد های دسته فرزند رو هم لود بکنه پس به این کلاس نیاز داریم
    // چایلد دسته فزند میشه، اس هفت--- دسته فرزند میشه گوشی موبایل

    #region Properties
    public long ProductId { get; set; }

    public long CategoryId { get; set; }

    #endregion

    #region Relations

    public Product Product { get; set; }

    public Category Category { get; set; }

    #endregion
}