using MarketKhoone.Entities;

namespace MarketKhoone.Services.Contracts
{
    public interface IConsignmentItemService : IGenericService<ConsignmentItem>
    {
        /// <summary>
        /// آیا رکوردی با این تنوع محصول، در آیتم های محموله وجود دارد یا خیر؟
        /// </summary>
        /// <param name="productVariantId"></param>
        /// <param name="consignmentId"></param>
        /// <returns></returns>
        Task<bool> IsExistsByProductVariantIdAndConsignmentId(long productVariantId, long consignmentId);
    }
}
