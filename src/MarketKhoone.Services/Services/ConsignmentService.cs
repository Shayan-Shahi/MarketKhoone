using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketKhoone.Common.Helpers;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Entities.Enums.Consignment;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels;
using MarketKhoone.ViewModels.Consignments;
using MarketKhoone.ViewModels.Enums;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class ConsignmentService : GenericService<Consignment>, IConsignmentService
    {
        #region Cosntructor
        private readonly DbSet<Consignment> _consignments;
        private readonly IMapper _mapper;
        public ConsignmentService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _consignments = uow.Set<Consignment>();
        }
        #endregion

        public async Task<ShowConsignmentsViewModel> GetConsignments(ShowConsignmentsViewModel model)
        {
            var consignments = _consignments.AsNoTracking().AsQueryable();

            #region Search

            var searchedShopName = model.SearchConsignments.ShopName;
            if (!string.IsNullOrWhiteSpace(searchedShopName))
            {
                consignments = consignments.Where(x => x.Seller.ShopName.Contains(searchedShopName));
            }

            consignments = ExpressionHelpers.CreateSearchExpressions(consignments, model.SearchConsignments);

            #endregion

            #region OrderBy
             
            var isSortingAsc = model.SearchConsignments.Sorting == SortingOrder.Asc;
            if (model.SearchConsignments.SortingConsignments == SortingConsignments.ShopName)
            {
                if (isSortingAsc)
                    consignments = consignments.OrderBy(x => x.Seller.ShopName);
                else
                    consignments = consignments.OrderByDescending(x => x.Seller.ShopName);
            }
            else
            {
                consignments = consignments.CreateOrderByExpression(model.SearchConsignments.SortingConsignments.ToString(),
                    model.SearchConsignments.Sorting.ToString());
            }



            #endregion

            #region Pagination

            var paginationResult = await GenericPaginationAsync(consignments, model.Pagination);

            #endregion

            return new()
            {
                Consignments = await _mapper.ProjectTo<ShowConsignmentViewModel>(paginationResult.Query).ToListAsync(),
                Pagination = paginationResult.Pagination

            };

        }

        public Task<ShowConsignmentDetailsViewModel> GetConsignmentDetails(long consignmentId)
        {
            return _consignments.ProjectTo<ShowConsignmentDetailsViewModel>(
                    configuration: _mapper.ConfigurationProvider, parameters: new { consignmentId = consignmentId })
                .SingleOrDefaultAsync(x => x.Id == consignmentId);
          
        }

        public Task<Consignment> GetConsignmentForConfirmation(long consignmentId)
        {
            return _consignments
                .Where(x => x.ConsignmentStatus == ConsignmentStatus.AwaitingApproval)
                .SingleOrDefaultAsync(x => x.Id == consignmentId);
        }

        public Task<Consignment> GetConsignmentToChangesStatusToReceived(long consignmentId)
        {
            return _consignments.Where(x => x.ConsignmentStatus == ConsignmentStatus.ConfirmAndAwaitingForConsignment)
                .SingleOrDefaultAsync(x => x.Id == consignmentId);
        }

        public Task<bool> IsExistsConsignmentWithReceivedStatus(long consignmentId)
        {
            return _consignments.Where(x => x.ConsignmentStatus == ConsignmentStatus.Received)
                .AnyAsync(x => x.Id == consignmentId);
        }

        public Task<Consignment> GetConsignmentWithReceivedStatus(long consignmentId)
        {
            return _consignments.Where(x => x.ConsignmentStatus == ConsignmentStatus.Received)
                .SingleOrDefaultAsync(x => x.Id == consignmentId);
        }

        public Task<bool> CanAddStockForConsignmentItems(long consignmentId)
        {
            return _consignments.Where(x => x.ConsignmentStatus == ConsignmentStatus.Received)
                .AnyAsync(x => x.Id == consignmentId);
        }
    }
}
