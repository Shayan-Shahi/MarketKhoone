using AutoMapper;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class CategoryVariantService : CustomGenericService<CategoryVariant>, ICategoryVariantService
    {
        #region Constructor
        private readonly DbSet<CategoryVariant> _categoryVariants;
        private readonly IMapper _mapper;
        public CategoryVariantService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _categoryVariants = uow.Set<CategoryVariant>();
        }
        #endregion

        public async Task<List<long>> GetCategoryVariants(long categoryId)
        {
            return await  _categoryVariants
                .Where(x => x.CategoryId == categoryId)
                .Select(x=>x.VariantId)
                .ToListAsync();
        }
    }
}
