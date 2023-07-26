function getCategories() {
    getHtmlWithAJAX(`${location.pathname}?handler=GetCategories`, null, 'showCategories', null);
}

$(function () {

    //وقتی در تب دوم، کاربر برند را انتخاب کرد توسط سلکت باکس پر شده توسط ما
    //باید درصد کمیسیون مربوط به اون برند رو به اون نشان دهیم
    $('#Product_BrandId').change(function () {
        var selectedBrandId = $(this).val();
        if (selectedBrandId == 0) {
            $('#commission-percentage-place-in-create-product').addClass('invisible');
        }
        else {

            var formToSend = {
                brandId: selectedBrandId,
                categoryId: selectedCategoryId
            }
            getDataWithAJAX('?handler=GetCommissionPercentage', formToSend, 'showCommissionPercentage');

        }
    });


    getCategories();
    activatingModalForm();

    // تنظبمات مربوط به آپلود تصویر در تاینی ام سی ایی
    var speicaltyCheckTinyMce = tinymce.get('Product_SpecilatyCheck');
    speicaltyCheckTinyMce.settings.max_height = 1000;

    //توی لود اول، همه تب ها به چز تب اول رو غیر فعال بکن

    $('#add-product-tab button:not(:first)').attr('disabled', 'disabled');
    //آیکون قرمز رنگ شبیه به ورود ممنوع رو هنگامی که ماوس روی تب ها میره به جز اول نشون بده
    $('#add-product-tab button:not(:first)').addClass('not-allowed-cursor');

   
});
// با کلیک بر روی باتن "مرحله بعد" تب بعدی رو نشون بده
$(document).on('click', '.go-to-next-tab', function () {
    var nextTabId = $(this).parents('.tab-pane').next().attr('id');
    $(`#add-product-tab button[data-bs-target="#${nextTabId}"]`).tab('show');
});
    //چرا حتما باید بره به داخل داکیومنت آن؟
    // چونکه مثلا اطلاعات تب دوم، سوم و...
    //خلاصه اطلاعات تب های بعد تب اول د رهنگام لود اول
    // توی صفحه نیستن، و مثلا باتن "مرحله بعد" کار نخواهد کرد

    //$('.go-to-next-tab').click(function () {
    //    var nextTabId = $(this).parents('.tab-pane').next().attr('id');
    //    $(`#add-product-tab button[data-bs-target="#${nextTabId}"]`).tab('show');
    //});

function showCommissionPercentage(message, data) {
    $('#commission-percentage-place-in-create-product').removeClass('invisible');
    $('#commission-percentage-place-in-create-product')
        .html(` درصد کمیسیون فروش برای این دسته بندی و این برند
                                    <span class="text-danger">${data}</span>
                                    درصد میباشد.
                                    </div>`);
}

//وقتی روی هر بخش کلیک میشه
//selectedCategoryId
//مقدار دهی میشه  و پوش میشه به
//selectedCategoriesIds;
//الان مقدار گرفت این آرایه، بعد مثلا فور ایچ میزنیم روش تا برای باتن هایی
//که این المنت مقدار کلیک گرفته رو اکتیو=آبی رنگ کنیم
var selectedCategoriesIds = [];


//وقتی فروشنده درخواست برند جدیدی میکند
// یعنی یک دسته بندی را انتخاب کرده و به نب دوم رسیده
//متوجه شده که برندی که دنبالشه توی لیست برندهای اون دسته بندی توی سلکت باکس نیست
// میاد درخواست برند جدید میده
// چون در تب اول آیدی اون دسته بندی رو ما داریم الان
//بایداون آیدی دسته بندی رو هنگامی که روی لینک درخواست برند جدید
// کلیک کرد تا مودال براش باز بشه، به صفحه مودال و اکشن
//OnGet
//پاس بدیم تا موقع پست کردن بتونیم به سمت اکشن پست بفرستیم
var requestNewBrandUrl = $('#request-new-brand-url').attr('href');
//حالا میگیم زمانی که روی لینک درخوسا بند جدید کلیک شده
// بیا و اتریبیت
//href
// رو به زیری تغییر بده
//(برو به تابعی که روی لینک درخواست بند جدید هست نگاه کن)

