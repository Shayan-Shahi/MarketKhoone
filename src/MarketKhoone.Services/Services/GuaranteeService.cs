using AutoMapper;
using MarketKhoone.Common.Helpers;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels;
using MarketKhoone.ViewModels.Guarantees;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class GuaranteeService : GenericService<Guarantee>, IGuaranteeService
    {
        #region Constructor
        private readonly DbSet<Guarantee> _guarantees;
        private readonly IMapper _mapper;
        public GuaranteeService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _guarantees = uow.Set<Guarantee>();
        }
        #endregion

        public async Task<ShowGuaranteesViewModel> GetGuarantees(ShowGuaranteesViewModel model)
        {
            var guarantees = _guarantees.AsNoTracking().AsQueryable();

            #region Search

            guarantees = ExpressionHelpers.CreateSearchExpressions(guarantees, model.SearchGuarantees, callDeletedStatusExpression: false);

            #endregion

            #region OrderBy

            guarantees = guarantees.CreateOrderByExpression(model.SearchGuarantees.Sorting.ToString(),
                model.SearchGuarantees.SortingOrder.ToString());

            #endregion

            var paginationResult = await GenericPaginationAsync(guarantees, model.Pagination);

            return new()
            {
                Guarantees = await _mapper.ProjectTo<ShowGuaranteeViewModel>(
                    paginationResult.Query
                ).ToListAsync(),
                Pagination = paginationResult.Pagination
            };
        }

        public Task<List<ShowSelect2DataByAjaxViewModel>> SearchOnGuaranteesForSelect2(string input)
        {

            return _guarantees
                .Where(x => x.Title.Contains(input))
                .Where(x => x.IsConfirmed)
                .Select(x => new ShowSelect2DataByAjaxViewModel()
                {
                    Id = x.Id,
                    Text = x.FullTitle
                })
                .OrderBy(x => x.Id)
                .Take(20)
                .ToListAsync();
        }

        public Task<EditGuaranteeViewModel> GetForEdit(long id)
        {
            return _mapper.ProjectTo<EditGuaranteeViewModel>(_guarantees.Where(x => x.Id == id))
                .SingleOrDefaultAsync();
        }

        public Task<GuaranteeDetailsViewModel> GetGuaranteeDetails(long brandId)
        {
            return _mapper.ProjectTo<GuaranteeDetailsViewModel>(_guarantees.AsNoTracking())
                .SingleOrDefaultAsync(x => x.Id == brandId);
        }

        public Task<Guarantee> GetInActiveGuarantee(long id)
        {
            return _guarantees.Where(x => !x.IsConfirmed)
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}
