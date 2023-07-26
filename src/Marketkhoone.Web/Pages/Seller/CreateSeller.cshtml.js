$('#legal-person-checkbox-create-seller').change(function () {

    var labelEl = $(this).parents('.form-switch').find('label');
    if (this.checked) {
        
        addRequiredRule('#CreateSeller_CompanyName');
        addRequiredRule('#CreateSeller_RegisterNumber');
        addRequiredRule('#CreateSeller_EconomicCode');
        addRequiredRule('#CreateSeller_SignatureOwners');
        addRequiredRule('#CreateSeller_NationalId');
        addRangeRule('#CreateSeller_CompanyType');

        labelEl.html('شخص حقوقی');
    }
    else {
        removeRequiredRule('#CreateSeller_CompanyName');
        removeRequiredRule('#CreateSeller_RegisterNumber');
        removeRequiredRule('#CreateSeller_EconomicCode');
        removeRequiredRule('#CreateSeller_SignatureOwners');
        removeRequiredRule('#CreateSeller_NationalId');
        removeRangeRule('#CreateSeller_CompanyType');
        labelEl.html('شخص حقیقی');
    }
    //وقتی تاگل شخحص حقیقی/حقوقی تغییر میکند
    // حتما باید بلر انجام بشه تا ارور های ولیدیشدن بالای فرم برداشته بشه
    //با این خط کد پایین میکیم همون لحظه تاگل بیا و فرم رو ولید کن تا اون خطا ها برداشته بشن
    //دیس همان چک باکس تاگل است
    $(this).parents('form').valid();
    $('#legal-person-box-create-seller').slideToggle();
});

$('#legal-person-box-create-seller').hide(0);

function addRequiredRule(selector) {
    var displayName = $(selector).parent().find('label').html().trim();
    $(selector).rules('add', {
        required: true,
        messages: {
            required: `لطفا ${displayName} راوارد نمایید`
        }
    });
}
//ولیدیشن مربوط به اینام در سلکت باکس نوع شرکت
function addRangeRule(selector) {
    var displayName = $(selector).parent().find('label').html().trim();
    $(selector).rules('add', {
        range :[0,4],
        messages: {
            range: `لطفا ${displayName} راوارد نمایید`
        }
    });
}
function removeRangeRule(selector) {
    $(selector).rules('remove', 'range');
}
//پایان ولیدیشن مربوط به اینام در سلکت باکس نوع شرکت 
function removeRequiredRule(selector) {
    //وقتی کاربر از حقوقی رفت به حقیقی- مثلا اشتباه اومده
    // الان بالای فرم این ولیدیشن های مربوط به شخص حقوقی هستن
    // با این خط کد میگیم اون ولیدشن ها رو حذف کن
    $('selector').rules('remove', 'required');
}
//در اولین لود صفحه، دکمه اول که دکمه "قبلی" هست رو غیر فعال بکن
$('#create-seller-container #previous-tab-create-seller').attr('disabled', 'disabled');

//========جابجایی بین نَو تب ها با استفاده از 3 باتن
var currentTab = $('#create-seller-container .nav-tabs button:first').attr('data-bs-target');

//از تب جاری، برو به تبی بعدی که کلیک کردم
$('#create-seller-container #next-tab-create-seller').click(function () {
    var nextTab = $(`#create-seller-container .nav-tabs button[data-bs-target="${currentTab}"]`).next();
    //میخواهیم دفعه سوم ، چهارم،.... هم که رو یدکمه بعدی کلیک کردیم، بره به تب بعدی
    //پس باید تب جاری مقدارش رو آپدیت کنیم
    if (nextTab.attr('data-bs-target')) {
        currentTab = nextTab.attr('data-bs-target');
        nextTab.tab('show');
    } 
    
});
$('#create-seller-container #previous-tab-create-seller').click(function () {
    var previousTab = $(`#create-seller-container .nav-tabs button[data-bs-target="${currentTab}"]`).prev();
    if (previousTab.attr('data-bs-target')) {
        currentTab = previousTab.attr('data-bs-target');
        previousTab.tab('show');
    }

});

