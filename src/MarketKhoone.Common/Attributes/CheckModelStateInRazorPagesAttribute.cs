using EShopMarket.Common.Helpers;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.IdentityToolkit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MarketKhoone.Common.Attributes;

public class CheckModelStateInRazorPagesAttribute : Attribute, IAsyncPageFilter
{
    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        return Task.CompletedTask;
    }

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        var modelState = context.ModelState;
        if (!modelState.IsValid)
        {
            context.Result = new JsonResult(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
            {
                Data = modelState.GetModelStateErrors()
            });
        }
        else
            await next();
    }
}
