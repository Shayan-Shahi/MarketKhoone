using AutoMapper;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    internal class ProvinceAndCityService : GenericService<ProvinceAndCity>, IProvinceAndCityService
    {
        #region Constructor

        private readonly DbSet<ProvinceAndCity> _provinceAndCities;
        private readonly IMapper _mapper;
        public ProvinceAndCityService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _provinceAndCities = uow.Set<ProvinceAndCity>();
        }
        #endregion

        public Task<Dictionary<long, string>> GetProvincesToShowInSelectBoxAsync()
        {
            return _provinceAndCities.AsNoTracking().Where(x => x.ParentId == null)
                .ToDictionaryAsync(x => x.Id, x => x.Title);
        }

        public Task<Dictionary<long, string>> GetCitiesByProvinceIdInSelectBoxAsync(long provinceId)
        {

            return _provinceAndCities.AsNoTracking().Where(x => x.ParentId == provinceId)
                .ToDictionaryAsync(x => x.Id, x => x.Title);
        }

        public async Task<(long, long)> GetForSeedData()
        {
            var province = await _provinceAndCities.Where(x => x.ParentId == null)
                .Select(x => new
                {
                    x.Title,
                    x.Id
                })
                .SingleAsync(x => x.Title == "اصفهان");

            var city = await _provinceAndCities.Where(x => x.ParentId != null)
                .Select(x => new
                {
                    x.Title,
                    x.Id
                })
                .SingleAsync(x => x.Title == "کاشان");
            return (province.Id, city.Id);
        }

        Task<List<string>> IProvinceAndCityService.GetProvinceForAutocomplete(string term)
        {
            return _provinceAndCities
                .Where(x => term.Contains(term))
                .OrderBy(x => x.Id)
                .Take(10)
                .Select(x => x.Title)
                .ToListAsync();
        }



    }
}
