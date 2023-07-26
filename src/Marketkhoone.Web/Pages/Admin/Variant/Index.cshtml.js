$(function () {
    fillDataTable();
    initializingAutocomplete();
    $('.custom-ajax-form').submit(function () {
        $('input:not(:first)').prop("disabled", false);
    });
});

$(document).on('change', 'form input', function () {
    if ($.isNumeric($(this).val())) {
        $('.custom-ajax-form input:not(:first)').attr('disabled', 'disabled');
  
    } else {
        $('.custom-ajax-form input:not(:first)').removeAttr('disabled');
    }
    $('.custom-ajax-form').submit(function () {
        $('input:not(:first)').prop("disabled", false);
    });
});

function confirmRejectRemoveMessageFunction(message) {
    showToastr('success', message);
    $('#form-modal-place').modal('hide');
    fillDataTable();
}