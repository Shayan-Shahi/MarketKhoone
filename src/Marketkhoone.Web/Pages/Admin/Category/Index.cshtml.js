$(function () {
    fillDataTable();

    // زمانی که روی دکمه حذف کردن تنوع دسته بندی کلیک شد
    // استفاده شده در مودال ویرایش تنوع دسته بندی
    $(document).on('click', '.remove-selected-variant-button', function () {
        var variantId = $(this).parent().attr('variant-id');

        // حذف اینپوت با تایپ
        // Hidden
        $(this).parents('form').find('input[name="SelectedVariants"][value="' + variantId + '"]').remove();

        // حذف خود دکمه از بخش تنوع های این دسته بندی
        $(this).parent().remove();
        $(this).parent().remove();
    });

    // زمانیکه روی هر کدام از تنوع های بخش تمامی تنوع ها کلیک شد
    // باید اون تنوع رو به بخش تنوع های این دسته بندی اضافه کنیم
    $(document).on('click', '.variant-item-button', function () {
        var variantId = $(this).attr('variant-id');

        // اگه از قبل این تنوع اضافه شده بود
        if ($(this).parents('form').find('input[name="SelectedVariants"][value="' + variantId + '"]').length > 0) {
            showToastr('warning', 'این تنوع از قبل برای این دسته بندی اضافه شده است');
            return;
        }

        // اینپوت مخفی رو به فرم اضافه میکنیم که بعدا بتونیم اینو به سمت سرور بفرستیم
        $(this).parents('form').prepend('<input type="hidden" name="SelectedVariants" value="' + variantId + '" />');
        var variantToAppend =
            '<button variant-id="' + variantId + '" type="button" class="mx-1 p-2 badge rounded-pill bg-primary border-0">' +
            $(this).html() +
            '</button>';

        // باتن مربوطه رو به بخش تنوع های این دسته بندی اضافه میکنیم
        $('#selected-variants-box').append(variantToAppend);
        $('#selected-variants-box span:last').addClass('me-1');
        // اضافه کردن دکمه حذف کردن به تنوعی که الان اضافه کردیم
        $('#selected-variants-box button:last').append('<i class="bi bi-x-circle remove-selected-variant-button"></i>');
    });

  
});

var brandBox = `<div class="btn-group m-1">
                <button type="button" class="btn btn-outline-dark">
                    [brand title]
                </button>
                <button type="button" class="btn btn-info text-white">
                    %
                    [commission percentage]
                </button>
                <button type="button" class="btn btn-danger remove-selected-brand">
                    <i class="bi bi-x-lg"></i>
                </button>
            </div>`

function onAutocompleteSelect(event, ui) {
    var enteredBrand = ui.item.value;
  
    //Autcomplete
    //یگ سری کار پیش فرض رو مقادیر وارد شده بهش را انجام میده
    // مثلا نمیزاره ما بعد اینکه اینتر زدیم تا لیبل برند وارد شده زیرش ساخته بشه
    // رو خالی کنیم، میگیم بیا و مانع کار پیش فرض اوتو کامپیلیت بشو
    event.preventDefault();

    //بعد اینکه کاربر برند را وارد کرد و اینتر زد، بیا و اینپوت رو خالی کن
    $(event.target).val('');
    var commissionPercentage = $('#commission-percentage-input').val();
    if (isNullOrWhitespace(commissionPercentage)) {
        showToastr('error', 'لطفا درصد کمیسیون را وارد نمایید');
        return;
    }
    var parsedCommissionPercentage = parseInt(commissionPercentage);
    if (parsedCommissionPercentage > 20 || parsedCommissionPercentage < 1) {
        showToastr('error', 'درصد کمیسیون باید بین 1 تا 20 درصد باشد');
        return;
    }
    //نزار کاربر یه برند رو دوبار وارد کنه، اگر هم یه دسته بندی از قبل
    //دارای یه برندی هست، اجازه نده دوباره کاربر بتونه همون رو اضافه کنه
   // این فقط کنترل سمت فرانت هست
   //سمت بکند هم با متد
   //Distinct
   //آیدی های تکراری برگشت زده  شده از سرویس رو حذف میکنیم
    if ($('#add-brand-to-category-form input[type="hidden"][value^="' + enteredBrand + '|||"]').length == 0) {
        var brandBoxToAppend = brandBox.replace('[brand title]', enteredBrand);
        brandBoxToAppend = brandBoxToAppend.replace('[commission percentage]', commissionPercentage);
        //یه قسمت داریم که میگه برندی برای این دسته بندی انتخاب نشده است
        //یه دیووه که اگه دسته بندی برندی نداشت به کاربر نشونش میدیم
        // به محض اینکه کاربر اومد و یه برندی رو به دسته بندی اضافه کرد
        //باید پیغام (این دیوو) رو  برش داریم
        $('#empty-selected-brands').addClass('d-none');
        $('#selected-brands-box').append(brandBoxToAppend);
  

        //بعد زدن اینتر توسط کاربر، لیبل برند ساخته میشه زیر اوتوکامپیلین
        // حالا این لیبل رو میریزیم توی یه اینپوت تا به سمت سرور ارسالش کنیم
        var inputToAppend = `<input type="hidden" name="SelectedBrands" value="${enteredBrand}|||${commissionPercentage}" />`;
       
 
        $('#add-brand-to-category-form').prepend(inputToAppend);
       /* showToastr('success', 'برند مورد نظر به لیست اضافه شد، و در انتظار تایید نهایی ش')*/
    }
    else {
        showToastr('warning', 'این برند از قبل اضافه شده است')
    }
}

//دکمه ضربدر قرمز رنگ موجود در لیبل رو حذف کن
// میدونی دیگه چرا رفته به داخل
//$(document).on
//چون اول، صفحه لود میشه....بعد کاربر میاد، برندی رو وارد میکنه، بعد اینتر میزنه، تازه اینجا لیبل ساخته میشه
//یعنی در هنگام لود صفحه چنین المنتی(ضربدر) وجود نداره پس باید بره داخل
//$(document).on
$(document).on('click', '.remove-selected-brand', function () {
    // شروع لیبل ها را حذف کن
    var selectedBrandText = $(this).parent().find('button:first').text().trim();
    $(this).parent().remove();
    // پایان لیبل ها را حذف کن
    //ما مقدار این لیبل ها ر میریختیم داخل اینپوت هیدن تا به سمت سرور بفرستیم
    // الان باید اون اینپوت های هیدن مرتبط با این لیبل حذف شده رو حذف کنیم
    $('#add-brand-to-category-form input[value^="' + selectedBrandText + '|||"]').remove();

  /*  showToastr('success', 'برند مورد نظر با موفقیت از این دسته بندی حذف شد')*/

    // حالا که داریم با زدن رو ضربدر لیبل ها رو حذف میکنیم
    //حواست باشه که اگر این لیبل ها تعدادشون صفر شده
    // پیغام برندی برای این دسته بندی انتخاب نشده است
    // رو دوباره نشون بده
    if ($('#selected-brands-box .btn-group').length == 0) {
        $('#empty-selected-brands').removeClass('d-none');
    }
});




//نباید اجازه دهیم کاربر با زدن اینتر فرم مودال رو ارسال کنه
$(document).on('keydown', '#search-brand', function (event) {
    if (event.key == 'Enter') {
        event.preventDefault();
    }
});