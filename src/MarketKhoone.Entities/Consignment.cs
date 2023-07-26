using MarketKhoone.Entities.AuditableEntity;
using MarketKhoone.Entities.Enums.Consignment;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketKhoone.Entities;

[Table("Consignments")]
public class Consignment : EntityBase, IAuditableEntity
{
    #region Properties

    //کدوم فروشنده قراره این محموله رو بفرسته
    public long SellerId { get; set; }
    //چه تاریخی قراراه بفرسته
    public DateTime DeliveryDate { get; set; }

    //انبار دار باید بتونه موقع تحویل، توضیحاتی برای اون محموله احیانا برای فروشنده بفرسته
    [Column(TypeName = "ntext")]
    public string Description { get; set; }

    public ConsignmentStatus ConsignmentStatus { get; set; }

    #endregion

    #region Relations

    public Seller Seller { get; set; }

    public ICollection<ConsignmentItem> ConsignmentItems { get; set; }
        = new List<ConsignmentItem>();

    #endregion
}

