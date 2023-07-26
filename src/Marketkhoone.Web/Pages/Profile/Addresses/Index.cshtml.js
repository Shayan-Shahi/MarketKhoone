var deleteAddressForm;
var deleteAddressEl;
var editAddressForm;
var editAddressEl;


$(function () {
    $('.btn-remove-address').click(function () {
        debugger;
        deleteAddressForm = $(this).parent('form');
        deleteAddressEl = $(this).parents('.profile-address-item');
        showSweetAlert2('آیا از حذف این آدرس از لیست آدرس ها اطمینان دارید؟', 'deleteAddress');
    });
});

$(function () {
    $('.btn-edit-address').click(function () {
        debugger;
        editAddressForm = $(this).parent('form');
        editAddressEl = $(this).parents('.profile-address-item');
        showSweetAlert2('آیا از ویرایش این آدرس اطمینان دارید؟', 'editAddress');
    });
});

function deleteAddress() {
    deleteAddressForm.submit();
}

function deleteAddressFunction(message) {
    showToastr('success', message);
    deleteAddressEl.remove();
}
function editAddress() {
    editAddressForm.submit();
}

function editAddressFunction(message) {
    showToastr('success', message);
    deleteAddressEl.remove();
}