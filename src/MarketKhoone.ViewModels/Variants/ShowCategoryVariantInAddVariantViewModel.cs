namespace MarketKhoone.ViewModels.Variants;

public class ShowCategoryVariantInAddVariantViewModel
{
    //ما از طریق جدوا واسط
    //CategoryVariant
    //اقدام میکنیم پس اول میگیم وارد 
    //Variant
    //بشو و مقادیر رو بگیر
    public long VariantId { get; set; }

    public string VariantValue { get; set; }

    public string VariantColorCode { get; set; }

    public bool? VariantIsColor { get; set; }
}