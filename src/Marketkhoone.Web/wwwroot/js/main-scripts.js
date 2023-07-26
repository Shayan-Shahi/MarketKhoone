var isAuthenticated;
var loginPageUrl;

$(document).ready(function () {
    var win = $(this);
    var mainInputEl = $('#main-search-input');
    if (win.width() > 456) {
        mainInputEl.attr('placeholder', 'نام کالا، برند یا دسته مورد نظر خود را جستجو نمایید ...');
    } else {
        mainInputEl.attr('placeholder', 'جستجو ...');
    }
    $(window).on('resize', function () {
        win = $(this);
        if (win.width() > 456) {
            mainInputEl.attr('placeholder', 'نام کالا، برند یا دسته مورد نظر خود را جستجو نمایید ...');
        } else {
            mainInputEl.attr('placeholder', 'جستجو ...');
        }
    });


    isAuthenticated = $('body').attr('is-authenticated') === 'true';
    loginPageUrl = $('body').attr('login-page-url');

    $('body').removeAttr('is-authenticated');
    $('body').removeAttr('long-page-url');

    
});



const firstLoginModalBody = `<div class="modal" id="first-login-modal">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-body text-center">
                <h6 class="mt-1">
                    لطفا ابتدا وارد مارکت خونه شوید...

                </h6>

           
                    <a class="btn btn-success mt-5 mx-3">ورود</a>
                    <button  class="btn btn-danger mt-5" id="hi" data-bs-dimiss="modal">بستن</button>
          
            </div>
        </div>
    </div>
</div>`;

function showFirstLoginModal() {
    if ($('#first-loging-modal').length === 0) {
        $('body').append(firstLoginModalBody);
        $('#first-login-modal a').attr('href', loginPageUrl);
      
    }
    $('#first-login-modal').modal('show');
   
}
