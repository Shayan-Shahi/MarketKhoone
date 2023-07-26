using MarketKhoone.Entities;

namespace MarketKhoone.Services.Contracts
{
    public interface IProvinceAndCityService : IGenericService<ProvinceAndCity>
    {
        Task<Dictionary<long, string>> GetProvincesToShowInSelectBoxAsync();
        Task<Dictionary<long, string>> GetCitiesByProvinceIdInSelectBoxAsync(long provinceId);
        Task<(long, long)> GetForSeedData();

        Task<List<string>> GetProvinceForAutocomplete(string term);
    }
}
