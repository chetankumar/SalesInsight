$(document).ready(function () {
	$(window).resize(function () {
		hideShowSearchMixFormulationColumns();
	});
	$(".show_formulation_search").click(function () {
		$("#SearchMixFormulationModal").modal("show");
	});
	$(".search_mix_formulations_btn").click(function () {
		$(".searching_mixes").show();
		update_search_formulation_modal();
	});

	$(".reset_mix_formulations_btn").click(function () {
		reset_search_formulation_modal();
		update_search_formulation_modal();
	});

	$("body").on("click", ".push_selected_mix", function () {
		quick_add_mix($(this).attr("data-id"));
	});

});

var sDom = null;
sDom = '<"top">frt<"bottom"i><"clear"><"paging"lp><"clear">';

function construct_mix_formulation_search_params() {
	params = {};
	$(".search_formulation_form").find("select").each(function (index, object) {
		value = $(object).val();
		if (value != "" && value != null)
			params[$(object).attr("name")] = value;
	});
	params["plantId"] = pricing_plant_id;
	params["pricingMonth"] = pricing_month.toISOString();
	params["quoteId"] = qId;
	console.log(params);
	return params;
}

function reset_search_formulation_modal() {
	$(".resetting_mixes").show();
	$(".search_formulation_form").find("select").each(function (index, object) {
		$(object).val("");
		//$(object).multiselect("rebuild");
		if ($(object)[0] && $(object)[0].sumo) {
			$(object)[0].sumo.reload();
		}
	});
}

function update_search_formulation_modal() {
	$(".mix_formulation_loader").show();
	url = "/Quote/SearchMixFormulations";

	params = construct_mix_formulation_search_params();
	jQuery.ajaxSettings.traditional = true;
	$.ajax({
		type: "POST",
		url: url,
		dataType: "json",
		traditional: true,
		data: params,
		success: function (data) {
			refreshMixFormulationSearch(data);
		},
		complete: function () {
			$(".searching_mixes").hide();
			$(".resetting_mixes").hide();
			$(".mix_formulation_loader").hide();
			setTimeout(function () {
				hideShowSearchMixFormulationColumns();
			}, 0);
		}
	});
}
function hideShowSearchMixFormulationColumns() {
	$("#search_mix_formulation_table").show();
	var searchMixFormulationTable = $("#search_mix_formulation_table").DataTable();
	var currentScreenWidth = $(window).width();
	var columnPosition = [6, 11, 10, 9, 8, 3, 2, 4, 5, 7, 1, 12, 0];
	var screenWidths = [1224, 1085, 1024, 992, 965, 820, 760, 600, 540, 500, 431, 366, 300];
	var finalWidthPosition = -1;
	$.each(screenWidths, function (i, v) {
		if (currentScreenWidth < screenWidths[i]) {
			finalWidthPosition = i;
		}
		else {
			return false;
		}
	});
	searchMixFormulationTable.columns(columnPosition.splice(0, finalWidthPosition + 1)).visible(false);
	searchMixFormulationTable.columns(columnPosition).visible(true);

	$("#search_mix_formulation_table_wrapper").show();
}
function sortNatural(arr) {
	var alphas = [], numerics = [], alphaNumeric = [];

	function naturalSort(a, b) {
		if ((isNaN(a) && isNaN(b))) {
			if (String(a).match(/\d+%/) && String(b).match(/\d+%/))
				return (parseFloat(b) < parseFloat(a)) - (parseFloat(a) < parseFloat(b));
			else
				return (b.toLowerCase() < a.toLowerCase()) - (a.toLowerCase() < b.toLowerCase());
		}
		a = parseFloat(a); b = parseFloat(b);
		return (b < a) - (a < b);
	}

	function alpaNaturalSort(a, b) {
		var a1 = parseFloat(a), b1 = parseFloat(b);
		if (!isNaN(a1) && isNaN(b1)) {
			return 1;
		} else if (isNaN(a1) && !isNaN(b1)) {
			return -1;
		}
		a = parseFloat(a); b = parseFloat(b);
		return (b < a) - (a < b);
	}

	if (arr.length) {
		alphaNumeric = arr.filter(function (x) { return String(x).match(/\d+%/); });
		alphas = arr.filter(function (x) { return isNaN(x) && !String(x).match(/\d+%/); });
		numerics = arr.filter(function (x) { return !isNaN(x); });
	}

	arr = numerics.sort(alpaNaturalSort).concat(alphaNumeric.sort(naturalSort)).concat(alphas.sort(naturalSort));
	return arr;
}


