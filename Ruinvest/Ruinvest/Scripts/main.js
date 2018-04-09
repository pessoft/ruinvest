$(document).ready(function () {
    $(".deposit").click(depositChoise);
    $("#reg-btn-ok").click(registrationUser);
    $("#log-btn-ok").click(loginUser);
    $("a[href='#user-panel']").click(scrollToUserPanel);
    $("#log-password").keypress((e) => {
        if (e.keyCode == 13) {
            loginUser();
        }
    });
    autoScroll();
});

var MessageType = {
    Success : "success",
    Info : "info",
    Warning : "warn",
    Error: "error"
}

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
        showInfoMessage(dataResult.ErrMessage, MessageType.Error)
    }
}

function showInfoMessage(message, type, elementId) {
    if (!elementId) {
        $.notify(
            message,
            {
                globalPosition: "bottom left",
                className: type,
                arrowShow: false
            });
    } else {
        $(`#${elementId}`).notify(
            message,
            {
                elementPosition: "bottom center",
                className: type,
                arrowShow: true
            });
    }
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

function addNewDeposit() {
    var data = {
        DepositAmount: $(".deposit.active - depost").attr("data-day").val(),
        Rate: $("#input-amount").val(),
    };
    
    $.post('/Home/CreateDeposit', data, successAddNewDeposit);
}

function successAddNewDeposit(data) {
    if (data.Success) {
        showInfoMessage("Депозит успешно добавлен", MessageType.Success)
    } else {
        showInfoMessage(dataResult.ErrMessage, MessageType.Error)
    }
}
