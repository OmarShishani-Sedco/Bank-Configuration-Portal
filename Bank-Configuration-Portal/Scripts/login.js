$(document).ready(function () {
    const DURATION = 3000; // 3 seconds

    var validationSummary = $('#validationSummary');
    if (validationSummary.find('li').length > 0) {
        setTimeout(function () {
            validationSummary.fadeOut('slow');
        }, DURATION);
    }
});
