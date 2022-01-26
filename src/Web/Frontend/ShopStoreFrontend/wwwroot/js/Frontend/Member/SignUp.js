const progress = $("#progress")
const prev = $("#prev")
const next = $("#next")
const verify = $("#verify")
const circles = $(".circle")
const form = $("form")

let currentActive = 1
let register = false

$('label[required]').before('<span style="color:red">* </span>');


$("form").off('submit').submit(function (e) {
    //阻止元素默認發生的行為
    e.preventDefault();
    console.log('sign submit')
    $.ajax({
        async: true,
        type: $("form").attr("method"),
        url: $("form").attr("action"),
        data: $("form").serialize(),
        success: (res) => {

            if (res.success) {
                swal({
                    title: res.message,
                    text: "",
                    type: "success",
                    showLoaderOnConfirm: true,
                }, function (isConfirm) {
                    register = true;
                    console.log(register);
                    //跳轉到下一頁
                    $("#verify").hide();
                    next.click();
                });
            } else {
                swal(res.message, "", "error")
            }
        },
        error: (res) => {
            swal(res.message, "", "error")
            register = false;
        }
    });
});

//1
verify.click("click", () => {
    var result = form.valid()
    //$("form").valid();
    if (result) {
        console.log('true')
        $("form").submit();
    }    
})

next.click("click", () => {

    if (register != true) {
        alert('no')
        return;
    }

    currentActive++;

    if (currentActive > circles.length) {
        currentActive = circles.length;
    }

    update();

    if (currentActive == 2) {
        $("form").animate({ width: 'toggle' }, 350)
        $("#send_mail").animate({ width: 'toggle' }, 350)

        InitTimer(300, $("#timerVerify"), timerVerify);        
        next.attr("disabled", true);
    }

    if (currentActive == 3) {
        $("#send_mail").animate({ width: 'toggle' }, 350)
        $("#registerSuccess").animate({ width: 'toggle' }, 350)
        InitTimer(5, $("#timerRedirected"), timerRedirected);
    }
})

//2
//驗證使用者輸入的code
$("#verifyBtn").click("click", () => {

    $.ajax({
        type: $("form").attr("method"),
        url: "/Member/CheckEmailCode",
        data: $("form").serialize() + "&code=" + $("#verifyInput").val(),
        success: (res) => {
            if (res.success) {
                swal({
                    title: res.message,
                    text: "",
                    type: "success",
                    showLoaderOnConfirm: true,
                }, function (isConfirm) {
                    next.click();
                });
            } else {
                swal(res.message, "", "error")
            }
        },
        error: (res) => {
            swal(res.message, "", "error")
        }
    });



    //$.post("/Member/CheckEmailCode",
    //    {
    //        //memberName: $("#inputRealName").val(),
    //        //code: $("#verifyInput").val(),
    //        model: $("form").serialize()
    //    },
    //    (data) => {
    //        if (data.success) {
    //            //認證成功
    //            alert(data.message)
    //            //update();
    //            next.click();
    //        } else {
    //            switch (data.code) {
    //                case 0:
    //                    //驗證碼錯誤
    //                    alert(data.code + data.message)
    //                    break;
    //                case 2:
    //                    //認證碼過期
    //                    alert(data.code + data.message)
    //                    break;
    //            }
    //        }
    //    })   
})

prev.click("click", () => {
    currentActive--;
    if (currentActive < 1) {
        currentActive = 1;
    }
    update();

    if ($(".active").text() == '1') {
        $("form").slideLeftShow(500)
        $("#send_mail").slideLeftHide(500)
    }
})

function update() {
    circles.each(function (index, circle) {
        if (currentActive < index + 1) {
            $(this).removeClass("active")
        } else {
            $(this).addClass("active")
        }
    })

    if (currentActive <= 1) {
        prev.attr("disabled", true);
    } else if (currentActive >= circles.length) {
        next.attr("disabled", true);
    } else {
        next.attr("disabled", false);
        prev.attr("disabled", false);
    }

    progress.width((currentActive - 1) * 100 / (circles.length - 1) + '%')
}


/*timer*/
let countDown, timer;
let sec = 1;

//開始計時器
function InitTimer(c, obj, fun) {
    clearInterval(countDown)
    sec = c
    timer = obj
    countDown = setInterval(fun, 1000)
}

//信箱驗證用計時器
function timerVerify() {

    var min = Math.floor(sec / 60),
        remSec = sec % 60

    if (remSec < 10) {
        remSec = "0" + remSec
    }

    timer.text(min + ":" + remSec)

    if (sec > 0) {
        sec = sec - 1;
    } else {
        clearInterval(countDown)
        timer.text("over")
    }
}

//跳轉到登入頁面計時器
function timerRedirected() {

    var min = Math.floor(sec / 60),
        remSec = sec % 60

    timer.text(remSec)

    if (sec > 0) {
        sec = sec - 1;
    } else {
        clearInterval(countDown)
        window.location.replace("/Member/Login");
    }
}





jQuery.fn.slideLeftHide = function (speed, callback) {
    this.animate({
        width: "hide",
        paddingLeft: "hide",
        paddingRight: "hide",
        marginLeft: "hide",
        marginRight: "hide"
    }, speed, callback)
}

jQuery.fn.slideLeftShow = function (speed, callback) {
    this.animate({
        width: "show",
        paddingLeft: "show",
        paddingRight: "show",
        marginLeft: "show",
        marginRight: "show"
    }, speed, callback)
}


