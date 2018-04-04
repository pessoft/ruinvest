$(document).ready(function () {
    $(".deposit").click(depositChoise);
    $("#reg-btn-ok").click(registrationUser);
    $("#log-btn-ok").click(loginUser);
    $("a[href='#user-panel']").click(scrollToUserPanel);
    autoScroll();
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
        showErrMessage(dataResult.ErrMessage)
    }
}

function showErrMessage(message) {
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

function scrollToUserPanel() {
    var timeDuration = 1000;
    var targetElement = $(this).attr('href');
    $('html ,body').animate({ scrollTop: $(targetElement).offset().top }, timeDuration);
}

function autoScroll() {
    $('html ,body').animate({ scrollTop: 10 }, 900);
}