$(document).ready(function () {
    const DURATION = 3000; // 3 seconds

    var validationSummary = $('.fadeout3s');

    setTimeout(function () {
        validationSummary.fadeOut('slow');
    }, DURATION);

});
