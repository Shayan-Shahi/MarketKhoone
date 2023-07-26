using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketKhoone.ViewModels.Orders
{
    public class ReturnProductViewModel
    {
        public long OrderNumber { get; set; }

        public List<ParcelPostItemInReturnProduct> ParcelPostItems { get; set; }

    }

    
}

public class ParcelPostItemInReturnProduct
{
    public string ProductVariantProductPersianTitle { get; set; }
    public int Price { get; set; }
    public string ProductPicture { get; set; }
    public string ProductVariantSellerShopName { get; set; }
    public string GuaranteeTitle { get; set; }
    public string ProductVariantProductProductCode { get; set; }
    public string ProductVariantProductSlug { get; set; }
    public bool? ProductVariantVariantIsColor { get; set; }
    public string ProductVariantVariantColorCode { get; set; }
    public string ProductVariantVariantValue { get; set; }
}
