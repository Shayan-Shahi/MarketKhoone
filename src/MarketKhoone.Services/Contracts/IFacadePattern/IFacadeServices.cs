using MarketKhoone.Services.Contracts.Identity;

namespace MarketKhoone.Services.Contracts.IFacadePattern
{
    public interface IFacadeServices
    {
        ISendSmsService SendSmsService { get; }
        IEmailService EmailService { get; }
        ICategoryService CategoryService { get; }
        IBrandService BrandService { get; }
        IUploadedFileService UploadedFileService { get; }
        ICategoryFeatureService CategoryFeatureService { get; }
        IFeatureService FeatureService { get; }
        IFeatureConstantValueService FeatureConstantValueService { get; }
        IProvinceAndCityService ProvinceAndCityService { get; }
        ISellerService SellerService { get; }
        IProductService ProductService { get; }
        IProductVariantService ProductVariantService { get; }
        IProductCommentService ProductCommentService { get; }
        IOrderService OrderService { get; }
        IGuaranteeService GuaranteeService { get; }
        IProductShortLinkService ProductShortLinkService { get; }
        IVariantService VariantService { get; }
        IAddressService AddressService { get; }
        IUserHistoryService UserHistoryService { get; }
        IWalletService WalletService { get; }
        IParcelPostItemService ParcelPostItemService { get; }
        ICommentScoreService CommentScoreService { get; }
        IProductQuestionAnswerScoreService ProductQuestionAnswerScoreService { get; }
        ICartService CartService { get; }
        IUserProductFavoriteService UserProductFavoriteService { get; }
        ICommentReportService CommentReportService { get; }
        IQuestionAndAnswerService QuestionAndAnswerService { get; }
        IAnswerScoreService AnswerScoreService { get; }
        IDiscountNoticeService DiscountNoticeService { get; }
        IUserListService UserListService { get; }
        IUserListProductService UserListProductService { get; }
        IUserListShortLinkService UserListShortLinkService { get; }
        IConsignmentService ConsignmentService { get; }
        ICategoryBrandService CategoryBrandService { get; }
        ICategoryVariantService CategoryVariantService { get; }
        IProductStockService ProductStockService { get; }
        IParcelPostService ParcelPostService { get; }
        IConsignmentItemService ConsignmentItemService { get; }
        IDiscountCodeService DiscountCodeService { get; }
        IUsedDiscountCodeService UsedDiscountCodeService { get; }
        IGiftCardService GiftCardService { get; }



    }
}
