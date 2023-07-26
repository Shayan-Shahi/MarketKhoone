
using Ganss.Xss;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities.Identity;
using MarketKhoone.Services.Contracts;
using MarketKhoone.Services.Contracts.Identity;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.Services.Services;
using MarketKhoone.Services.Services.FacadePattern;
using MarketKhoone.Services.Services.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Security.Principal;

namespace MarketKhoone.IocConfig
{
    public static class AddCustomServicesExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IPrincipal>(provider =>
                provider.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.User ?? ClaimsPrincipal.Current);

            services.AddScoped<IUserClaimsPrincipalFactory<User>, ApplicationClaimsPrincipalFactory>();
            services.AddScoped<UserClaimsPrincipalFactory<User, Role>, ApplicationClaimsPrincipalFactory>();

            services.AddScoped<IdentityErrorDescriber, CustomIdentityErrorDescriber>();

            services.AddScoped<IApplicationUserStore, ApplicationUserStore>();
            services
                .AddScoped<UserStore<User, Role, ApplicationDbContext, long, UserClaim, UserRole, UserLogin, UserToken,
                    RoleClaim>, ApplicationUserStore>();

            services.AddScoped<IApplicationRoleStore, ApplicationRoleStore>();
            services
                .AddScoped<RoleStore<Role, ApplicationDbContext, long, UserRole, RoleClaim>, ApplicationRoleStore>();

            services.AddScoped<IApplicationUserManager, ApplicationUserManager>();
            services.AddScoped<UserManager<User>, ApplicationUserManager>();

            services.AddScoped<IApplicationRoleManager, ApplicationRoleManager>();
            services.AddScoped<RoleManager<Role>, ApplicationRoleManager>();

            services.AddScoped<IApplicationSignInManager, ApplicationSignInManager>();
            services.AddScoped<SignInManager<User>, ApplicationSignInManager>();


            services.AddScoped<IIdentityDbInitializer, IdentityDbInitializer>();

            #region CustomServices
            services.AddScoped<IFacadeServices, FacadeServices>();
            services.AddScoped<ISendSmsService, SendSmsService>();
            services.AddScoped<IViewRendererService, ViewRendererService>();
            #endregion

            #region Html sanitizer
            IHtmlSanitizer sanitizer = new HtmlSanitizer();
            services.AddSingleton<IHtmlSanitizer, HtmlSanitizer>();
            services.AddSingleton(sanitizer);
            #endregion

            return services;
        }
    }

}
