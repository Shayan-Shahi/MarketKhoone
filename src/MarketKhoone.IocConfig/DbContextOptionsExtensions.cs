using DNTCommon.Web.Core;
using MarketKhoone.Common.GuardToolKit;
using MarketKhoone.Common.PersianToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Services.Contracts.Identity;
using MarketKhoone.ViewModels.Identity.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MarketKhoone.IocConfig
{
    public static class DbContextOptionsExtensions
    {
        public static IServiceCollection AddConfiguredDbContext(
            this IServiceCollection services, SiteSettings siteSettings)
        {
            siteSettings.CheckArgumentIsNull(nameof(siteSettings));
            var connectionString = siteSettings.ConnectionStrings.ApplicationDbContextConnection;
            services.AddScoped<IUnitOfWork>(serviceProvider =>
                serviceProvider.GetRequiredService<ApplicationDbContext>());
            // We use `AddDbContextPool` instead of AddDbContext because it's faster
            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
                options.AddInterceptors(new PersianYeKeCommandInterceptor());
            });
            return services;
        }

        /// <summary>
        /// Creates and seeds the database.
        /// </summary>
        public static void InitializeDb(this IServiceProvider serviceProvider)
        {
            //using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            //{
            //    var context = serviceScope.ServiceProvider.GetRequiredService<IIdentityDbInitializer>();
            //    context.Initialize();
            //    context.SeedData();
            //}
            serviceProvider.RunScopedService<IIdentityDbInitializer>(identityDbInitialize =>
            {
                identityDbInitialize.Initialize();
                identityDbInitialize.SeedData();
            });
        }
    }
}
