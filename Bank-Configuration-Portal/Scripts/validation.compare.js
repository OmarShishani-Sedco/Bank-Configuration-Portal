(function ($) {
    function num(v) {
        var n = parseFloat(v);
        return isNaN(n) ? null : n;
    }

    $.validator.addMethod("lessthan", function (value, element, other) {
        if (this.optional(element)) return true;
        var a = num(value),
            b = num($('[name="' + other + '"]', element.form).val());
        return a === null || b === null ? true : a < b;
    });

    $.validator.addMethod("greaterthan", function (value, element, other) {
        if (this.optional(element)) return true;
        var a = num(value),
            b = num($('[name="' + other + '"]', element.form).val());
        return a === null || b === null ? true : a > b;
    });

    $.validator.unobtrusive.adapters.add("lessthan", ["other"], function (options) {
        options.rules["lessthan"] = options.params.other;
        options.messages["lessthan"] = options.message;
    });

    $.validator.unobtrusive.adapters.add("greaterthan", ["other"], function (options) {
        options.rules["greaterthan"] = options.params.other;
        options.messages["greaterthan"] = options.message;
    });

    // Whenever one field changes, re-validate its counterpart
    $(document).on("input change", '[name="MinServiceTimeSeconds"], [name="MaxServiceTimeSeconds"]', function () {
        var $form = $(this.form);
        var otherName = this.name === "MinServiceTimeSeconds"
            ? "MaxServiceTimeSeconds"
            : "MinServiceTimeSeconds";
        $form.find('[name="' + otherName + '"]').valid();
    });
}(jQuery));
