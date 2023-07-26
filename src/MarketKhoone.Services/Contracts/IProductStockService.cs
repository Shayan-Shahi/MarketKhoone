using MarketKhoone.Entities;

namespace MarketKhoone.Services.Contracts
{
    public interface IProductStockService : IGenericService<ProductStock>
    {
        /// <summary>
        /// خواندن موجودی کالا ها برای افزایش موجودی در جدول تنوع محصولات
        /// برای اینکه همیشه به جدول موجودی کالا ها کووری نزینیم که یفهمییم موجودی کالاها چقدر است
        /// این مورد را به جدول تنوع مصولات اضافه میکنیم که موجودی کالا همیشه در دسترس باشه
        /// این متد در صفحه صبت نظر برای محموله استفاده میشه
        /// </summary>
        /// <param name="consignmentId"></param>
        /// <returns></returns>
        Task<Dictionary<long, int>> GetProductStocksForAddProductVariantsCount(long consignmentId);
        /// <summary>
        /// آیا در داخل حدول موجودی کالا برای این تنوع محصول و در این محموله رکوردی وجود دارد؟
        /// جعت استفاده در صفحه افزایش موجودی کالا
        /// اگر وجود داشته باشد همین رکورد را ویرایش کنیم و تعداد
        /// Count
        /// آنرا تغییر دهیم. چرا
        /// جون در یک تنوع نمیشه دو تا تنوع همسان وجود داشته باشه
        /// و ما فقط باید موجودی را افزایش دهیم
        /// </summary>
        /// <param name="productVariantId"></param>
        /// <param name="consignmentId"></param>
        /// <returns></returns>
        Task<ProductStock> GetByProductVariantIdAndConsignmentId(long productVariantId, long consignmentId);
    }
}
