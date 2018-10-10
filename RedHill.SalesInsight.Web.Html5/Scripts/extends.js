if (typeof Object.prototype.removeCommas != 'function') {
    String.prototype.removeCommas = function () {
        return this.replace(/,/g,'');
    };
}

jQuery.fn.extend({
    removeCommas: function () {
        return this.each(function () {
            val = this.val();
            val = val.replace(/,/g, '');
            this.val(val);
        });
    }
});

function removeCommas(value) {
    value = value.replace(/,/g, '');
    return value;
}

function addOption(selector, label, value) {
    select = $(selector);
    option = $("<option>");
    option.text(label);
    option.attr("value", value);
    select.append(option);
}
function removeOption(selector, value) {
    select = $(selector);
    select.find("option[value=" + value + "]").remove();
}

$(document).ready(function () {
    $(".remove_commas_btn").click(function () {
        form = $(this).parents("form").first();
        console.log(form);
        $(form).find(".comma_field").each(function (index, input) {
            value = $(this).val();
            $(this).val(removeCommas(value));
        });
        return true;
    });

    $(".auto_submit").change(function () {
        $(this).parents("form").submit();
    });

    $(".check_before_submit").click(function (event) {
        selector = $(this).data("selector");
        entity = $(this).data("entities");
        $(selector).each(function (index, e) {
            value = $(e).val();
            if (value == undefined || value == "") {
                alert("Please input " + entity + " before submitting");
                event.stopPropagation();
                event.preventDefault();
                return false;
            }
        });
        return true;
    });


    $(".with_close").click(function () {
        form = $(this).parents("form").first();
        form.submit();
        window.close();
        //setTimeout(function () { window.close(); }, 2000);
        return false;
    });
});