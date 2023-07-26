using MarketKhoone.Services.Services.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Marketkhoone.Web.Pages.Inventory
{
    [Authorize(Roles = ConstantRoles.DeliveryMan)]
    public class DeliveryOrderBasePanel : PageBase
    {
    }
}