///جابجایی بین نَو تب ها  با استفاده از خود نَو تب ها و سینک کرردن مقدار تب جاری با باتن ها
//show.bs.tab
// یه ایونت هست در نَو تب ها، که با اون مقدار جاری تب کلیک شده رو میگیریم




$('#create-seller-container .nav-tabs button').on('show.bs.tab', function (e) {

    currentTab = $(e.target).attr('data-bs-target');

    //با جابجایی دکمه ی قبلی و بعدی رو فعال یا غیر فعال بکن
    var firstTab = $('#create-seller-container .nav-tabs button:first').attr('data-bs-target');
    var lastTab = $('#create-seller-container .nav-tabs button:last').attr('data-bs-target');

    if (currentTab == lastTab) {

        $('#create-seller-container #next-tab-create-seller').attr('disabled', 'disabled');
    } else {
        $('#create-seller-container #next-tab-create-seller').removeAttr('disabled');
    }
    if (currentTab ==firstTab) {

        $('#create-seller-container #previous-tab-create-seller').attr('disabled', 'disabled');
    } else {
        $('#create-seller-container #previous-tab-create-seller').removeAttr('disabled');
    }

    // پایان  با جابجایی دکمه ی قبلی و بعدی رو فعال یا غیر فعال بکن
});

///=======پایان جابجایی بین نَو تب ها


///======گرفتن شهرها با تغییر وضعیت سلکت باکس استان ها بصورت ایجکسی

$('#CreateSeller_ProvinceId').change(function () {

    var formData = {
        provinceId: $(this).val()
    }
    getDataWithAJAX('/Seller/CreateSeller/test?handler=GetCities', formData, 'putCititesInTheSelectBox');
});

function putCititesInTheSelectBox(message, data) {
    $('#CreateSeller_CityId option').remove();
    $('#CreateSeller_CityId').append('<option value="0">انتخاب کنید</option>');
    $.each(data, function (key, value) {
        $('#CreateSeller_CityId').append(`<option value="${key}">${value}</option>`)
    });
}
///======پایان گرفتن شهرها با تغییر وضعیت سلکت باکس استان ها بصورت ایجکسی

//فعال ساز دیت تایم پیکر
const dtp1Instance = new mds.MdsPersianDateTimePicker(document.getElementById('birth-date-icon-create-seller'), {
    targetTextSelector: '#CreateSeller_BirthDate',
    persianNumber: true,
    selectedDate: new Date($('#CreateSeller_BirthDate').attr('birth-date-en') || new Date()),
    selectedDateToShow: new Date($('#CreateSeller_BirthDate').attr('birth-date-en') || new Date())
});


$('#CreateSeller_CompanyName').val('اسم شرکت تست');
$('#CreateSeller_RegisterNumber').val('456465456465456');
$('#CreateSeller_EconomicCode').val('123123123123');
$('#CreateSeller_SignatureOwners').val('علی احمدی - محمد محمودی');
$('#CreateSeller_NationalId').val('56456465456456');
$('#CreateSeller_CompanyType').val('2');
$('#ShopName').val('فروشگاه تست');
$('#CreateSeller_Address').val('آدرس کامل');
$('#CreateSeller_Website').val('https://google.com');
$('#CreateSeller_PostalCode').val('1234567890');
$('#CreateSeller_ShabaNumber').val('12345678901234567890');
$('#CreateSeller_Telephone').val('02122334455');
$('#CreateSeller_AboutSeller').val('<h3>Hello</h3>');
var firstOptionProvince = $('#CreateSeller_ProvinceId option:eq(1)').val();


function CreateSeller(message, data) {
    showToastr('success', message);
    location.href = data;
}