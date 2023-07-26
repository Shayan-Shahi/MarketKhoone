using AutoMapper;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class CommentReportService : CustomGenericService<CommentReport>, ICommentReportService
    {
        #region Constructor

        private readonly DbSet<CommentReport> _commentReports;
        private readonly IMapper _mapper;
        public CommentReportService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _commentReports = uow.Set<CommentReport>();
        }

        #endregion

    }
}