function showCategories(data) {
    $('#product-category div.row.card-body').html(data);

    $('#product-category div.row.card-body button[has-child=true]').click(function () {

        //باتن انتخاب گروه کالا رو وقتی که زیردسته چایلد نداره فعال میکنیم، پس
        // وقتی به زیردسته ای رسید که چایلد داره باید غیر فعال کنیم
        // اینجا همه کارهایی که در حالت ایز چایلد فالس انجام دادیم رو بی اثر میکنیم
        $("#select-product-category-button").attr('disabled', 'disabled');
        $("#select-product-category-button").addClass('btn-light');
        $("#select-product-category-button").removeClass('btn-primary');

        var selectedCategoryId = $(this).attr('category-id');
        //به آرایه اضافه میکنیم
        selectedCategoriesIds.push(selectedCategoryId);
        getHtmlWithAJAX(`${location.pathname}?handler=GetCategories`, { selectedCategoriesIds: selectedCategoriesIds }, 'showCategories', null);

        //رنگ آبی مثلا زیر دسته های دو به بالا رو حذف کن
        var selectedRow = parseInt($(this).parent().attr('category-row'));
        for (var counter = selectedRow; counter <= selectedCategoriesIds.length; counter++) {
            $('#product-category div[category-row= ' + counter + '] button').removeClass('active');
        }
        //رنگ آبی رو چطور برای زیر دسته هایی که
        //has-child=false;
        // دارن برداریم؟
        // با کد پایین
        $('#product-category button[has-child=false]').removeClass('active');



        $(this).addClass('active');
        //اینجا مثل زیر دسته دو کلیک شده
        // هر چی توی آرایه
        //selectedCategoriesIds
        //هست رو خالی میکنیم و دوباره اضافه میکنیم از اول
        selectedCategoriesIds = [];

        //برای خوندن مقداری که سلکتور بهش اشاره داره از ایچ استفاده میکنیم
        $('#product-category button.active').each(function () {
            var newSelectedRow = $(this).attr('category-id');
            selectedCategoriesIds.push(newSelectedRow);

        });

        // الان که به تب دوم رفتیم، همه تب ها رو فعال بکن

        $('#add-product-tab button:not(:first)').removeAttr('disabled');
        //آیکون قرمز رنگ شبیه به ورود ممنوع رو هنگامی که ماوس روی تب ها میره رو هم بردار
        $('#add-product-tab button:not(:first)').removeAttr('not-allowed-cursor');

        // اگر کاربر وسط کار برگشت به تب اول تا دسته بندی رو تغییر بده
        // میدونیم بهش یه سوییت الرت نشون میدیم و میگیم همه اطلاعاتت پاک میشه
        // موافقی؟ اگر در سوییت الرت ، بله رو زد. باید همه اینپوت ها به جز آر وی تی و اینپئت هیدن کتگوری آیدی  رو خالی کنیم
        $('#add-product-form input').not(`[name="${rvt}"], #Product_MainCategoryId`).val('');
        //تاینی ام سی ها رو هم خالی کن
        tinymce.get('Product-ShortDescription').setContent('');
        tinymce.get('Product-SpecialtyCheck').setContent('');
        //پیش نمایش عکس های محصول رو هم  خالی کن
        $('#product-images-preview-box').html('');
    });

   
  

    //توی نوار خاکستری هربار زیردسته قبلی رو هم اَپند میکنه
    // باید بیگیم یه بار خالی کن تا در نوار خاکشتری مثلا دوبار ننویسه کالای دیجیتال> کالای دیجیتال> لپتاب
    $('#selected-categories-for-add-product').html('');

    //رنگ موارد انتخاب شده رو آبی کن
    //برای خوندن آرایه از فور ایچ استفاده میکنیم

    selectedCategoriesIds.forEach(element => {
        $('#product-category button[category-id=' + element + ']').addClass('active');


        //میخواهیم روی  زیر دسته هایی که چایلد ترو دارن کلیک شده، متنش رو در نواری خاکستری رنگ زیری بنویسه

        var currentCategory = $('#product-category button[category-id=' + element + ']');
        var currentCategoryText = currentCategory.text().trim();

        $('#selected-categories-for-add-product').append(
            `<span>${currentCategoryText} <i class="bi bi-chevron-left"></i> </span>`
        );

    });

    //======قسست زیردسته هایی که چایلد ندارند
    $('#product-category div.row.card-body button[has-child=false]').click(function () {


        //باتن انتخاب گروه کالا رو وقتی که زیردسته چایلد نداره فعال میکنیم

        $("#select-product-category-button").removeAttr('disabled');
        $("#select-product-category-button").removeClass('btn-light');
        $("#select-product-category-button").addClass('btn-primary');


        //اگر ابتدا به زیر دسته ایی که
        //has-child
        //اش ترو هست کلیک شود، و سپس به زیر دسته ای که
        //has-child = false;
        //است کلیک شود باید رنگ آبی= اکتوی بودن متناسبا تغییر کند
        var selectedRow = parseInt($(this).parent().attr('category-row'));
        $('#product-category div[category-row=' + selectedRow + '] button').removeClass('active');


        //یه حالت داریم مثلا کلا چهار تا زیر دسته هست
        //زیر دسته سه هم خودش که پرند حساب میشه برای زیر دسته چهار
        //خودش یه بخش داره، که چایلد نداره در زیر دسته چهار
        // الان تا زیر دسته چهار رنگ آبی هست
        // میخوایم کلیک کنیم روی زیر دسته سه ایی که چایلد نداره
        // باید رنگ زیر دسته چهار رو برداریم

        for (var counter = selectedRow; counter <= selectedCategoriesIds.length; counter++) {
            $('#product-category div[category-row =' + (selectedRow + 1) + ']').remove();
        }
        //اینبار میخواهیم روی  زیر دسته هایی که چایلد فالس دارن کلیک شده، متنش رو در نواری خاکستری رنگ زیری بنویسه 

        $(this).addClass('active');

        $('#selected-categories-for-add-product').html('');
        $('#product-category button.active').each(function () {

            var currentCategory = $(this);
            var currentCategoryText = currentCategory.text().trim();
            if (currentCategory.attr('has-child') === 'true') {
                $('#selected-categories-for-add-product').append(
                    `<span> ${currentCategoryText} <i class="bi bi-chevron-left"></i> </span>`
                );
            }
            else {
                $('#selected-categories-for-add-product').append(
                    `<span>${currentCategoryText}</span>`
                );
            }
        })
    });

    //اگر حداقل یک زیر دسته انتخاب شده بود، بیا و دکمه ضربدر و رو فعال کن
    // با کلیک بر روی ضریدر
    //getCategories();
    //رو صدا بزن
    $('#reset-product-category-button').click(function () {
        if ($('#selected-categories-for-add-product span').length > 0) {
            //وقتی روی یه زیر دسته ای کلیک میکنیم
            //selectedCategroiesIds[]
            //مقداردهی میشه و دوباره به
            //getCategories();
            //ارسال میشه، پس خالیش میکنیم تا دوباره نره به سمت تابع
            selectedCategoriesIds = [];

            //متن نوار خاکستری رو خالی میکنیم
            //$('#selected-category-for-add-product').html('');
            getCategories();
        }
    })
}


