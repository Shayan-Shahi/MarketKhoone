using MarketKhoone.ViewModels.Carts;

namespace MarketKhoone.ViewModels;

public class MainHeaderViewModel
{
    public int AllProductsCountInCart { get; set; }

    public List<ShowCartInDropDownViewModel> Carts { get; set; }
}
