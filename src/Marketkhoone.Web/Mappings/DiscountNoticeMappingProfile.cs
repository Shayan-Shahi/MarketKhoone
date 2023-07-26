using AutoMapper;
using MarketKhoone.Entities;
using MarketKhoone.ViewModels.DiscountNotices;

namespace Marketkhoone.Web.Mappings
{
    public class DiscountNoticeMappingProfile : Profile
    {
        public DiscountNoticeMappingProfile()
        {
            this.CreateMap<DiscountNotice, AddDiscountNoticeViewModel>().ReverseMap();
        }
    }
}