// لیست آیدی های دسته بندی هایی که انتخاب شده است
var finalSelectedCategoriesIds = [];

// آیا روی دکمه خیر در آلرت کلیک شده است
var isUndoClicked = false;

//جلسات 100 به بعد

//مثلا کاربر یه دسته بندی رو توی تب اول انتخل کرده، رفته تب دوم تا اطلاعا محصول رو وارد کنه
// پشیمون میشه و برمیگرده تب اول و یه دسته بندی دیگه رو انتخاب میکنه اینجا باید اطلاعات
//تب دوم یا سوم یا.... رو کالا خالی کنیم
// پس باید بدونیم الان که کاربر در تب مثلا سوم هست در تلاس اول به تب سوم رسیده یا در تلاش بیش از بار اول
var isCategoryAlreadySelected = false;

//فرضا کاربر در تب اول در اون زیر دسته ها بالاخره یه چیزی رو انتخاب کرد
//مثلا میخواد موبایل اضافه کنه
// در تب دوم باید بریم همه دسته بندی های مربوط به موبایل رو بیاریم و در سلکت باکس نشون بدیم
var selectedCategoryId;
$('#select-product-category-button').click(function () {

    if (isCategoryAlreadySelected) {
        showSweetAlert2('تعییر دسته بندی منجر به از بین رفتن تمامی دسته بندی ها خواهد شد، آیا مطمئن به انجام این کار هستید؟', 'emptyAllInputsAndShowOtherTabs', 'undoSelectedCategoryButton')
    }
    else {
        emptyAllInputsAndShowOtherTabs();
    }
    isCategoryAlreadySelected = true;
});
//اگر کاربر در سوییت الرت گفت نمیخوام دسته بنئدی تغییر کنه
// باید دوباره دسته بندی جدیدی که انتخاب کرده و رنگ آبی رو گرفته
// برداریم و به همون دسته بندی ایی که قبلا اطلاعاتش رو در تب های بعدی پر کرده بود، در تب اول  اون دسته بندی
//رو آبی کنیم
function undoSelectedCategoryButton() {
    //از همه دسته بندی هایی کلا کلاس اکتیو رو بگیر
    $(`#product-category button`).removeClass('active');
    //به اون دسته بندی که الان میدونیم انتخاب اولی کاربر هست، کلاس اکتیو رو بده
    // چون کاربر با پیغامی که از سوویت الرت ما گرفته، خیر زده
    $(`#product-category button[category-id="${selectedCategoryId}"]`).addClass('active');

}

