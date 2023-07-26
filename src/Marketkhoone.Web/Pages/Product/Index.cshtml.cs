using AutoMapper;
using EShopMarket.Common.Helpers;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.Helpers;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels;
using MarketKhoone.ViewModels.Categories;
using MarketKhoone.ViewModels.DiscountNotices;
using MarketKhoone.ViewModels.ProductComments;
using MarketKhoone.ViewModels.Products;
using MarketKhoone.ViewModels.QuestionsAndAnswers;
using MarketKhoone.ViewModels.UserLists;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Marketkhoone.Web.Pages.Product
{
    public class IndexModel : PageBase
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;
        private readonly IUnitOfWork _uow;
        private readonly IViewRendererService _viewRendererService;
        private readonly IMapper _mapper;

        public IndexModel(IFacadeServices facadeServices, IUnitOfWork uow,
            IViewRendererService viewRendererService, IMapper mapper)
        {
            _facadeServices = facadeServices;
            _uow = uow;
            _viewRendererService = viewRendererService;
            _mapper = mapper;
        }

        #endregion

        public ShowProductInfoViewModel ProductInfo { get; set; }

        //پارامتر های ورودی این متد یعنی
        //productCode, slug
        // چی میکن؟
        // میگن در صفحه تکی محصول باید 
        //URL
        // بصورت 
        //https://...../productCode/slug/....
         // باشن
        public async Task<IActionResult> OnGet(int productCode, string slug)
        {
            ProductInfo = await _facadeServices.ProductService.GetProductInfo(productCode);

            if (ProductInfo is null)
            {
                return RedirectToPage(PublicConstantStrings.Error404PageName);
            }
            //اگر کاربر اسلاگ رو در یو آر ال یه چیزی عیر از اسلاگ
            //
            //موجود در دیتابیس وارد کرد، ریدایرکت کن له اسلاگ اصلی
            // ارزش سئویی دارد
            if (ProductInfo.Slug != slug)
            {
                return RedirectToPage("Index", new
                {
                    //چون این دو اسم با هم برابرند، میتونیم فقط یکیش رو وارد کنیم
                    // بخاطر همینه اولی خاکستریه
                    productCode = productCode,
                    slug = ProductInfo.Slug
                });
            }

            //var userId = User.Identity.GetLoggedInUserId();

            ////آیدی کامنت هایی که در صفحه نمایش داد می شوند
            //var commentIds = ProductInfo.ProductComments
            //    .Select(x => x.Id)
            //    .ToArray();

            ////آیدی سوالاتی که در صفحه نمایش داده می شوند
            //var questionIds = ProductInfo.ProductsQuestionsAndAnswers
            //    .SelectMany(x => x.Answers)
            //    .Select(x => x.Id)
            //    .ToArray();

            ////از داخل کامنت هایی که در صفخه نمایش داده می شونده کدامیک توسط این کاربر
            //// لایک و یا دیسلایک شده است
            //ProductInfo.LikedCommentsByUser =
            //    await _facadeServices.CommentScoreService.GetLikedCommentsLikedByUser(userId, commentIds);


            ////از داخل جواب هایی که در صفحه نمایش داده میشوند کدامیک توسط این کاربر
            ////لایک و یا دیسلایک روی آنها انجام شده است

            //ProductInfo.LikedAnswersByUser =
            //    await _facadeServices.ProductQuestionAnswerScoreService.GetLikedAnswersByUser(userId, questionIds);

            ////نظرات این محصول در چند صفحه نمایش  داده می شوند
            //ProductInfo.CommentsPagesCount = (int)Math.Ceiling((decimal)ProductInfo.ProductQuestionsCount / 2);

            ////سوالات این محصول در چمد صغحه نمایش داده می شوند
            //ProductInfo.QuestionsPagesCount = (int)Math.Ceiling((decimal)ProductInfo.ProductQuestionsCount / 2);


            ////آیدی های تنوع این محصول

            //var productVariantsIds = ProductInfo.ProductVariants.Select(x => x.Id).ToList();

            ////تنوع های این محصول که در سبد خرید این کاربری که، صفحه رو لود میکنه قرار داره
            //ProductInfo.ProductVariantsInCart =
            //    await _facadeServices.CartService.GetProductVariantsInCart(productVariantsIds, userId);

            return Page();
        }

        public async Task<IActionResult> OnPostAddOrRemoveFavorite(long productId, bool addFavorite)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new JsonResultOperation(false));
            }

            if (!await _facadeServices.ProductService.IsExistsBy(nameof(MarketKhoone.Entities.Product.Id), productId))
            {
                return Json(new JsonResultOperation(false));
            }

            var userId = User.Identity.GetLoggedInUserId();

            var userProductFavorite = await _facadeServices.UserProductFavoriteService.FindAsync(userId, productId);


            if (userProductFavorite is null && addFavorite)
            {
                await _facadeServices.UserProductFavoriteService.AddAsync(
                    new MarketKhoone.Entities.UserProductFavorite()
                    {
                        ProductId = productId,
                        UserId = userId,
                    });
            }
            else if (userProductFavorite != null && !addFavorite)
            {
                _facadeServices.UserProductFavoriteService.Remove(userProductFavorite);
            }

            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, string.Empty));
        }

        public async Task<IActionResult> OnPostAddProductVariantsToCart(long productVariantId, bool isIncrease)
        {
            var productVariant = await _facadeServices.ProductVariantService.FindByIdAsync(productVariantId);

            if (productVariant is null)
            {
                return Json(new JsonResultOperation(false));
            }

            var userId = User.Identity.GetLoggedInUserId();

            var cart = await _facadeServices.CartService.FindAsync(userId, productVariantId);
            if (cart is null)
            {
                var cartToAdd = new MarketKhoone.Entities.Cart()
                {
                    ProductVariantId = productVariantId,
                    UserId = userId,
                    Count = 1
                };
                await _facadeServices.CartService.AddAsync(cartToAdd);
            }
            else if (isIncrease)
            {
                //فروشنده تعیین کرده که حداکثر تعدادی که کاربر طی هر خرید میتنوه 
                //از این محصول وارد سبد خرید بکنه و خریدشو انجام بده 3 مورد است

                // مقدار داخل سبد خرید قبل فشردن دکمه به علاوه 
                //3
                cart.Count++;

                //بعد از ردن دکمه به علاوه
                //4

                //چون تعداد داهل سبد خرید بیشتر از مقداری هست که فروشنده تعیین کرده
                //در نتیجه مقدار داخل سبد خرید را به مقدار تعیین شده توسط فروشنده تعییر میدهیم
                if (cart.Count > productVariant.MaxCountInCart)
                    cart.Count = productVariant.MaxCountInCart;

                //موجودی انبار 2 است
                //موجودی داخل سبد خرید هم 2 است
                // حالا روی دکمه به علاوه کلیک میشه
                //چون حداکثر تعدادی که فروشنده تعیین کرده 3 است
                // در نتیجه از ایف بالا عبور میکنه و به ایف پاییین میرسه
                // موقعی که روی دکمه به علاوه یک کلیک میشه
                // تعداد داخل سبد خرید 3 است
                // و چون 3 بزرگتر ازبزرگتر از موجودی انبار یعنی 2 است
                // در نتیجه مقدار داخل سبد خرید هم به 2 تغییر میدهیم

                if (cart.Count > productVariant.Count)
                    cart.Count = (short)productVariant.Count;
            }
            else
            {
                cart.Count--;
                if (cart.Count == 0)
                {
                    _facadeServices.CartService.Remove(cart);
                }
            }

            await _uow.SaveChangesAsync();

            var isCartFull = productVariant.MaxCountInCart == (cart?.Count ?? 1) ||
                             (cart?.Count ?? 1) == productVariant.Count;

            var carts = await _facadeServices.CartService.GetCartsForDropDown(userId);

            return Json(new JsonResultOperation(true, string.Empty)
            {
                Data = new
                {
                    Count = cart?.Count ?? 1,
                    ProductVariantId = productVariantId,
                    IsCartFull = isCartFull,
                    CartsDetails =
                        await _viewRendererService.RenderViewToStringAsync("~/Pages/Shared/_CartPartial.cshtml", carts)
                }
            });
        }

        public async Task<IActionResult> OnPostAddCommentReport(long commentId)
        {
            var userId = User.Identity.GetUserId();

            if (userId is null)
            {
                return Json(new JsonResultOperation(false));
            }

            if (!await _facadeServices.ProductCommentService.IsExistsBy(nameof(MarketKhoone.Entities.ProductComment.Id),
                    commentId))
            {
                return JsonBadRequest();
            }

            if (await _facadeServices.CommentReportService.IsExistsBy(
                    nameof(MarketKhoone.Entities.CommentReport.UserId),
                    nameof(MarketKhoone.Entities.CommentReport.ProductCommentId), userId, commentId))
            {
                return Json(new JsonResultOperation(false, "شما از قبل این دیدگاه را گزارش کرده اید"));
            }

            //افزودن گزارش کامنت
            await _facadeServices.CommentReportService.AddAsync(new CommentReport()
            {
                UserId = userId.Value,
                ProductCommentId = commentId
            });

            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "گزارش این دیدگاه با موفقیت ثبت شد"));

        }

        public async Task<IActionResult> OnGetShowCommentsByPagination(long productId, int pageNumber,
            int commentsPagesCount
            , CommentsSortingForProductInfo sortBy, SortingOrder orderBy)
        {
            if (!await _facadeServices.ProductService.IsExistsBy(nameof(MarketKhoone.Entities.Product.Id), productId))
            {
                return Json(new JsonResultOperation(false));
            }

            var comments =
                await _facadeServices.ProductCommentService.GetCommentsByPagination(productId, pageNumber, sortBy,
                    orderBy);
            var model = new CommentForCommentPartialViewModel()
            {
                CurrentPage = pageNumber,
                CommentsPagesCount = commentsPagesCount,
                ProductComments = comments
            };

            var userId = User.Identity.GetUserId();

            if (userId != null)
            {
                var commentIds = comments.Select(x => x.Id).ToArray();

                model.LikedCommentsByUser = await _facadeServices.CommentScoreService
                    .GetLikedCommentsLikedByUser(userId.Value, commentIds);
            }

            return Partial("_CommentsPartial", model);
        }

        public async Task<IActionResult> OnPostCommentScore(long commentId, bool isLike)
        {
            var userId = User.Identity.GetUserId();

            if (userId is null)
            {
                return Json(new JsonResultOperation(false));
            }

            if (!await _facadeServices.ProductCommentService.IsExistsBy(nameof(MarketKhoone.Entities.ProductComment.Id),
                    commentId))
            {
                return Json(new JsonResultOperation(false));
            }

            var commentScore = await _facadeServices.CommentScoreService.FindAsync(userId.Value, commentId);

            var operation = string.Empty;

            //اگر وجود نداشته باشه اضافه میکنیم
            if (commentScore is null)
            {
                operation = "Add";
                await _facadeServices.CommentScoreService.AddAsync(new CommentScore()
                {
                    IsLike = isLike,
                    ProductCommentId = commentId,
                    UserId = userId.Value
                });
            }
            //اگر کاربر لایک کرده بود و بعد دوباره روی دکمهه یسلایک کلیک کرده بود باید لایک رو حذف کنیم
            //اگر کاربر دیسلایک کرده بود و بعد دوباره روی دکمه لایک کلیک کرده بود باید دیسلایک رو حذف کنیم
            else if (commentScore.IsLike && isLike || !commentScore.IsLike && !isLike)
            {
                operation = "Subtract";
                _facadeServices.CommentScoreService.Remove(commentScore);
            }
            //اگر کاربر لایک کرده بود و بعد روی دیسلایک کلیک کرد باید ایز لایک را به فالس تغییر دهیم
            //اگر کاربر دیسلایک کرده بود و بعد روی لایک کلیک کرده بود باید ایز لایک رو به ترو تغییر دهیم
            else
            {
                operation = "AddAndSubtract";
                commentScore.IsLike = !commentScore.IsLike;
            }

            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, string.Empty)
            {
                Data = operation
            });
        }

        public async Task<IActionResult> OnGetShowQuestionsByPagination(long productId, int pageNumber,
            int questionsPageCount,
            QuestionsSortingForProductInfo sortBy, SortingOrder orderBy)
        {
            if (!await _facadeServices.ProductService.IsExistsBy(nameof(MarketKhoone.Entities.Product.Id), productId))
            {
                return Json(new JsonResultOperation(false));
            }

            var questions =
                await _facadeServices.QuestionAndAnswerService.GetQuestionsByPagination(productId, pageNumber, sortBy,
                    orderBy);


            var model = new QuestionAndAnswerForQuestionAndAnswerPartialViewModel()
            {
                CurrentPage = pageNumber,
                QuestionsAndAnswersPagesCount = questionsPageCount,
                ProductQuestionsAndAnswers = questions
            };

            var userId = User.Identity.GetUserId();

            if (userId != null)
            {
                var answerIds = questions.SelectMany(x => x.Answers)
                    .Select(x => x.Id)
                    .ToArray();

                model.LikedAnswersByUser =
                    await _facadeServices.AnswerScoreService.GetLikedAnswersLikedByUser(userId.Value, answerIds);
            }

            return Partial("_QuestionsAndAnswersPartial", model);

        }

        public async Task<IActionResult> OnPostQuestionScore(long answerId, bool isLike)
        {
            var userId = User.Identity.GetUserId();

            if (userId is null)
            {
                return Json(new JsonResultOperation(false));
            }

            if (!await _facadeServices.QuestionAndAnswerService.IsExistsAndAnswer(answerId))
            {
                return Json(new JsonResultOperation(false));
            }

            var answerScore = await _facadeServices.AnswerScoreService.FindAsync(userId.Value, answerId);
            var operation = string.Empty;

            //اگر جود نداشته باشد اضاقه میکنیم
            if (answerScore is null)
            {
                operation = "Add";
                await _facadeServices.AnswerScoreService.AddAsync(new ProductQuestionAnswerScore()
                {
                    IsLike = isLike,
                    AnswerId = answerId,
                    UserId = userId.Value
                });
            }
            //اگر کاربر لایک کرده بود و بعد دوباره روی دکمه دیسلایک کلیک کرده بود باید لایک کاربر رو حذف کنیم
            //اگر کاربر دیسلایک کرده بود و بعد دوباره روی دیکمه دیسلایک کلیک کرده بود باید دیسلایک کاربر رو حذف کنیم
            else if (answerScore.IsLike && isLike || !answerScore.IsLike && !isLike)
            {
                operation = "Subtract";
                _facadeServices.AnswerScoreService.Remove(answerScore);
            }

            else
            {
                operation = "AddAndSubtract";
                answerScore.IsLike = !answerScore.IsLike;
            }

            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, string.Empty)
            {
                Data = operation
            });
        }

        public async Task<IActionResult> OnGetShowDiscountNotice(long productId)
        {
            var userId = User.Identity.GetUserId();

            if (userId is null)
            {
                return Json(new JsonResultOperation(false));
            }

            //گرفتن اطلاعات برای بخش اطلاع رسانی شگفت انگیز
            // برای مثال اگه اطلاع رسانی از ظریق شماره تلفن رو از قبل قعال کرده بود
            // باید چکباکس مربوطه رو تیک بزنیم
            var discountNotice =
                await _facadeServices.DiscountNoticeService.GetDataForAddDiscountNotice(productId, userId.Value) ??
                new();

            discountNotice.Email = User.Identity.GetUserClaimValue(ClaimTypes.Email);
            discountNotice.PhoneNumber = User.Identity.Name;
            return Partial("_DiscountNotice, discountNotice");
        }

        public async Task<IActionResult> OnPostAddDiscountNotice(AddDiscountNoticeViewModel model)
        {
            var userId = User.Identity.GetUserId();
            if (userId is null)
            {
                return Json(new JsonResultOperation(false));
            }

            var discountNotice = await _facadeServices.DiscountNoticeService.FindAsync(userId.Value, model.ProductId);
            //اگه هم ایمیل و هم موبایل و هم چت فالس باشه، این متغیر ترو میشه
            var isAllItemsFalse = !(model.NoticeViaChat && model.NoticeViaEmail && model.NoticeViaPhoneNumber);

            //اگه وجود نداشت اضافه کن
            //اگه وجود نداشت بروزرسانی کن
            if (discountNotice is null)
            {
                //اگر هیچکدام از موارد را کلیک نکرده بود، رکورد را اضافه نکن
                if (!isAllItemsFalse)
                {
                    var discountToAdd = _mapper.Map<MarketKhoone.Entities.DiscountNotice>(model);
                    discountToAdd.UserId = userId.Value;
                    await _facadeServices.DiscountNoticeService.AddAsync(discountToAdd);
                }
            }

            //اگر رکورد از قبل وجود دشات و هیچکدام از موارد تیک نخورده بود
            //یعنی باید رکورد را حذف کنیم
            else if (isAllItemsFalse)
            {
                _facadeServices.DiscountNoticeService.Remove(discountNotice);
            }
            else
            {
                discountNotice = _mapper.Map(model, discountNotice);
            }

            await _uow.SaveChangesAsync();
            return Json("عملیات با موفقیت انجام شد");
        }

        public async Task<IActionResult> OnGetShowUserList(long productId)
        {
            var userId = User.Identity.GetUserId();

            if (userId is null)
            {
                return Json(new JsonResultOperation(false));
            }

            var userLists = await _facadeServices.UserListService.GetUserListInProductInfo(productId, userId.Value);

            var modelToPass = new ShowUserListInProductInfoViewModel()
            {
                Items = userLists
            };
            return Partial("_UserList", modelToPass);
        }

        public async Task<IActionResult> OnPostUpdateUserList(long productId, List<long> userListIds)
        {
            var userId = User.Identity.GetUserId();
            if (userId is null)
            {
                return Json(new JsonResultOperation(false));
            }

            //تمامی لیست های کاربر
            var allUserListIds = await _facadeServices.UserListService.GetAllUserListIds(userId.Value);

            //چند تا لیست رو تیک زده؟ برای مثال دو تا
            //این دو رکورد رو در داخل تمامی لیست های کاربر سرچ میکنیم
            //اگه دوتا رکورد پیدا شد همه چی اوکیه
            if (!_facadeServices.UserListService.CheckUserListIdsForUpdate(userListIds, allUserListIds))
            {
                return Json(new JsonResultOperation(false));
            }

            //این محصول رو از تمامی لیست های کاربر حذف میکنیم
            var userListProductsToRemove =
                await _facadeServices.UserListProductService.GetUserListProducts(productId, allUserListIds);

            _facadeServices.UserListProductService.RemoveRange(userListProductsToRemove);

            //این محصول رو بعد از اینکه از تمامی لیست های کاربر حذف کردیم، مجداد به
            // لیست های که تیکشون فعال شده اضافه میکنیم

            var userListsProductToAdd = new List<MarketKhoone.Entities.UserListProduct>();

            userListIds.ForEach(x =>
            {
                userListsProductToAdd.Add(new UserListProduct()
                {
                    ProductId = productId,
                    UserListId = x
                });
            });

            await _facadeServices.UserListProductService.AddRangeAsync(userListsProductToAdd);
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "عملیات با موفقیت انجام شد"));
        }

        public async Task<IActionResult> OnPostAddUserList(ShowUserListInProductInfoViewModel model)
        {
            var userId = User.Identity.GetUserId();
            if (userId is null)
            {
                return Json(new JsonResultOperation(false));
            }

            var userListToAdd = _mapper.Map<MarketKhoone.Entities.UserList>(model.AddUserList);
            userListToAdd.UserId = userId.Value;

            var result = await _facadeServices.UserListService.AddAsync(userListToAdd);
            if (!result.Ok)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.DuplicateErrorMessage)
                {
                    Data = result.Columns.SetDuplicateColumnsErrorMessages<AddCategoryViewModel>()
                });
            }

            //گرفتن لینک رندوم برای این لیست
            var shortLink = await _facadeServices.UserListShortLinkService.GetUserListShortLinkForCreateUserList();
            userListToAdd.UserListShortLinkId = shortLink.Id;
            shortLink.IsUsed = true;

            await _uow.SaveChangesAsync();
            return JsonOk("لیست مورد نظر با موفقیت ایجاد شد", new
            {
                Id = userListToAdd.Id,
                Title = userListToAdd.Title
            });
        }

        public async Task<IActionResult> OnGetCheckForTitle(ShowUserListInProductInfoViewModel model)
        {
            var userId = User.Identity.GetUserId();

            if (userId is null)
            {
                return Json(true);
            }

            return Json(
                !await _facadeServices.UserListService.CheckForTitleDuplicate(userId.Value, model.AddUserList.Title));
        }
    }
}



