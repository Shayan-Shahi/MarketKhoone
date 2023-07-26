using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities.Identity;
using MarketKhoone.Services.Contracts.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MarketKhoone.Services.Services.Identity;

public class ApplicationUserStore
    : UserStore<User, Role, ApplicationDbContext, long, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>,
        IApplicationUserStore
{
    public ApplicationUserStore(
        IUnitOfWork uow,
        IdentityErrorDescriber describer = null)
        : base((ApplicationDbContext)uow, describer)
    {
        AutoSaveChanges = false;
    }

    #region CustomClass

    #endregion
}
