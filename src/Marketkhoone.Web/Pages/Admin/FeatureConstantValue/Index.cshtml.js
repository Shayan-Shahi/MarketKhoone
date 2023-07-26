$(function () {
    fillDataTable();

    //کد اضافه کردن در اولین سلکت کاربر
    $('#FeatureConstantValues_SearchFeatureConstantValues_CategoryId').change(function () {
        var selectedCategoryId = $(this).val();
        getDataWithAJAX(`${location.pathname}?handler=GetCategoryFeatures`, { categoryId: selectedCategoryId }, 'fillFeatureInSelectBox');
    });
    //===========بخش مودال
    // ما در مودال اضافه کردن ویژگی ثابت دو تا سلکت باکس داریم و یک اینپوت
    //سلکت باکس اولی دسته بندی هاست، که موقع لودینگگ پر شده به کاربر نشون داده میشه
    // تا کاربر یکیش رو انتخاب کنه
    // وقتی یکیش رو انتخاب کرد، با استفاده از دیگر کد های جاو اسکریپتی اینجا
    //اون سلکت باکس دوم که خود ویژگی هست پر میشه
    //الان میخواهیم کدی رو بنویسییم که کاربر دسته بندی رو انتخاب کرد  سلکت باکس دوم پر بشه
    // الان فقط رفتیم به مودال، پس داکیومنت ردی

    $(document).on('change', '#CategoryId', function () {
        var selectedCategoryId = $(this).val();
        getDataWithAJAX(`${location.pathname}?handler=GetCategoryFeatures`, { categoryId: selectedCategoryId }, 'fillFeaturesInModalSelectbox');
    })
    

});
//کد اضافه کردن در سلکت کاربر
function fillFeatureInSelectBox(message, data) {
    $('#FeatureConstantValues_SearchFeatureConstantValues_FeatureId').html('');
    $('#FeatureConstantValues_SearchFeatureConstantValues_FeatureId').append('<option value="0">انتخاب کنید</option>');
    data.forEach(function (item) {

        var optionToAdd = `<option value="${item.featureId}">${item.featureTitle}</option>`;
        $('#FeatureConstantValues_SearchFeatureConstantValues_FeatureId').append(optionToAdd);
    });
}
//کد اضافه کردن در سلکت کاربر در مودال
function fillFeaturesInModalSelectbox(message, data) {
    $('#FeatureId').html('');
    $('#FeatureId').append('<option value="0">انتخاب کنید</option>');
    data.forEach(function (item) {

        var optionToAdd = `<option value="${item.featureId}">${item.featureTitle}</option>`;
        $('#FeatureId').append(optionToAdd);
    });
}