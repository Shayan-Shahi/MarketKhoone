using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities.Identity;
using MarketKhoone.Services.Contracts.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MarketKhoone.Services.Services.Identity;

public class ApplicationRoleStore
    : RoleStore<Role, ApplicationDbContext, long, UserRole, RoleClaim>,
        IApplicationRoleStore
{
    public ApplicationRoleStore(
        IUnitOfWork uow,
        IdentityErrorDescriber describer = null)
        : base((ApplicationDbContext)uow, describer)
    {
        //تا زمانیکه 
        //SaveChange();
        //فراخوانی نشه، چیزی به پایگاه داده اعمال نمیشه

        //مگه اینطوری بود اصلا؟
        // بله، اگر و فقط اگر از متد های خود آیدینتی استفاده کنیم،
        // خود دات نت میاد و بدون اینکه سیو چنجی رخ بده، مقادیر رو به پایگاه داده اعمال میکنه
        //وقتی مقدار  فالس رو ست میکنیم، منتظر میمونه به سیو چنج برسه بعد تغییرات رو
        //به پایگاه داده اعمال کنه

        AutoSaveChanges = false;
    }

    #region CustomClass

    #endregion
}
