using Microsoft.AspNetCore.Mvc;

namespace MarketKhoone.ViewModels.Categories;

public class AddVariantToCategoryViewModel
{
    [HiddenInput]
    //برای کدوم دسته بندی میخواهیم برندی اضافه کنیم
    public long SelectedCategoryId { get; set; }

    //ممکنه یه برند به یه دسته بندی اضافه نکنیم، چند برند رو اضافه کنیم پس لیستی از برندها داریم
    public List<string> SelectedVariants { get; set; }
        = new();
}
