using AutoMapper;
using MarketKhoone.Common.Helpers;
using MarketKhoone.Entities;
using MarketKhoone.Entities.Identity;
using MarketKhoone.ViewModels.Brands;
using MarketKhoone.ViewModels.Categories;
using MarketKhoone.ViewModels.CategoryFeatures;
using MarketKhoone.ViewModels.Consignments;
using MarketKhoone.ViewModels.FeatureConstantValues;
using MarketKhoone.ViewModels.Guarantees;
using MarketKhoone.ViewModels.Products;
using MarketKhoone.ViewModels.ProductShortLinks;
using MarketKhoone.ViewModels.ProductVariants;
using MarketKhoone.ViewModels.Sellers;
using MarketKhoone.ViewModels.Variants;

namespace Marketkhoone.Web.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            #region Parameters

            long userId = 0;
            long sellerId = 0;
            long consignmentId = 0;
            DateTime now = default;

            #endregion

            this.CreateMap<User, CreateSellerViewModel>();
            this.CreateMap<CreateSellerViewModel, Seller>()
                .AddTransform<string>(str => str != null ? str.Trim() : null);

            this.CreateMap<CreateSellerViewModel, User>()
                .AddTransform<string>(str=>str !=null ? str.Trim() : null)
                .ForMember(x=>x.BirthDate,
                    options=>
                       options.Ignore());

            this.CreateMap<Seller, ShowSellerViewModel>()
                .ForMember(dest => dest.ProvinceAndCity,
                    options =>
                        options.MapFrom(src => $"{src.Province.Title} - {src.City.Title}"))

                .ForMember(dest => dest.CreatedDateTime,
                    options =>
                        options.MapFrom(src => src.CreatedDateTime.ToLongPersianDate()));

            this.CreateMap<Brand, ShowBrandViewModel>();
            this.CreateMap<AddBrandViewModel, Brand>()
                .AddTransform<string>(str => str != null ? str.Trim() : null);

            this.CreateMap<Brand, EditBrandViewModel>().ReverseMap()
                .AddTransform<string>(str => str != null ? str.Trim() : null);

            this.CreateMap<Category, EditCategoryViewModel>()
                .ForMember(x => x.Picture,
                    options =>
                        options.Ignore())


                .ForMember(dest => dest.SelectedPicture,
                    options =>
                        options.MapFrom(src => src.Picture))

                .ForMember(dest => dest.CanVariantTypeChange,
                    options =>
                        options.MapFrom(src => !src.CategoryVariants.Any() && (!src.HasVariant)));
               

            this.CreateMap<EditCategoryViewModel, Category>()
                .AddTransform<string>(str => str != null ? str.Trim() : null);

            this.CreateMap<EditCategoryViewModel, Category>()
                .AddTransform<string>(str => str != null ? str.Trim() : null);

            this.CreateMap<Brand, BrandDetailsViewModel>();
            this.CreateMap<CategoryFeature, CategoryFeatureForCreateProductViewModel>();

            this.CreateMap<FeatureConstantValue, ShowFeatureConstantValueViewModel>();

            this.CreateMap<AddFeatureConstantValueViewModel, FeatureConstantValue>()
                .AddTransform<string>(str => str != null ? str.Trim() : null);

            this.CreateMap<FeatureConstantValue, ShowCategoryFeatureConstantValueViewModel>();
            this.CreateMap<FeatureConstantValue, EditFeatureConstantValueViewModel>();

            this.CreateMap<AddProductViewModel, Product>()
                .AddTransform<string>(str => str != null ? str.Trim() : null);

            this.CreateMap<FeatureConstantValue, FeatureConstantValueForCreateProductViewModel>();

            this.CreateMap<Product, ShowProductViewModel>()
                .ForMember(dest => dest.MainPicture,
                    options =>
                        options.MapFrom(src => src.ProductMedia.First().FileName));

            this.CreateMap<Product, ShowProductInSellerPanelViewModel>()
                .ForMember(dest => dest.MainPicture,
                    options =>
                        options.MapFrom(src => src.ProductMedia.First().FileName));

            this.CreateMap<Product, ShowAllProductInSellerPanelViewModel>()
                .ForMember(dest => dest.MainPicture,
                    options =>
                        options.MapFrom(src => src.ProductMedia.First().FileName));


            this.CreateMap<Product, ProductDetailsViewModel>();
            this.CreateMap<ProductFeature, ProductFeatureForProductDetailsViewModel>();

            this.CreateMap<Variant, ShowVariantViewModel>();

            this.CreateMap<Guarantee, ShowGuaranteeViewModel>();

            this.CreateMap<Product, AddVariantViewModel>()
                .ForMember(dest => dest.ProductId,
                    options =>
                        options.MapFrom(src => src.Id))

                .ForMember(dest => dest.ProductTitle,
                    options =>
                        options.MapFrom(src => src.PersianTitle))

                .ForMember(dest => dest.CommissionPercentage,
                    options =>
                        options.MapFrom(src => src.Category.CategoryBrands
                            .Select(x => new
                            {
                                x.BrandId,
                                x.CommissionPercentage
                            })
                            .Single(x => x.BrandId == src.BrandId)
                            .CommissionPercentage

                        ))


                .ForMember(dest => dest.MainPicture,
                    options =>
                        options.MapFrom(src => src.ProductMedia.First().FileName))


                .ForMember(dest => dest.MainPicture,
                    options =>
                        options.MapFrom(src => src.ProductMedia.First().FileName))

                .ForMember(dest => dest.Variants,
                    options =>
                        options.MapFrom(src => src.Category.CategoryVariants.Where(x => x.Variant.IsConfirmed)))


                .ForMember(dest => dest.AddedVariantsIds,
                    options =>
                        options.MapFrom(src =>
                            src.ProductVariants.Where(x => x.SellerId == sellerId).Select(x => x.VariantId)));


            this.CreateMap<CategoryVariant, ShowCategoryVariantInAddVariantViewModel>();

            this.CreateMap<AddVariantViewModel, ProductVariant>();

            this.CreateMap<ProductVariant, ShowProductVariantViewModel>()
                .ForMember(dest => dest.StartDateTime,
                    options =>
                        options.MapFrom(src => src.StartDateTime != null
                            ? src.StartDateTime.Value.ToLongPersianDate()
                            : null))

                .ForMember(dest => dest.EndDateTime,
                    options =>
                        options.MapFrom(src => 
                             src.EndDateTime != null
                                ? src.EndDateTime.Value.ToLongPersianDate()
                                : null));


            this.CreateMap<ProductVariant, ShowProductVariantInCreateConsignmentViewModel>();

            this.CreateMap<ProductVariant, GetProductVariantInCreateConsignmentViewModel>();

            this.CreateMap<Consignment, ShowConsignmentViewModel>()
                .ForMember(dest => dest.DeliveryDate,
                    options =>
                        options.MapFrom(src => src.DeliveryDate.ToLongPersianDate()));


            this.CreateMap<Consignment, ShowConsignmentDetailsViewModel>()
                .ForMember(dest => dest.DeliveryDate,
                    options =>
                        options.MapFrom(src => src.DeliveryDate.ToLongPersianDate()))

                .ForMember(dest => dest.DeliveryDate,
                    options =>
                        options.MapFrom(src => src.DeliveryDate.ToLongPersianDate()))


                .ForMember(dest => dest.Items,
                    options =>
                        options.MapFrom(src => src.ConsignmentItems.Where(x => x.ConsignmentId == consignmentId)));


            this.CreateMap<ConsignmentItem, ShowConsignmentItemViewModel>();

            this.CreateMap<Product, ShowProductInfoViewModel>()
                .ForMember(dest => dest.Score,
                    options =>
                        options.MapFrom(src =>
                            src.ProductComments.Any()
                                ? src.ProductComments.Average(pc => pc.Score)
                                : 0

                        ))


                .ForMember(dest => dest.ProductCommentsCount,
                    options =>
                        options.MapFrom(src =>
                            src.ProductsQuestionsAndAnswers.Where(x => x.IsConfirmed)
                                .LongCount(pc => pc.ParentId == null)))

                .ForMember(dest => dest.SuggestCount,
                    options =>
                        options.MapFrom(src => src.ProductComments
                            .Where(x => x.IsBuyer)
                            .LongCount(pc => pc.Suggest == true)))


                .ForMember(dest => dest.BuyerCount,
                    options =>
                        options.MapFrom(src => src.ProductComments
                            .LongCount(x => x.IsBuyer)))

                .ForMember(dest => dest.ProductVariants,
                    options =>
                        options.MapFrom(src =>
                            src.ProductVariants.Where(x => x.Count > 0)))

                .ForMember(dest => dest.IsFavorite,
                    options =>
                        options.MapFrom(src =>
                            userId != 0 && src.UserProductsFavorites.Any(x => x.UserId == userId)))

                .ForMember(dest => dest.IsVariantTypeNull,
                    options =>
                        options.MapFrom(src => src.Category.IsVariantColor == null))


                .ForMember(dest => dest.ProductComments,
                    options =>
                        options.MapFrom(src =>
                            src.ProductComments.Where(x => x.IsConfirmed.Value)
                                .Where(x => x.CommentTitle != null)
                                .OrderByDescending(x => x.Id)
                                .Take(2)))


                .ForMember(dest => dest.ProductsQuestionsAndAnswers,
                    options =>
                        options.MapFrom(src => src.ProductsQuestionsAndAnswers.Where(x => x.IsConfirmed)
                            .Where(x => x.ParentId == null)
                            .OrderByDescending(x => x.Id)
                            .Take(2)));


            this.CreateMap<ProductCategory, ProductCategoryForProductInfoViewModel>();

            this.CreateMap<ProductFeature, ProductFeatureForProductInfoViewModel>();

            this.CreateMap<ProductVariant, ProductVariantForProductInfoViewModel>()
                .ForMember(dest => dest.EndDateTime,
                    options =>
                        options.MapFrom(src =>
                            src.EndDateTime != null ? src.EndDateTime.Value.ToString("yyyy/MM/dd HH:mm:ss") : null))

                .ForMember(dest => dest.IsDiscountActive,
                    options =>
                        options.MapFrom(src =>
                            src.OffPercentage != null && (src.StartDateTime <= now && src.EndDateTime >= now)))


                .ForMember(dest => dest.Count,
                    options =>
                        options.MapFrom(src =>
                            src.Count > 3 ? (byte)0 : (byte)src.Count));


            this.CreateMap<ProductShortLink, ShowProductShortLinkViewModel>();

            this.CreateMap<ProductVariant, EditProductVariantViewModel>()
                .ForMember(dest => dest.MainPicture,
                    options =>
                        options.MapFrom(src => src.Product.ProductMedia.First().FileName))

                .ForMember(dest => dest.ProductTitle,
                    options =>
                        options.MapFrom(src => src.Product.PersianTitle))


                .ForMember(dest => dest.IsDiscountActive,
                    options =>
                        options.MapFrom(src =>
                            src.OffPercentage != null && (src.StartDateTime <= now && src.EndDateTime >= now)))

                .ForMember(dest => dest.CommissionPercentage,
                    options =>
                        options.MapFrom(src => src.Product.Category.CategoryBrands
                            .Select(x => new
                            {
                                x.BrandId,
                                x.CommissionPercentage
                            })
                            .Single(x => x.BrandId == src.Product.BrandId)
                            .CommissionPercentage
                        ));

            this.CreateMap<ProductVariant, AddEditDiscountViewModel>()
                .ForMember(dest => dest.MainPicture,
                    options =>
                        options.MapFrom(src => src.Product.ProductMedia.First().FileName))

                .ForMember(dest => dest.ProductTitle,
                    options =>
                        options.MapFrom(src => src.Product.PersianTitle))

                .ForMember(dest => dest.CommissionPercentage,
                    options =>
                        options.MapFrom(src => src.Product.Category.CategoryBrands
                            .Select(x => new
                            {
                                x.BrandId,
                                x.CommissionPercentage
                            })
                            .Single(x => x.BrandId == src.Product.BrandId)
                            .CommissionPercentage
                        ));

            this.CreateMap<AddEditDiscountViewModel, ProductVariant>()
                .ForMember(x => x.Price,
                    options => options.Ignore())

                .ForMember(x => x.StartDateTime,
                    options =>
                        options.Ignore())

                .ForMember(x => x.EndDateTime,
                    options=>options.Ignore());

            this.CreateMap<Variant, ShowVariantInEditCategoryVariantViewModel>();

        }
    }
}
