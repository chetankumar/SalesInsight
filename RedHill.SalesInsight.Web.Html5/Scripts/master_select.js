function fetch_and_change(select, options, selected_opt, triggerChange) {
    $(select).html("");
    $(options).each(function (i, opt) {
        dom = $("<option>", opt);
        if (opt["value"] + "" == "" + selected_opt) {
            dom.attr("checked", "checked");
        }
        $(select).append(dom);
    });
    $(select).val(selected_opt);
    if (triggerChange === undefined || triggerChange === true)
        $(select).trigger("change");
}

$(document).ready(function () {
    $(".master_select").each(function (i, obj) {
        trigger_change($(obj));
    });

    $("body").on("change", ".master_select", function () {
        trigger_change($(this));
    });

    function trigger_change(element) {
		value = $(element).val();
		if (value != "") {
			url = $(element).data("url");
			selected = $(element).data("selected");
			dependent_selector = $(element).data("dependent");
			var triggerChange = $(element).attr("data-triggerChange") == undefined;
			$.get(url + value, function (data) {
				fetch_and_change(dependent_selector, data, selected, triggerChange);
				var event = jQuery.Event("masterSelectChange", { items: data, targetSelector: dependent_selector });
				$(element).trigger(event);
			}).fail(function () {
				fetch_and_change(dependent_selector, [], selected, triggerChange);
			});
		}
    }
});