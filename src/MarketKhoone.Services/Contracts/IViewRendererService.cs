namespace MarketKhoone.Services.Contracts
{
    public interface IViewRendererService
    {
        /// <summary>
        /// Renders a .cshtml as an string
        /// </summary>
        /// <param name="viewNameOrPath"></param>
        /// <returns></returns>
        Task<string> RenderViewToStringAsync(string viewNameOrPath);

        /// <summary>
        /// Renders a .cshtml file as an string.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="viewNameOrPath"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> RenderViewToStringAsync<TModel>(string viewNameOrPath, TModel model);
    }
}
