$(function () {
    $(document).on('click', '.reset_Password', function (e) {
        var item = $(this);
        item.attr("data-original-html", item.html());
        var userid = $("#UserId").val();
        var oldPassword = $(".form-group #oldPassword").val();
        var newPassword = $(".form-group #newPassword").val();
        var confirmPassword = $('.form-group #confirmPassword').val();
        var token = $("#Token").val();
        $.ajax({
            data: { oldPassword: oldPassword, newPassword: newPassword, confirmPassword: confirmPassword, u: userid, token: token },
            url: "/Login/ResetPassword",
            method: "POST",
            beforeSend: function () {
                item.html("<i class='fa fa-spinner fa-spin'></i> Processing...");
            },
            error: function () {
            },
            success: function (res) {
                if (res.statusList.length > 0) {
                    populateError(res);
                }
                else {
                    populateSuccess(res);
                    setTimeout(resetTextBox(), 5000);
                    var u = location.href.split('?token').splice(1);
                    if (u != null) {
                        window.location.replace('/Login/Index');
                    }
                    else {
                        window.location.reload();
                    }
                }
            },
            complete: function () {
                item.html(item.attr("data-original-html"));
            }
        });
    });

    function populateError(res) {
        var modal = modal = $("#password_reset_modal .modal-body .panel-body");
        var error = "";
        var isExistError = $('.alert.alert-danger');
        if (isExistError)
            isExistError.remove();
        for (var i = 0; i < res.statusList.length; i++) {
            error = "";
            error = error + "<div class='alert alert-danger shake'><span>" + res.statusList[i] + "</span></div>"
            modal.prepend(error);
        }
    }
    function populateSuccess(res) {
        var popupModal = $("#password_reset_modal");
        var modal = modal = $("#password_reset_modal .modal-body .panel-body");
        var sucess = "";
        var isExistError = $('#password_reset_modal .modal-body .panel-body .alert.alert-danger');
        if (isExistError)
            isExistError.remove();
        for (var i = 0; i < res.statusList.length; i++) {
            sucess = "";
            sucess = sucess + "<div class='alert alert-danger'><span>" + res.statusList[i] + "</span></div>"
            modal.prepend(sucess);
        }
    }
    function resetTextBox() {
        $("#oldPassword").val('');
        $("#newPassword").val('');
        $("#confirmPassword").val('');
        $('[data-dismiss]').trigger('click');
    }
});