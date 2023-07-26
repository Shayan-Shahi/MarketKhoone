$(function () {
    activationSwiper();
});

function activtionSwiper() {
    new Swiper('.product-images-in-profile-orders', {
        slidesPerView: 8,
        spaceBetween: 30,
        pagination: {
            el: '.swiper-pagination',
            clickable: true
        },
        breakpoints: {
            0: {
                slidesPerView: 5,
                spaceBeween: 30,
            },
            768: {
                slidesPerView: 8,
                spaceBetween: 30
            }

        }
    });
}
//نمایش صفحات به صورت صفحه بندی شده
function showOrdersByPagination(el) {
    if ($(el).hasClass('bg-danger')) {
        return;
    }

    var pageNumber = $(el).attr('page-number');
    getHtmlWithAJAX('?handler=ShowOrdersByPagination', { pageNumber: pageNumber }, 'showOrdersByPaginationFunction');

}
//نمایش محصولات به صورت صفحه بندی شده
function showOrdersByPaginationFunction(data) {
    $('#orders-box-in-comment-profile').html(data);
    activtionSwiper();
    convertingEnglishNumbersToPersianNumber();
    scroToEl('body');
}