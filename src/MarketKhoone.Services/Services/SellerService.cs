using AutoMapper;
using MarketKhoone.Common.Helpers;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Entities.Enums.Seller;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels;
using MarketKhoone.ViewModels.Enums.Seller;
using MarketKhoone.ViewModels.Sellers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class SellerService : GenericService<Seller>, ISellerService
    {
        #region Constructor

        private readonly DbSet<Seller> _sellers;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public SellerService(IUnitOfWork uow, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(uow)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _sellers = uow.Set<Seller>();
        }
        #endregion
        public async Task<int> GetSellerCodeForCreateSeller()
        {
            var lastUserCode = await _sellers.AsNoTracking().OrderByDescending(x => x.Id)
                .Select(x => x.SellerCode).FirstOrDefaultAsync();
            return lastUserCode + 1;
        }

        public async Task<ShowSellersViewModel> GetSellers(ShowSellersViewModel model)
        {
            var sellers = _sellers.AsNoTracking().AsQueryable();

            #region Search

            sellers = ExpressionHelpers.CreateSearchExpressions(sellers, model.SearchSellers);

            #endregion

            #region OrderBy



            //چون ما در ویو مودل این بخش، یک سری پراپرتی داریم که د رجدول یوزر نیست
            // رفلکشنی که نوشتیم دیگه جواب نیست، و باید خودمون سرچ دستی بنویسیم
            // سه پراپرتی در ویو مدل هستن، ولی در انتیتی نیستن پس سرچ دستی مینویسیم

            //یه اینپوت جدا هم برای 
            //Full Name
            // داریم

            var searchedFullName = model.SearchSellers.UserFullName;
            if (!string.IsNullOrWhiteSpace(searchedFullName))
            {
                sellers = sellers.Where(x => (x.User.FirstName + " " + x.User.LastName).Contains(searchedFullName));
            }
            //پایان یه اینپوت جدا هم برای  
            //Full Name
            // داریم


            //برای 
            //ShopName
            //هم باید سرچ دستی بنویسیم
            //چرا؟ چون اینبار
            //ShopName
            //توی ویوو مدل نیست با اینکه توی انتیتی هست
            // میخواستیم بصورت گِت ریکویست وریفیکیشن توکن رو به سمت سرور بفرستیم، اومدیم و شاپ نیم رو از 
            // ویو مدل خارج کردیم و بصورت یه پراپرتی توی اکشن نوشتیم--خواشتسم این روش رو هم بریم برای افزایش مهارت برنامه نویسیمون

            var searchedShopName = model.SearchSellers.ShopName;
            if (!string.IsNullOrWhiteSpace(searchedShopName))
            {
                sellers = sellers.Where(x => x.ShopName.Contains(searchedShopName));
            }

            //براس چند تا از سلکت باکس ها هم باید سرچ دستی بنویسیم بنویسیم چرا؟
            // چون رفلکشن متد ما که ازش برای سرچ استفاده میکنیم
            // فقط برای دیلیلتید ایتَتوش کاستیمایز شده نه برای هر سلکت باکسی

            switch (model.SearchSellers.IsRealPersonStatus)
            {
                case IsRealPersonStatus.IsRealPerson:
                    sellers = sellers.Where(x => x.IsRealPerson);
                    break;
                case IsRealPersonStatus.IsLegalPerson:
                    sellers = sellers.Where(x => !x.IsRealPerson);
                    break;
            }

            switch (model.SearchSellers.IsActiveStatus)
            {
                case IsActiveStatus.Active:
                    sellers = sellers.Where(x => x.IsActive);
                    break;

                case IsActiveStatus.Disabled:
                    sellers = sellers.Where(x => !x.IsActive);
                    break;
            }

            // پایان سرچ مقادیر اومده از سلکت باکس های جدید 


            if (model.SearchSellers.SortingSellers == SortingSellers.Province)
            {
                if (model.SearchSellers.SortingOrder == SortingOrder.Asc)
                {
                    sellers = sellers.OrderBy(x => x.Province.Title);
                }
                else
                {
                    sellers = sellers.OrderByDescending(x => x.Province.Title);
                }
            }
            else if (model.SearchSellers.SortingSellers == SortingSellers.FullName)
            {
                if (model.SearchSellers.SortingOrder == SortingOrder.Asc)
                {
                    sellers = sellers.OrderBy(x => x.User.FirstName + " " + x.User.LastName);
                }
                else
                {
                    sellers = sellers.OrderByDescending(x => x.User.FullName + " " + x.User.LastName);
                }
            }
            else if (model.SearchSellers.SortingSellers == SortingSellers.City)
            {
                if (model.SearchSellers.SortingOrder == SortingOrder.Asc)
                {
                    sellers = sellers.OrderBy(x => x.City.Title);
                }
                else
                {
                    sellers = sellers.OrderByDescending(x => x.City.Title);
                }
            }
            else
            {
                sellers = sellers.CreateOrderByExpression(model.SearchSellers.SortingSellers.ToString(),
                    model.SearchSellers.SortingOrder.ToString());
            }

            #endregion



            #region Pagination

            var paginationResult = await GenericPaginationAsync(sellers, model.Pagination);

            #endregion

            return new()
            {
                Sellers = await _mapper.ProjectTo<ShowSellerViewModel>(paginationResult.Query).ToListAsync(),
                Pagination = paginationResult.Pagination
            };

        }

        public Task<SellerDetailsViewModel> GetSellerDetails(long sellerId)
        {
            return _mapper.ProjectTo<SellerDetailsViewModel>(_sellers.AsNoTracking())
                .SingleOrDefaultAsync(x => x.Id == sellerId);
        }

        public async Task<Seller> GetSellerToRemoveInManagingSellers(long id)
        {
            return await _sellers.Where(x => x.DocumentStatus == DocumentStatus.AwaitingInitialApproval)
                .Include(x=>x.User)
                .SingleOrDefaultAsync(x => x.Id == id);

            //چرا یوزر رو اینکلود کردیم؟
            //وقتی داریم یه فروشنده رو حذف میکنیم، یه فیلد داره در جدول یوز بنام
            //IsSeller
            //باید جدول یوزر رو هم اینکلود کنیم تا بتونبم سمت اکشن اون فیلد رو هم فالس کنیم
            // وقتی یه فروشنده حذف بشه، فروشنده نیست دیگه، ولی همچنان کاربر سیستم ماست که فیلد
            //IsSeller = true
            //دارد، نمیشه که!! پس باید این فیلد رو فالس کنیم
        }

        public async Task<List<string>> GetShopNamesForAutocomplete(string term)
        {
            return await _sellers.AsNoTracking()
                .OrderBy(x => x.Id)
                .Take(20)
                .Where(x => x.ShopName.Contains(term))
                .Select(x => x.ShopName)
                .ToListAsync();
        }

        public async Task<long?> GetSellerId2()
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetLoggedInUserId();

            var seller = await _sellers
                .Select(x => new
                {
                    x.Id,
                    x.UserId
                })
                .SingleOrDefaultAsync(x => x.UserId == userId);
            return seller?.Id;


        }

        public async Task<long> GetSellerId(long userId)
        {
            var seller = await _sellers
                .Select(x => new
                {
                    x.Id,
                    x.UserId
                })
                .SingleOrDefaultAsync(x => x.UserId == userId);
            return seller.Id;
        }

        public async Task<long> GetSellerId()
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetLoggedInUserId();

            var seller = await _sellers
                .Select(x => new
                {
                    x.Id,
                    x.UserId
                })
                .SingleOrDefaultAsync(x => x.UserId == userId);
            return seller.Id;
        }


        public override async Task<DuplicateColumns> AddAsync(Seller entity)
        {
            var result = new List<string>();
            if (await _sellers.AnyAsync(x => x.ShabaNumber == entity.ShabaNumber))
                result.Add(nameof(Seller.ShabaNumber));

            if (await _sellers.AnyAsync(x => x.ShopName == entity.ShopName))
                result.Add(nameof(Seller.ShopName));
            if (!result.Any())
                await base.AddAsync(entity);
            return new(!result.Any())
            {
                Columns = result
            };
        }


    }
}
