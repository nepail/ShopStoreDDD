$(document).ready(function () {
    Layout.Init();
    
})

var test;

var Layout = {
    //初始化
    Init() {
        Layout.UC.Init();
        Layout.SW.Reg();
    },
    
    DATA: {
        //檢查未讀訊息
        CheckUserAlert() {
            $.ajax({
                url: '/Member/CheckUserAlert',
                success: res => {
                    if (res.success) {                        
                        Layout.UC.RenderUserAlert(JSON.parse(res.item));
                    }
                },
                error: res => {
                    swal('CheckUserAlert Error', 'Network Error', 'error')
                }
            })
        }
    },

    UC: {
        //初始化
        Init() {
            Layout.UC.Setting.Slick();
            Layout.UC.Setting.Toastr();
            Layout.UC.SetBtn();
            Layout.UC.On.Unload();
        },

        //渲染使用者通知div
        RenderUserAlert(item) {            
            var content = '';

            for (var i = 0, len = item.length; i < len; i++) {
                content += `
                     <div class="notify-content">
                        <div class="notify-text">
                            <h1>${item[i].AlertTime}</h1>
                            <p>${item[i].OrderId} : ${item[i].StateMsg}</p>
                        </div>
                    </div>`
            }

            $('#notification').html(content);
            $('#blip').css('opacity', 1);
        },
       
        On: {
            Unload() {
                $(window).on('unload', function (e) {
                    e.preventDefalue();
                    StopServerHub();
                })
            }
        },

        SetBtn() {
            //User 鈴鐺通知
            $('#btnNotify').click(function () {
                var $notification = $('#notification');
                var $notifyText = $('.notify-text');

                $('#blip').css('opacity', 0);

                $notification.toggleClass('open');
                $notifyText.toggleClass('show');
            })
        },

        Setting: {
            Slick() {
                $('.single-item').slick({
                    dots: true,
                    fade: true,
                    arrows: true,
                    autoplay: true,
                    autoplaySpeed: 5000,
                });

                //card-img-item
                $('.card-img-item').slick({
                    //dots: true,
                    //fade: true,
                    //arrows: true,
                    //autoplay: true,
                    //autoplaySpeed: 5000,
                    //centerMode: true,
                    //centerPadding: '10px',
                    lazyLoad: 'ondemand',
                    slidesToShow: 3,
                    slidesToScroll: 1
                });
            },
            Toastr() {
                toastr.options = {
                    // 參數設定[註1]
                    "positionClass": "toast-top-right",
                    "closeButton": false, // 顯示關閉按鈕
                    "debug": false, // 除錯
                    "newestOnTop": false,  // 最新一筆顯示在最上面
                    "progressBar": true, // 顯示隱藏時間進度條
                    "preventDuplicates": false, // 隱藏重覆訊息
                    "onclick": null, // 當點選提示訊息時，則執行此函式
                    "showDuration": "300", // 顯示時間(單位: 毫秒)
                    "hideDuration": "1000", // 隱藏時間(單位: 毫秒)
                    "timeOut": "5000", // 當超過此設定時間時，則隱藏提示訊息(單位: 毫秒)
                    "extendedTimeOut": "1000", // 當使用者觸碰到提示訊息時，離開後超過此設定時間則隱藏提示訊息(單位: 毫秒)
                    "showEasing": "swing", // 顯示動畫時間曲線
                    "hideEasing": "linear", // 隱藏動畫時間曲線
                    "showMethod": "fadeIn", // 顯示動畫效果
                    "hideMethod": "fadeOut" // 隱藏動畫效果
                }

            }
        }

    },

    //ServiceWorker
    SW: {
        //註冊
        Reg() {
            //檢查是否註冊過ServiceWorker
            navigator.serviceWorker.getRegistrations().then(registrations => {
                
                if (registrations.length > 0) {
                    //已註冊過SW    
                } else {
                    //註冊ServiceWorker
                    RegistrationServiceWorker();
                }

            });

            //註冊ServiceWorker
            function RegistrationServiceWorker() {

                if ('serviceWorker' in navigator) {
                    navigator.serviceWorker
                        .register('/serviceWorker.js')//第二個參數可代入想要註冊的範圍
                        .then(function (reg) {
                            
                        })
                        .catch(function (error) {
                            console.log('註冊失敗： ' + error); //註冊失敗
                        })
                }
            }
        },

        //操作
        Remote: {
            //發送命令
            RemoteServiceWorker(context, method) {

                if (navigator.serviceWorker.controller) {

                    //建立 MessageChannel
                    var messageChannel = new MessageChannel();

                    //Port1 監聽來自SW的訊息
                    messageChannel.port1.onmessage = function (event) {

                        if (event.data.type == 'UserAlert') {
                            toastr.success(event.data.stateMsg, `您的訂單編號(#${event.data.orderId})`);
                        }

                    }

                    //發送訊息至SW，同時挾帶 port2 使SW可以回傳訊息
                    navigator.serviceWorker.controller.postMessage({
                        'command': method,
                        'message': context,
                        'url': window.location.origin + '/ServerHub'
                    }, [messageChannel.port2]);
                }
            },

            //發送斷開長連接命令
            StopServerHub() {
                if (navigator.serviceWorker.controller) {
                    navigator.serviceWorker.controller.postMessage({
                        'command': 'StopServerHub'
                    })
                }
            }
        }
    }
}


