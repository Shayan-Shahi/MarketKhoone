﻿using MarketKhoone.Entities.AuditableEntity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketKhoone.Entities;

[Table("ProductStocks")]
[Index(nameof(ProductStock.ConsignmentId),
    nameof(ProductStock.ProductVariantId), IsUnique = true)]
public class ProductStock : EntityBase, IAuditableEntity
{
    #region Properties

    public long ProductVariantId { get; set; }

    public long ConsignmentId { get; set; }

    public int Count { get; set; }

    #endregion

    #region Relations

    public ProductVariant ProductVariant { get; set; }

    public Consignment Consignment { get; set; }

    #endregion
}