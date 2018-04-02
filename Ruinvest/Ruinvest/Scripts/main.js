$(document).ready(function () {
    $(".deposit").click(depositChoise);
    $("#reg-btn-ok").click(registrationUser);
    $("#log-btn-ok").click(loginUser);
});

function depositChoise() {
    $(".deposit").removeClass("active-depost");
    $(this).addClass("active-depost");
}

function registrationUser() {
    var data = {
        FirstName: $("#reg-first-name").val(),
        SecondName: $("#reg-second-name").val(),
        PhoneNumber: $("#reg-phone-number").val(),
        Password: $("#reg-password").val()
    };

    $.post('/Home/Registration', data, successAccount);
}

function loginUser() {
    var data = {
        PhoneNumber: $("#log-phone-number").val(),
        Password: $("#log-password").val()
    };

    $.post('/Home/Login', data, successAccount);
}

function successAccount(dataResult) {
    if (dataResult.Success) {
        document.location.href = '/Home/Deposits';
    } else {
        ShowErrMessage(dataResult.ErrMessage)
    }
}

function ShowErrMessage(message) {
    $.notify(
        message,
        {
            globalPosition: "bottom left",
            className: "error",
            arrowShow: false
        });
}

(function ($) {
    $.fn.removeClassWild = function (mask) {
        return this.removeClass(function (index, cls) {
            var re = mask.replace(/\*/g, '\\S+');
            return (cls.match(new RegExp('\\b' + re + '', 'g')) || []).join(' ');
        });
    };
})(jQuery);