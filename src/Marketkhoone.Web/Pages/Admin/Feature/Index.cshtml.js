$(function () {
    fillDataTable();
   

    $('#Feature_SearchFeatures_CategoryId').change(function () {
        var selectedCategoryId = $(this).val();
        if (selectedCategoryId === 0) {
            $('#add-category-feature-for-selected-category').addClass('d-none');
        }
        else {
            var selectedCategoryText = $(this).find('option:selected').text();
            //میخواهیم وقتی توسط سلکت باکس دسته بندی
            //کاربر یه دسته بندی را انتخاب کرد
            // تا بهش فیچر اضافه کنه در مودالی که براش باز میشه
            // یو آر ال هنگام باز شدن مودال(نه زمانی که مودال کامل لود شده، هنگام باز شدن مودال دقت شود!) هم در مرورگر تغییر بکنه
            var addCategoryFeatureLink = $('#add-category-feature-link').attr('href');
            $('#add-category-feature-for-selected-category').removeClass('d-none');
            $('#add-category-feature-for-selected-category').html(`افزودن ویژگی دسته بندی برای "${selectedCategoryText}"`);

            //فرمت یو آر ال هنگامی که کاربر یه لحظه بعدش به مودال باز شده رسیده
            $('#add-category-feature-for-selected-category')
                .attr('href', `${addCategoryFeatureLink}&categoryId = ${selectedCategoryId}`);
        }
    });
});

function customAjaxFormFunction() {

    $('#Title').val('');
    $('#Title').focus();
}
  