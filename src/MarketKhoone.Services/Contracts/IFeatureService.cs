using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Features;

namespace MarketKhoone.Services.Contracts
{
    public interface IFeatureService : IGenericService<Feature>
    {
        Task<ShowFeaturesViewModel> GetCategoryFeatures(ShowFeaturesViewModel features);
        Task<List<string>> AutocompleteSearch(string term);
        Task<Feature> FindByTitleAsync(string searchedTitle);

    }
}
