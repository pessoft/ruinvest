
$(document).ready(function () {
    $(".deposit").click(depositChoise);
    $(".money-out").click(purseChoise);
    $("#reg-btn-ok").click(registrationUser);
    $("#log-btn-ok").click(loginUser);
    $("a[href='#user-panel']").click(scrollToUserPanel);
    $("#addNewDeposit").click(addNewDeposit);
    $("#addMoney").click(addMoney);
    $("#moneyOutOrder").click(moneyOutOrder);
    $("#log-password").keypress((e) => {
        if (e.keyCode == 13) {
            loginUser();
        }
    });
    autoScroll();
});

var AmountInterval = {
    Min: 50,
    Max: 50000
}

var MessageType = {
    Success : "success",
    Info : "info",
    Warning : "warn",
    Error: "error"
}

var MessageTemplate = {
    CreateDepositSuccess: "Депозит успешно добавлен",
    NotSelectedDeposit: "Выберите один из тарифов",
    NotEnoughMoney: "На вашем счете не достаточно средств",
    IncorrectedAmount: "Некорректная сумма",
    OrderMoneyOut: "Заявка на вывод средств поставлена в обработку"
}

function depositChoise() {
    $(".deposit").removeClass("active-item");
    $(this).addClass("active-item");
}

function purseChoise() {
    $(".money-out").removeClass("active-item");
    $(this).addClass("active-item");
}

function registrationUser() {
    var data = {
        FirstName: $("#reg-first-name").val(),
        SecondName: $("#reg-second-name").val(),
        PhoneNumber: $("#reg-phone-number").val(),
        Password: $("#reg-password").val()
    };

    $.post($(this).data('request-url'), data, successAccount);
}

function loginUser() {
    var data = {
        PhoneNumber: $("#log-phone-number").val(),
        Password: $("#log-password").val()
    };

    $.post($(this).data('request-url'), data, successAccount);
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
    let $depositActive = $(".deposit.active-item");
    let availableMoney = Number($("#availableMoney").attr("data-money"));
    let model = {
        Success : true,
        Message : ""
    };
    let depositAmount = Number($("#input-amount").val());

    if (depositAmount > availableMoney) {
        model.Success = false;
        model.Message = MessageTemplate.NotEnoughMoney
    }

    if ($depositActive.length == 0) {
        model.Success = false;
        model.Message = MessageTemplate.NotSelectedDeposit
    }

    if (model.Success) {
        var data = {
            DepositAmount: depositAmount,
            Rate: $depositActive.attr("data-day"),
        };

        $.post('/Home/CreateDeposit', data, successAddNewDeposit);
    } else {
        showInfoMessage(model.Message, MessageType.Info, "input-amount")
    }
}

function addMoney() {
    let amount = Number($("#input-moneyIn").val());

    if (amount < AmountInterval.Min || amount > AmountInterval.Max) {
        showInfoMessage(MessageTemplate.IncorrectedAmount, MessageType.Info, "input-moneyIn")
    } else {
        $.post('/Home/MoneyIn', { Amount: amount }, successAddMoney);
    }
}

function successAddMoney(dataResult) {
    if (dataResult.Success) {
        window.location.href = dataResult.Data;
    } else {
        showInfoMessage(dataResult.ErrMessage, MessageType.Error)
    }
}

function successAddNewDeposit(dataResult) {
    if (dataResult.Success) {
        $("#availableMoney").attr("data-money", dataResult.Data);
        $("#availableMoney #userMoney").html(dataResult.Data);
        showInfoMessage(MessageTemplate.CreateDepositSuccess, MessageType.Success)
    } else {
        showInfoMessage(dataResult.ErrMessage, MessageType.Error)
    }
}

function moneyOutOrder() {
    let model = {
        Success: true,
        Message: ""
    };

    let numberPurce = $("#input-purce").val(); 
    let amount = $(".money-in #input-amount").val();
    let availableMoney = Number($("#availableMoney").attr("data-money"));

    if (!isNaN(amount)) {
        let amoutF = parseFloat(amount);
        if (amoutF > availableMoney) {
            model.Success = false;
            model.Message = MessageTemplate.NotEnoughMoney;
        } else if (amoutF < AmountInterval.Min) {
            model.Success = false;
            model.Message = MessageTemplate.IncorrectedAmount;
        }
    } else {
        model.Success = false;
        model.Message = MessageTemplate.IncorrectedAmount;
    }

    if (model.Success) {
        $.post('/Home/MoneyOut', { NumberPurce: numberPurce, Amount: amount }, successMoneyOutOrder);
    } else {
        showInfoMessage(model.Message, MessageType.Error)
    }
}

function successMoneyOutOrder(dataResult) {
    if (dataResult.Success) {
        showInfoMessage(MessageTemplate.OrderMoneyOut, MessageType.Success)
        $("#availableMoney").attr("data-money", dataResult.Data)

        $(".block-order-create").fadeOut(1000, function () {
            $(".block-order-create").remove();
        });
    } else {
        showInfoMessage(dataResult.ErrMessage, MessageType.Error)
    }
}