function set_options(tag, data, attribute) {
	select = $("." + tag + "_filter").find("select");
	existing_values = select.val();
	select.find("option").remove();
	options = data.map(function (d) { return d[attribute] });
	options = _.uniq(options);
	options = sortNatural(options);
	$(options).each(function (index, element) {
		option = $("<option>");
		option.text(element);
		option.attr("value", element);
		if (existing_values != null && existing_values.indexOf(element + "") > -1) {
			option.attr("selected", "selected");
		}
		option.appendTo(select);
	});
	//select.multiselect("rebuild");
	bindSumoSelect(select);
}

function bindSumoSelect(select) {
	if ($(select)[0] && $(select)[0].sumo) {
		$(select)[0].sumo.reload();
	} else {
		$(select).SumoSelect({
			search: true,
			placeholder: 'Search',
			forceCustomRendering: true
		});
	}
}

function set_raw_materials(data) {
	selectIncluded = $(".included_filter").find("select");
	selectExcluded = $(".excluded_filter").find("select");
	var existing_values = selectIncluded.val();
	selectIncluded.find("option").remove();
	var populateExcluded = (selectExcluded.find("option").length == 0)
	options = data.map(function (d) { return [d["Id"], d["MaterialCode"] + "-" + d["Description"]] });
	$(options).each(function (index, element) {
		option = $("<option>");
		option.text(element[1]);
		option.attr("value", element[0]);
		if (existing_values != null && existing_values.indexOf(element[0] + "") > -1) {
			option.attr("selected", "selected");
		}
		option.appendTo(selectIncluded);
		if (populateExcluded) {
			option.clone().appendTo(selectExcluded);
		}
	});
	//selectIncluded.multiselect("rebuild");
	bindSumoSelect(selectIncluded);
	if (populateExcluded) {
		//selectExcluded.multiselect("rebuild");
		bindSumoSelect(selectExcluded);
	}
}

function refreshMixFormulationSearch(data) {
	table_data = data["Formulations"];

	// If formulation_data is defined, but null, then update this value.
	// This is done to setup the table_data on first load.
	if (typeof (formulation_data) != "undefined" && formulation_data == null) {
		formulation_data = table_data;
	}

	set_options("air", table_data, "Air");
	set_options("psi", table_data, "PSI");
	set_options("slump", table_data, "Slump");
	set_options("md1", table_data, "MD1");
	set_options("md2", table_data, "MD2");
	set_options("md3", table_data, "MD3");
	set_options("md4", table_data, "MD4");
	set_options("ash", table_data, "AshPercentage");
	set_options("fine_agg", table_data, "FineAggPercentage");
	set_options("sacks", table_data, "Sacks");
	set_raw_materials(data["RawMaterials"]);

	existing = $("#search_mix_formulation_table").DataTable();
	if (existing != null) {
		existing.destroy(false);
		$("#search_mix_formulation_table").find("tbody").find("tr").remove();
	}
	$("#search_mix_formulation_table").DataTable({
		"sDom": sDom,
		"language": {
			"info": "_START_ to _END_ of _TOTAL_ records",
			"lengthMenu": "_MENU_ Records per page"
		},
		"data": table_data,
		"columns": [
			{ "data": "MixNumber" },
			{ "data": "SalesDesc" },
			{ "data": "AshPercentage" },
			{ "data": "FineAggPercentage" },
			{ "data": "Sacks" },
			{ "data": "Air" },
			{ "data": "Slump" },
			{ "data": "PSI" },
			{ "data": "MD1" },
			{ "data": "MD2" },
			{ "data": "MD3" },
			{ "data": "MD4" },
			{ "data": "Cost" },
			{ "data": "FormulationId" },
		],
		"columnDefs": [{
			"targets": 13,
			"data": "FormulationId",
			"render": function (data, type, full, meta) {
				return '<a data-id="' + data + '" class="btn btn-primary btn-xs push_selected_mix" title="Select Mix"><i class="fa fa-fw fa-plus"></i></a>';
			}
		}]
	});
	$("#search_mix_formulation_table").hide();
}