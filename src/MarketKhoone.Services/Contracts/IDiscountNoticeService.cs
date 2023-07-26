using MarketKhoone.Entities;
using MarketKhoone.ViewModels.DiscountNotices;

namespace MarketKhoone.Services.Contracts
{
    public interface IDiscountNoticeService : ICustomGenericService<DiscountNotice>
    {
        /// <summary>
        /// گرفتن اطلاعات برای بخش اطلاع رسانی شگفت انگیز
        /// برای مثال اگه اطلاع رسانی از ظریق شماره تلفن را از قبل فعال کرده باشد
        /// باید چک باکس مربوطه را تیک بزنیم
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<AddDiscountNoticeViewModel> GetDataForAddDiscountNotice(long productId, long userId);
    }
}
