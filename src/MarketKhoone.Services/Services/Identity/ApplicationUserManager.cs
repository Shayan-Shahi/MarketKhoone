using AutoMapper;
using DNTPersianUtils.Core;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities.Identity;
using MarketKhoone.Services.Contracts.Identity;
using MarketKhoone.ViewModels.Sellers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MarketKhoone.Services.Services.Identity;

public class ApplicationUserManager
    : UserManager<User>, IApplicationUserManager
{
    private readonly DbSet<User> _users;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicationUserManager(
        IApplicationUserStore store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<User> passwordHasher,
        IEnumerable<IUserValidator<User>> userValidators,
        IEnumerable<IPasswordValidator<User>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<ApplicationUserManager> logger,
        IUnitOfWork uow, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        : base(
            (UserStore<User, Role, ApplicationDbContext, long, UserClaim, UserRole, UserLogin, UserToken,
                RoleClaim>)store,
            optionsAccessor, passwordHasher, userValidators, passwordValidators,
            keyNormalizer, errors, services, logger)
    {
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _users = uow.Set<User>();
    }

    #region CustomClass
    public async Task<DateTime?> GetSendSmsLastTimeAsync(string phoneNumber)
    {
        var result = await _users.Select(x => new
        {
            x.UserName,
            x.SendSmsLastTime
        })
            .SingleOrDefaultAsync(x => x.UserName == phoneNumber);
        return result?.SendSmsLastTime;
    }

    public async Task<bool> CheckForUserIsSeller(string phoneNumber)
    {
        //کاربر وقتی شماره موبایلش را در صفحه فروشنده شو میزنه و کد تایید رو پاس میکنه
        // اینجا فیلد ایز سلر رو توی جدول یوزر ترو میکنه ولی هنوز نقش سلر نگرفته، چون ما مدارک سلر رو باید بررسی کنیم تا بعد
        //تایید یا رد ما بتونه به عنوان فروشنده ما انتحاب بشه یا نشه
        // ذاتا کاربری هم که نقش سلر رو داره دوباره اجازه نمیدیم به این اکشن درخواست بده، این اکشن یعنی اکشنی که به
        //فروشنده ی حال حاضر که تایید ما رو گرقته بعد بررسی مدارکش، میگه بیا و دوباره فروشنده شو!!که مگه باید چند بار ثبت نام بکنه.
        return await _users.Where(x => x.PhoneNumber == phoneNumber)
            //هیچ کدوم از نقش های کاربر نباید سلر باشند
            .Where(x => x.UserRoles.All(r => r.Role.Name != ConstantRoles.Seller))
            .AnyAsync(x => x.IsSeller);
    }

    public async Task<CreateSellerViewModel> GetUserInfoForCreateSeller(string phoneNumber)
    {
        var result = await _mapper.ProjectTo<CreateSellerViewModel>(_users)
            .SingleOrDefaultAsync(x => x.PhoneNumber == phoneNumber);

        //    باید تاریح رو کانورت بکنیم
        if (result?.BirthDate != null)
        {
            var parsedDateTime = DateTime.Parse(result.BirthDate);
            result.BirthDateEn = parsedDateTime.ToString("yyyy/MM/dd");
            result.BirthDate = parsedDateTime.ToShortPersianDateString().ToPersianNumbers();
        }

        return result;
    }

    public async Task<User> GetUserForCreateSeller(string userName)
    {
        return await _users.Where(x => x.IsSeller)
            .Where(x => x.UserRoles.All(r => r.Role.Name != ConstantRoles.Seller))
            .SingleOrDefaultAsync(x => x.UserName == userName);
    }


    public async Task<int> GetAllUsersCount()
    {
        var getAllUsersCount = await _users.AsNoTracking().CountAsync();
        return getAllUsersCount;
    }

    public long GetLoggedInUser()
    {
        return _httpContextAccessor.HttpContext.User.Identity.GetLoggedInUserId();
    }

    #endregion


}
