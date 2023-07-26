using MarketKhoone.Entities;
using MarketKhoone.ViewModels;
using MarketKhoone.ViewModels.Guarantees;

namespace MarketKhoone.Services.Contracts
{
    public interface IGuaranteeService : IGenericService<Guarantee>
    {
        Task<ShowGuaranteesViewModel> GetGuarantees(ShowGuaranteesViewModel model);
        Task<List<ShowSelect2DataByAjaxViewModel>> SearchOnGuaranteesForSelect2(string input);
        Task<EditGuaranteeViewModel> GetForEdit(long id);
        Task<GuaranteeDetailsViewModel> GetGuaranteeDetails(long brandId);
        Task<Guarantee> GetInActiveGuarantee(long id);
    }
}
