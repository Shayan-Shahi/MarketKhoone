using AutoMapper;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Services.Contracts;
using MarketKhoone.Services.Contracts.Identity;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.Services.Services.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MarketKhoone.Services.Services.FacadePattern
{
    public class FacadeServices : IFacadeServices
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IApplicationUserManager _userManager;
        public FacadeServices(IUnitOfWork uow, IMapper mapper,
            IWebHostEnvironment environment, IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor, IApplicationUserManager userManager)
        {
            _uow = uow;
            _mapper = mapper;
            _environment = environment;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }


        private ISendSmsService _sendSmsService;
        public ISendSmsService SendSmsService
        {
            get
            {
                return _sendSmsService = _sendSmsService ?? new SendSmsService();
            }
        }
        private IEmailService _emailService;
        public IEmailService EmailService
        {
            get
            {
                return _emailService = _emailService ?? new EmailService(_configuration);
            }
        }

        private ICategoryService _categoryService;
        public ICategoryService CategoryService
        {
            get
            {
                return _categoryService = _categoryService ?? new CategoryService(_uow, _mapper, _httpContextAccessor);
            }
        }
        private ICategoryBrandService _categoryBrandService;
        public ICategoryBrandService CategoryBrandService
        {
            get
            {
                return _categoryBrandService = _categoryBrandService ?? new CategoryBrandService(_uow, _mapper);
            }
        }

        private IBrandService _brandService;
        public IBrandService BrandService
        {
            get
            {
                return _brandService = _brandService ?? new BrandService(_uow, _mapper);
            }
        }

        private IUploadedFileService _uploadedFileService;
        public IUploadedFileService UploadedFileService
        {
            get
            {
                return _uploadedFileService = _uploadedFileService ?? new UploadedFileService(_environment);
            }
        }
        private ICategoryFeatureService _categoryFeatureService;
        public ICategoryFeatureService CategoryFeatureService
        {
            get
            {
                return _categoryFeatureService = _categoryFeatureService ?? new CategoryFeatureService(_uow, _mapper);
            }
        }
        private ICategoryVariantService _categoryVariantService;
        public ICategoryVariantService CategoryVariantService
        {
            get
            {
                return _categoryVariantService = _categoryVariantService ?? new CategoryVariantService(_uow, _mapper);
            }
        }
        private IFeatureService _featureService;
        public IFeatureService FeatureService
        {
            get
            {
                return _featureService = _featureService ?? new FeatureService(_uow, _mapper);
            }
        }
        private IFeatureConstantValueService _featureConstantValueService;
        public IFeatureConstantValueService FeatureConstantValueService
        {
            get
            {
                return _featureConstantValueService = _featureConstantValueService ?? new FeatureConstantValueService(_uow, _mapper);
            }
        }

        private IProvinceAndCityService _provinceAndCityService;
        public IProvinceAndCityService ProvinceAndCityService
        {
            get
            {
                return _provinceAndCityService = _provinceAndCityService ?? new ProvinceAndCityService(_uow, _mapper);
            }
        }

        private ISellerService _sellerService;
        public ISellerService SellerService
        {
            get
            {
                return _sellerService = _sellerService ?? new SellerService(_uow, _mapper, _httpContextAccessor);
            }
        }
        private IGuaranteeService _guaranteeService;
        public IGuaranteeService GuaranteeService
        {
            get
            {
                return _guaranteeService = _guaranteeService ?? new GuaranteeService(_uow, _mapper);
            }
        }

        private IProductService _productService;
        public IProductService ProductService
        {
            get
            {
                return _productService = _productService ?? new ProductService(_uow, _mapper, _httpContextAccessor);
            }
        }

        private IProductVariantService _productVariantService;
        public IProductVariantService ProductVariantService
        {
            get
            {
                return _productVariantService = _productVariantService ?? new ProductVariantService(_uow, _mapper, _httpContextAccessor);
            }
        }
        private IProductStockService _productStockService;
        public IProductStockService ProductStockService
        {
            get
            {
                return _productStockService = _productStockService ?? new ProductStockService(_uow, _mapper);
            }
        }

        private IProductCommentService _productCommentService;
        public IProductCommentService ProductCommentService
        {
            get
            {
                return _productCommentService = _productCommentService ?? new ProductCommentService(_uow, _mapper, _httpContextAccessor);
            }
        }
        private ICommentScoreService _commentScoreService;
        public ICommentScoreService CommentScoreService
        {
            get
            {
                return _commentScoreService = CommentScoreService ?? new CommentScoreService(_uow, _mapper);
            }
        }

        private ICommentReportService _commentReportService;
        public ICommentReportService CommentReportService
        {
            get
            {
                return _commentReportService = _commentReportService ?? new CommentReportService(_uow, _mapper);
            }
        }
        private IAnswerScoreService _answerScoreService;
        public IAnswerScoreService AnswerScoreService
        {
            get
            {
                return _answerScoreService = _answerScoreService ?? new AnswerScoreService(_uow, _mapper);
            }
        }
        private IProductQuestionAnswerScoreService _productQuestionAnswerScoreService;
        public IProductQuestionAnswerScoreService ProductQuestionAnswerScoreService
        {
            get
            {
                return _productQuestionAnswerScoreService = _productQuestionAnswerScoreService ?? new ProductQuestionAnswerScoreService(_uow, _mapper);
            }
        }

        private IQuestionAndAnswerService _questionAndAnswerService;
        public IQuestionAndAnswerService QuestionAndAnswerService
        {
            get
            {
                return _questionAndAnswerService = _questionAndAnswerService ?? new QuestionAndAnswerService(_uow, _mapper);
            }
        }

        private IOrderService _orderService;
        public IOrderService OrderService
        {
            get
            {
                return _orderService = _orderService ?? new OrderService(_uow, _mapper, _httpContextAccessor);
            }
        }

        private IProductShortLinkService _productShortLinkService;
        public IProductShortLinkService ProductShortLinkService
        {
            get
            {
                return _productShortLinkService = _productShortLinkService ?? new ProductShortLinkService(_uow, _mapper);
            }
        }
        private IVariantService _variantService;
        public IVariantService VariantService
        {
            get
            {
                return _variantService = _variantService ?? new VariantService(_uow, _mapper);
            }
        }

        private IAddressService _addressService;
        public IAddressService AddressService
        {
            get
            {
                return _addressService = _addressService ?? new AddressService(_uow, _mapper, _httpContextAccessor);
            }
        }


        private IUserHistoryService _userHistoryService;
        public IUserHistoryService UserHistoryService
        {
            get
            {
                return _userHistoryService = _userHistoryService ?? new UserHistoryService(_uow, _mapper, _httpContextAccessor);
            }
        }
        private IWalletService _walletService;
        public IWalletService WalletService
        {
            get
            {
                return _walletService = _walletService ?? new WalletService(_uow);
            }
        }
        private IParcelPostItemService _parcelPostItemService;
        public IParcelPostItemService ParcelPostItemService
        {
            get
            {
                return _parcelPostItemService = _parcelPostItemService ?? new ParcelPostItemService(_uow, _mapper, _httpContextAccessor);
            }
        }
        private IParcelPostService _parcelPostService;
        public IParcelPostService ParcelPostService
        {
            get
            {
                return _parcelPostService = _parcelPostService ?? new ParcelPostService(_uow);
            }
        }

        private ICartService _cartService;
        public ICartService CartService
        {
            get
            {
                return _cartService = _cartService ?? new CartService(_uow, _mapper);
            }
        }

        private IUserProductFavoriteService _userProductFavoriteService;
        public IUserProductFavoriteService UserProductFavoriteService
        {
            get
            {
                return _userProductFavoriteService = _userProductFavoriteService ?? new UserProductFavoriteService(_uow, _mapper);
            }
        }

        private IDiscountNoticeService _discountNoticeService;
        public IDiscountNoticeService DiscountNoticeService
        {
            get
            {
                return _discountNoticeService = _discountNoticeService ?? new DiscountNoticeService(_uow, _mapper);
            }
        }

        private IDiscountCodeService _discountCodeService;
        public IDiscountCodeService DiscountCodeService
        {
            get
            {
                return _discountCodeService = _discountCodeService ?? new DiscountCodeService(_uow, _userManager);
            }
        }

        private IUsedDiscountCodeService _usedDiscountCodeService;
        public IUsedDiscountCodeService UsedDiscountCodeService
        {
            get
            {
                return _usedDiscountCodeService = _usedDiscountCodeService ?? new UsedDiscountCodeService(_uow, _mapper);
            }
        }

        private IUserListService _userListService;
        public IUserListService UserListService
        {
            get
            {
                return _userListService = _userListService ?? new UserListService(_uow, _mapper);
            }
        }
        private IUserListProductService _userListProductService;
        public IUserListProductService UserListProductService
        {
            get
            {
                return _userListProductService = _userListProductService ?? new UserListProductService(_uow, _mapper);
            }
        }
        private IUserListShortLinkService _userListShortLinkService;
        public IUserListShortLinkService UserListShortLinkService
        {
            get
            {
                return _userListShortLinkService = _userListShortLinkService ?? new UserListShortLinkService(_uow, _mapper);
            }
        }
        private IGiftCardService _giftCardService;
        public IGiftCardService GiftCardService
        {
            get
            {
                return _giftCardService = _giftCardService ?? new GiftCardService(_uow, _mapper);
            }
        }
        private IConsignmentService _consignmentService;
        public IConsignmentService ConsignmentService
        {
            get
            {
                return _consignmentService = _consignmentService ?? new ConsignmentService(_uow, _mapper);
            }
        }
        private IConsignmentItemService _consignmentItemService;
        public IConsignmentItemService ConsignmentItemService
        {
            get
            {
                return _consignmentItemService = _consignmentItemService ?? new ConsignmentItemService(_uow, _mapper);
            }
        }

    }
}
