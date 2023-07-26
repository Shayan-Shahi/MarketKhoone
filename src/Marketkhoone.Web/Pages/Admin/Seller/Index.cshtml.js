$(document).ready(function () {
    fillDataTable();
    appendHtmlModalPlaceToBody();
});
  


function getSellerDetails(e) {
    var sellerId = $(e).attr('seller-id');
    getHtmlWithAJAX('?handler=GetSellerDetails', { sellerId: sellerId }, 'showSellerDetailsInModal', e);
}

function showSellerDetailsInModal(result, clickedButton) {
   
    var currentModal = $('#html-modal-place');
    currentModal.find = $('.modal-body').html(result);
    currentModal.modal('show');
    $('#html-modal-place .modal-header h5').html($(clickedButton).text().trim());
    initializeTinyMCE();
    //چون فرم بعدا به صفحه اضافه میشه
    // باید بگیم بعد اینکه فرم ایجاد شد، اهتبار سنجی ها رو دوباره فعال کن
    $.validator.unobtrusive.parse($('#html-modal-place form'));
    activatingDeleteButtons();
}
// بعد از تایید و یا رد کردن فروشنده گرید را رفرش میکنیم
function sellerDocumentInManagingSellers (message) {
    showToastr('success', message);
    $('#html-modal-place').modal('hide');
    fillDataTable();
}