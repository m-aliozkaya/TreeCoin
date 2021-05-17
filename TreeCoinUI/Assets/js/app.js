(function ($) {
    "use strict";

    // Back to top button
    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('.back-to-top').fadeIn();
        } else {
            $('.back-to-top').fadeOut();
        }
    });
    $('.back-to-top').click(function () {
        $('html, body').animate({scrollTop: 0}, 1500, 'easeInOutExpo');
        return false;
    });
})(jQuery);

// $('.modal').on('shown.bs.modal', function (e) {
//   $("body").addClass("modal-open").css('padding-right', '17px');
// });

$('#onay').on('click', function(e){

    if (confirm("Almak istediğinize emin misiniz!")) {
        $('#modalStatic').modal('hide');
    } else {

    }
});


  
