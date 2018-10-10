$(function () {
    var actions = {
        "191": function () {
            $("#support_request_box").modal("show");
        }
    };
    var settings = {
        supportRequestBox: '#support_request_box',
        supportRequestForm: '#support_request_form',
        file: '#support_attachment_files',
        progress: '#support_attachment_progress',
        progressBar: '#support_attachment_progress .progress-bar'
    };

    try {
        $(settings.supportRequestBox + " #su_screen_resolution").val(screen.width + "x" + screen.height);
    } catch (e) { }

    $(document).on("keyup", function (e) {
        if (e.ctrlKey) {
            keyAction(e.which.toString());
        }
    }).on("click", ".gritter-close", function (e) {
        var g = $(this).parents("#gritter-notice-wrapper");
        g.remove();
    });
    $("#support_request_box").on("click", "[data-action='remove_support_attachment']", function (e) {
        var item = $(this);
        var a = $(item.parents(".su_attachment"));
        var id = $(a.find(".file_id"));
        $.ajax({
            url: '/Home/RemoveSupportAttachment',
            method: 'POST',
            data: { fileName: id.val() },
            beforeSend: function () {
                item.addClass("disabled");
                item.find("i.fa").addClass("fa-spin");
            },
            success: function (res) {

            },
            complete: function (res) {
                a.remove();
            }
        });
        e.preventDefault();
    });
    $(".fileupload").fileupload().on('fileuploadadd', function (e, data) {
        data.context = $('<div/>').appendTo(settings.file);
        $(settings.progress).addClass("in");
        $.each(data.files, function (index, file) {
            var node = $('<div class="su_attachment" />')
                    .append($('<span/>').text(file.name));
            node.appendTo(data.context);
        });
    }).on('fileuploadprocessalways', function (e, data) {
        var index = data.index,
            file = data.files[index],
            node = $(data.context.children()[index]);
        if (file.preview) {
            node
                .prepend('<br>')
                .prepend(file.preview);
        }
        if (file.error) {
            node
                .append('<br>')
                .append($('<span class="text-danger"/>').text(file.error));
        }
        if (index + 1 === data.files.length) {
            data.context.find('button')
                .text('Upload')
                .prop('disabled', !!data.files.error);
        }
    }).on('fileuploadprogressall', function (e, data) {
        var progress = parseInt(data.loaded / data.total * 100, 10);
        $(settings.progressBar).css(
            'width',
            progress + '%'
        );
    }).on('fileuploaddone', function (e, data) {
        $.each(data.result.files, function (index, file) {
            if (file.url) {
                var link = $('<a>')
                    .attr('target', '_blank')
                    .prop('href', file.url);
                var input = $("<input class='file_id' name='Attachments' type='hidden'/>").val(file.url);
                $(data.context.children()[index]).append(input).append($("<a href='javascript:void(0);' data-action='remove_support_attachment'><i class='fa fa-times text-danger remove-attachment'></i></a>"));
            } else if (file.error) {
                var error = $('<span class="text-danger"/>').text(file.error);
                $(data.context.children()[index])
                    .append('<br>')
                    .append(error);
            }
        });
        $(settings.progress).removeClass("in");
        $(settings.progressBar).css(
            'width',
            '0%'
        );
    }).on('fileuploadfail', function (e, data) {
        $.each(data.files, function (index, file) {
            var error = $('<span class="text-danger"/>').text('File upload failed.');
            $(data.context.children()[index])
                .append('<br>')
                .append(error);
        });
    }).prop('disabled', !$.support.fileInput)
        .parent().addClass($.support.fileInput ? undefined : 'disabled');

    $("*[data-action]").on("click", function () {
        var element = $(this);
        var action = $(this).attr("data-action");
        if (action === "submit_support_request") {
            if (validate($(settings.supportRequestForm).find("[required]")) && !$(settings.supportRequestForm).find(".has-error").length) {
                var form = $(settings.supportRequestForm);
                form.submit();
                setTimeout(function () {
                    form.find("input, select, textarea, button").attr("disabled", "disabled");
                    element.html("Submitting...").attr("disabled", "disabled");
                }, 500);
            }
        }
    });

    function validate(selectors) {
        var valid = true, errors = [];

        $.each(selectors, function (i, e) {
            var item = $(this);
            if (item.is("select") || item.is("input") || item.is("textarea")) {
                valid = validateItem(item);
                if (!valid) {
                    errors.push(item);
                } else {
                    errors.splice(errors.indexOf(item), 1);
                }
                item.on("change blur input", function () {
                    validateItem(item);
                });
            }
        });
        return valid && !errors.length;
    }
    function validateItem(item) {
        var valid = true;
        if (!item.val().trim().length) {
            valid = false;
            showValidationError(item);
        } else {
            var dataType = item.attr("data-type");
            if (!dataType) {
                removeValidationError(item);
            } else {
                dataType = dataType.toLowerCase();
                switch (dataType) {
                    case "email":
                        valid = validateEmail(item.val());
                        if (!valid) {
                            showValidationError(item, "Please enter a valid email");
                        } else {
                            removeValidationError(item);
                        }
                        break;
                }
            }
        }
        return valid;
    }
    function showValidationError(item, message) {
        var formGroup = item.parent();
        formGroup.addClass("has-error");
        var msg = message || item.attr("data-error-message");
        if (message) {
            msg = message;
        } else if (!msg.length) {
            msg = "This field is required";
        }
        if (!item.next(".help-block").length) {
            formGroup.append("<span class='help-block text-danger'>" + msg + "</span>");
        } else {
            var helpBlock = item.next(".help-block");
            helpBlock.html(msg);
            helpBlock.show();
        }
    }
    function removeValidationError(item) {
        var formGroup = item.parent();
        item.next(".help-block").remove();
        formGroup.removeClass("has-error");
    }
    function validateEmail(email) {
        var re = /\S+@\S+\.\S+/;
        return re.test(email);
    }

    function keyAction(key) {
        if (actions[key] && (typeof actions[key] == "function")) {
            actions[key]();
        }
    }
});