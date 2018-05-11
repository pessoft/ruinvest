
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

    $(window).trigger("load")//финт жопой
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
    OrderMoneyOut: "Заявка на вывод средств поставлена в обработку",
    NotFullDataRegistration: "Указаны не все данные для регистрации",
    NotFullDataLogin: "Указаны не все данные для входа",
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

    if (!data.FirstName || !date.SecondName
        || !date.PhoneNumber || !date.Password) {
        showInfoMessage(MessageTemplate.NotFullDataRegistration, MessageType.Error)

        return;
    }

    let loader = new Loader($("#reg-btn-ok"));
    loader.ToggleLoader()
    $.post($(this).data('request-url'), data, successCallBack(successAccount, loader));
}

function successCallBack(func, loader) {
    return function (data) {
        func(data, loader);
    }
}

function loginUser() {
    var data = {
        PhoneNumber: $("#log-phone-number").val(),
        Password: $("#log-password").val()
    };

    if (!data.PhoneNumber || !data.Password) {
        showInfoMessage(MessageTemplate.NotFullDataLogin, MessageType.Error);

        return;
    }

    let loader = new Loader($("#log-btn-ok"));
    loader.ToggleLoader()
    $.post($("#log-btn-ok").data('request-url'), data, successCallBack(successAccount, loader));
}

function successAccount(dataResult, loader) {
    loader.ToggleLoader();

    if (dataResult.Success) {
        window.location.replace(dataResult.Data);
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
        let loader = new Loader("#addNewDeposit");
        loader.ToggleLoader();

        $.post('/Home/CreateDeposit', data, successCallBack(successAddNewDeposit, loader));
    } else {
        showInfoMessage(model.Message, MessageType.Info)
    }
}

function addMoney() {
    let amount = Number($("#input-moneyIn").val());

    if (amount < AmountInterval.Min || amount > AmountInterval.Max) {
        showInfoMessage(MessageTemplate.IncorrectedAmount, MessageType.Info, "input-moneyIn")
    } else {
        let loader = new Loader("#addNewDeposit");
        loader.ToggleLoader();

        $.post('/Home/MoneyIn', { Amount: amount }, successCallBack(successAddMoney, loader));
    }
}

function successAddMoney(dataResult, loader) {
    loader.ToggleLoader();

    if (dataResult.Success) {
        window.location.replace(dataResult.Data);
    } else {
        showInfoMessage(dataResult.ErrMessage, MessageType.Error)
    }
}

function successAddNewDeposit(dataResult, loader) {
    loader.ToggleLoader();

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
        let loader = new Loader("#moneyOutOrder");
        loader.ToggleLoader();

        $.post('/Home/MoneyOut', { NumberPurce: numberPurce, Amount: amount }, successCallBack(successMoneyOutOrder, loader));
    } else {
        showInfoMessage(model.Message, MessageType.Error)
    }
}

function successMoneyOutOrder(dataResult, loader) {
    loader.ToggleLoader();

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

class Loader {
    constructor(element) {
        this.element = element;
        this.toggle = false;
        this.htmlContent = null;
        this.emptyContent = "<div class='loader'>&nbsp;</div>";
    }

    ToggleLoader() {
        if (!this.toggle) {
            this.toggle = true;
            this.htmlContent = $(this.element).html();
            $(this.element).html(this.emptyContent);
        } else {
            this.toggle = false;
            $(this.element).html(this.htmlContent);
            this.htmlContent = null;
        }
    }
}