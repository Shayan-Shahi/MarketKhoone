using AutoMapper;
using DNTCommon.Web.Core;
using MarketKhoone.Common.Helpers;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.IocConfig;
using MarketKhoone.ViewModels.Identity.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders;
using Parbad.Builder;
using Parbad.Gateway.Mellat;
using Parbad.Gateway.ParbadVirtual;
using Parbad.Gateway.ZarinPal;
using Parbad.Storage.EntityFrameworkCore.Builder;
using SixLabors.ImageSharp.Web.DependencyInjection;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);
var parbadConnectionString = builder.Configuration.GetConnectionString("ParbadDataContextConnection");

// Add services to the container.
builder.Services.Configure<SiteSettings>(options => builder.Configuration.Bind(options));
builder.Services.Configure<ContentSecurityPolicyConfig>(options => builder.Configuration.GetSection("ContentSecurityPolicyConfig").Bind(options));
// Adds all of the ASP.NET Core Identity related services and configurations at once.
builder.Services.AddCustomIdentityServices();
builder.Services.AddParbad()
    .ConfigureStorage(parbadBuilder =>
    {
        parbadBuilder.UseEfCore(options =>
        {
            // Example 1: Using SQL Server
            var assemblyName = typeof(ApplicationDbContext).Assembly.GetName().Name;

            // Example 2: If you prefer to have a separate MigrationHistory table for Parbad, you can change the above line to this:
            options.ConfigureDbContext = db => db.UseSqlServer(parbadConnectionString, sql =>
            {
                sql.MigrationsAssembly(assemblyName);
            });

            options.DefaultSchema = "dbo"; // optional

            options.PaymentTableOptions.Name = "Payment"; // optional

            options.TransactionTableOptions.Name = "Transaction"; // optional
        });
    })
    .ConfigureGateways(gateways =>
    {
        gateways
        .AddZarinPal()
            .WithAccounts(source => source.Add<ParbadGatewaysAccounts>(ServiceLifetime.Transient));

        gateways
            .AddMellat()
            .WithAccounts(source => source.Add<ParbadGatewaysAccounts>(ServiceLifetime.Transient));

        gateways
            .AddParbadVirtual()
            .WithOptions(options => options.GatewayPath = "/Cart/Payment/VirtualGateway");
    })
    .ConfigureHttpContext(parbadBuilder => parbadBuilder.UseDefaultAspNetCore());

builder.CreateCustomRoutes();

builder.Services.Configure<WebEncoderOptions>(options =>
{
    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
});

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddMaps(Assembly.GetExecutingAssembly());
});
var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddImageSharp();
var app = builder.Build();
app.Services.InitializeDb();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseImageSharp();
app.UseContentSecurityPolicy();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.UseParbadVirtualGateway();

app.Run();
