$(function () {
    $(document).on('keydown', '#variant-code-items-form-in-create-consignment input[type=number]', function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            return false;
        }
    });

    // حذف کردن
    //Tr
    //ها
    $(document).on('click', '.remove-product-variant-tr', function () {

        //مقدار
        //tr
        //که یه اینپوت هیدن در اینسپکت هست رو هم حذف کن
        var currentVariantCode = $(this).parents('tr').attr('variant-code');
        //وقتی که کاربر مقدار تنوع رو وارد کرده اینتر میزنه، ایجاد میشه...پس باید اون حالت خذف بشه
        $('#variant-code-items-form-in-create-consignment')
            .find('input[value="' + currentVariantCode + '"]').remove();

        //وقتی که کاربر مقدار تنوع رو وارد کرده ، و فرم رو سابمیت کرده ، ایجاد میشه...پس باید اون حالت حذف بشه
        $('#variant-code-items-form-in-create-consignment')
            .find('input[value^="' + currentVariantCode + '|||"]').remove();

        $(this).parents('tr').remove();
        if ($('#consignment-items tr').length === 0) {
            $('#record-not-found-box').removeClass('d-none');
              //دکمه ایجاد محموله رو غیر فعال کنه
             $('#send-consignment-submit-button').attr('disabled', 'disabled');
        }
    });

    const dtp1Instance = new mds.MdsPersianDateTimePicker(document.getElementById('delivery-date-in-create-consignment'), {
        targetTextSelector: '#CreateConsignment_DeliveryDate',
        persianNumber: true
    });


    //اجازه نمیدیم از یک تنوع بیش از یکبار در
    //Tr
    //ساخته بشه
    $('.get-html-by-sending-form').submit(function () {
        var selectedVariantCode = $('#VariantCode').val();
        if ($('#variant-code-items-form-in-create-consignment')
            .find('input:hidden[value^="' + selectedVariantCode + '|||"]').length > 0) {
            showToastr('warning', 'این تنوع محصول از قبل اضافه شده است');
            return false;
        }
    });

    //فرم رو که شامل
    //tr
    //هاست ارسال میکنیم
    $('#variant-code-items-form-in-create-consignment').submit(function () {

        //ایتدا همه اینپوت های به جز آر وی تی هدین در ایینسپکت رو حذف کن
        // از نو بساز
        $(this).find('input:hidden').not('input[name="' + rvt + '"]').remove();
        //روی همه تی آر ها، فور ایچ بزن
        $(this).find('table tbody tr').each(function () {
            var currentVariantCode = $(this).attr('variant-code');
            var currentProductCount = $(this).find('input').val();
            var parsedProductCount = parseInt(currentProductCount);
            if (parsedProductCount > maxCount || parsedProductCount < 1) {
                showToastr('warning', `تعداد هر محصول باید بین 1 تا ${maxCount} باشد`);
                return false;
            }
            $('#variant-code-items-form-in-create-consignment').prepend(
                `<input name="CreateConsignment.Variants" type="hidden" value="${currentVariantCode} ||| ${currentProductCount}"/>`
            );
        });

    });
  
});


var maxCount = 10000;
function appendProductVariantTr(result) {
    $('#record-not-found-box').addClass('d-none');
    $('#consignment-items').append(result);

    //مقدار مکسی رو به اینپوت ست کن
    // مقدار مین رو یک دادیم در کد اچ تی ام ال
    $('#consignment-items tr:last').find('input').attr('max', maxCount);

    //همین که یه
    //tr
    //ساخته شد
    //دکمه ایجاد محموله رو فعال کنه
    $('#send-consignment-submit-button').removeAttr('disabled');
    //به محض اینکه کاربر با اینتر کردن
    //مقدار تنوع رو وارد کرد تا
    //Tr
    //اش ساخته بشه
    //بیا و اینپوت های هیدن رو بساز

    var currentVariantCode = $('#consignment-items tr:last').attr('variant-code');
    $('#variant-code-items-form-in-create-consignment').prepend(
        //1
        //پیش فرضه
        `<input type="hidden" value="${currentVariantCode}|||1" />`
    );

  
}


function addConsignmentFunction(message, data) {
    showToastr('success', message);
    location.href = data;
}