//اگر کاربر در سوییت الرت بله رو زد، یعنی گفت  مهم نیست اطلاعاتی که برای یه دسته بندی
// وارد کردم پاک میشه، فقط عوض کن دسته بندی رو 
function emptyAllInputsAndShowOtherTabs() {
    //در اون زیر دسته ها که بالاخره کاربر به یکیش ختم میشه!!! و رنگش آبی هست یعنی اکتیو
    //اتریبویوت
    //category-id =@product.Id
    //یعنی آیدی زیر دسته نهایی رو بگشت زن تا ما بفرستیم سمت سرور و برند هایش رو بگیریم و در سلکت باکس بریزیم
    selectedCategoryId = $('#product-category div.list-group.col-4:last button.active').attr('category-id');
    getDataWithAJAX('?handler=GetCategoryInfo', { categoryId: selectedCategoryId }, 'categoryInfo');

    //مقدار اینپوت هیدن رو هم به سلکتید کتگوری آیدی تغییر بده
    $('#Product_MainCategoryId').val(selectedCategoryId);
    //اگر کاربر روی لینک درخواست برند جدید کلیک کرد، باید آیدی دسته بندی رو هم به مودال بفرستیم
    //RequestNewBrand
    //رو بالا گرفتیم
    $('#request-new-brand-url').attr('href', requestNewBrandUrl + '&categoryId=' + selectedCategoryId);
}
function categoryInfo(message, data) {

    //===Show CategoryBrands
    //از تب اول برو به تب دوم
    $('#add-product-tab button[data-bs-target="#product-info"]').tab('show');

    //روی دیتا های برگشتی که لیستی از آیدی ها هستن، فور ایچ بزن

    //ققط اول یه بار سلکت باکس رو خالی کن بعد پرش کن
    $('#Product_BrandId option').remove();
    //قبل اینکه جلقه فور ایچ رو بزنی، این انتخاب کنید رو به سلکت باکس اضافه کن
    $('#Product_BrandId').append('<option value="0">انتخاب کنید</option>');
    for (brandId in data.brands) {
        $('#Product_BrandId').append(`<option value="${brandId}">${data.brands[brandId]}</option>`);
    }
    $('#add-product-tab button[data-bs-target="#product-info"]').tab('show');

    //=== End Show CategoryBrands


    //===changeIsFakeStatus

    if (data.canAddFakeProduct === false) {
        //بخش مربوط به چک باکس افزودن کالای غیر اصل رو غیرفعال کن
        $('#Product_IsFake').attr('disabled', 'disable');
        //تیک چک باکس رو هم غیر فعال بکن
        $('#Product_IsFake').prop('checked', false);

    }
    else {
        $('#Product_IsFake').removeAttr('disabled');

    }
    //===End changeIsFakeStatus

    //== showCategoryFeatures

    $('#product-features .card-body.row').html(data.categoryFeatures);
    // == End ShowCategoryFeatures


    //اگر سلک تویی، با
    //getDataWithAJAX
    //پر بشه، با توجه به
    //Site.js اش
    //میایم و سلکت تو را فعال میکنیم
    initializeSelect2WithoutModal();

    //برای حالتی کخ کاربر به تب دوم رسیده، برند  رو هم از سلکت باکس انتخاب کرده
    // بعد پشیمون میشه و میره تب اول و میخواد که نوع دسته بندی رو حذف کنه
    //این بخش رو برای بار دوم که کاربر داره به تب دوم میاد، باید پاک کنیم
    // جون براساس برندی هست که بار اول انتخاب شده بود
    $('#commission-percentage-place-in-create-product').addClass('invisible');
}




$(document).on('change', '#IsIranianBrand', function () {
    var textToReplace = this.checked ? 'ایرانی' : 'خارجی';
    $(this).parents('.form-switch').find('label').html(textToReplace);
})

function createProductFunction(message, data) {
    showToastr('success', message);
    location.href = data;
}