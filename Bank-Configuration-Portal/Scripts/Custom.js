
//fade out  after 3 seconds elements with class fadeout3s
$(document).ready(function () {
    const DURATION = 3000; // 3 seconds

    var validationSummary = $('.fadeout3s');

    setTimeout(function () {
        validationSummary.fadeOut('slow');
    }, DURATION);

});
