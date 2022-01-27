$(document).ready(function () {
    Home.InitPage();

    $(window).on("beforeunload", function () {        
        Home.CONNECTION.RemoveGroup();
    })
})

var connection = new signalR.HubConnectionBuilder().withUrl('http://localhost:5000/chatHub', { accessTokenFactory: () => localStorage.getItem('token') }).build();
var serverHub = new signalR.HubConnectionBuilder().withUrl('http://localhost:5000/ServerHub', { accessTokenFactory: () => localStorage.getItem('token') }).build();

const chatButton = $('.chatbox__button');
const chatContent = $('.chatbox__support');
const icons = {
    isClicked: '<img src="~img/svg/chatbox-icon.svg" />',
    isNotClicked: '<img src="~img/svg/chatbox-icon.svg" />'
}

const chatbox = new InteractiveChatbox(chatButton, chatContent, icons);
chatbox.display();

var Home = {

    InitPage() {
        //側邊欄
        Home.UC.SideMenu.SetBtn.Sidebar();

        //聊天室初始化
        Home.UC.ChatRoom.Init();        
        Home.CONNECTION.Init();

    },

    DATA: {
        //計算百分比
        GetPercent(num, total) {
            num = parseFloat(num);
            total = parseFloat(total);
            if (isNaN(num) || isNaN(total)) {
                return "-";
            }
            return total <= 0 ? '0' : (Math.round(num / total * 10000) / 100.00);
        }
    },

    UC: {


        SideMenu: {
            //按鈕繫結事件
            SetBtn: {
                Sidebar() {
                    $('.sidebar-button').click(function () {
                        $('.sidebar').toggleClass('active');
                        $('.sub-menu').css('display', 'none');
                    });

                    $('.icon-link').find('a').click(function (e) {
                        e.preventDefault();
                        $(this).parent('.icon-link').siblings('.sub-menu').slideToggle();
                    });

                    $('.sub-menu').find('a').click(function (e) {
                        e.preventDefault;

                        if ($(this).attr('data-controller').includes('Logout')) {
                            window.location.href = '/Manager';
                        }

                        if ($(this).attr('data-controller') != '') {

                            $.ajax({
                                url: $(this).attr('data-controller'),
                                success: res => {
                                    if (res.includes('後台管理系統')) {
                                        window.location.href = '/Manager';
                                    }
                                    $('#app').html(res)
                                },
                                error: res => {
                                    window.location.href = '/Manager';
                                }
                            })
                        }
                    });

                    //首頁按鈕
                    $('#btnHome').click(function () {
                        $('#app').html(`
                        <div id="Home-container" class="Home-container">
                            <div id="loading-section" class="loading">                                
                                <span class="loading__anim"></span>
                            </div>
                        </div>`)
                    });
                },
            },
        },


        ChatRoom: {
            
            //聊天室初始化
            Init() {                                
                Home.UC.ChatRoom.UC.SetName();
                Home.UC.ChatRoom.UC.SetBtn.UserListClick();
                Home.UC.SetChatRoom.SetBtnUserListClick();
            },

            UC: {
                //設定名字
                SetName() {
                    var userName = localStorage.getItem('user');

                    $('#chat-box-user-name').text(userName);
                    $('#chat-box-user-firstName').text(userName[0])
                    $('#chat-username').text(userName);
                },

                //綁定按鈕事件
                SetBtn: {
                    UserListClick(ConList) {                        
                        if (ConList == undefined) return;

                        var userName = localStorage.getItem('user');

                        //在列表移除使用者自己
                        var index = ConList.findIndex(x => x.userName == userName);
                        ConList.splice(index, 1);

                        var userListHtml = '';

                        //console.table(ConList);

                        for (var i = 0, len = ConList.length; i < len; i++) {
                            userListHtml +=
                                `      <a href="#" data-name="${ConList[i].userName}" onclick="Home.UC.SetChatRoom.SwitchChat('${ConList[i].connectionID}')">
                                    <div class="content">
                                        <div class="user-img">
                                            <span>${ConList[i].userName[0]}</span>
                                        </div>
                                        <div class="details">
                                            <span>${ConList[i].userName}</span>
                                            <p class="user-incoming-msg"></p>
                                        </div>
                                    </div>
                                    <div class="status-dot"><i class="fas fa-circle"></i></div>
                                </a>`
                        }

                        $('#users-list').html(userListHtml);
                    }




                }
            }
        },

        SetChatRoom: {
            //點選使用者列表進行傳訊
            SetBtnUserListClick() {

                //上一頁按鈕
                $('#prevToChatList').click(function (e) {
                    e.preventDefault();                    

                    $('#main-chat').toggle();
                    $('#sub-chat').toggle();

                    var $div = $('#chat-box');
                    $div.scrollTop($div[0].scrollHeight);
                });

                //傳送訊息按鈕
                $('#btnSendToChat').click(function (e) {
                    e.preventDefault();
                    var $chatBox = $('#chat-box');
                    var $btnSend = $(this);
                    var textContent = $btnSend.siblings('input').val();

                    if (textContent == '') return;

                    $btnSend.siblings('input').val('');

                    $chatBox.append(
                        `      <div class="chat outgoing">
                                    <div class="details">
                                        <p>${textContent}</p>
                                    </div>
                                </div>`)

                    $chatBox.animate({ scrollTop: $chatBox.prop("scrollHeight") }, 500);
                    $btnSend.siblings('input').focus();
                                        
                    //'data-name' => groupName
                    Home.CONNECTION.SendGroupMsg($(this).attr('data-name'), textContent);
                })
            },

            //點擊user進入對話框
            SwitchChat(targetId) {                

                $('#main-chat').toggle();
                $('#sub-chat').toggle();                

                var $div = $('#chat-box');
                $div.scrollTop($div[0].scrollHeight);
                
                console.log('我的ID')
                console.log(connection.connectionId)

                Home.CONNECTION.CreateGroup(connection.connectionId, targetId);
            }
        }
    },

    CONNECTION: {
        Init() {           
            connection.start().then(function () {
                console.log('--- connection start ---');
            }).catch(function (err) {
                console.error('連線失敗');
                $('#loading-section').html('<span>讀取資料出現異常，請聯繫網站管理員</span>');
                return console.error(err.toString());
            });

            serverHub.start().then(function () {
                console.log('--- Server Connection Start ---');
            }).catch(function (err) {
                connsole.error('伺服器連線失敗')
            })

            //test
            connection.on('ReceiveMessageFromUser', function (user, message) {
                console.log(user + message);                
            })

            //庫存量警告
            serverHub.on('ReceiveMessage', function (user, message) {

                if ($('#Home-container').length > 0) {
                    var item = JSON.parse(message);

                    var itemHtml = '';

                    for (var keys in item) {
                        var percent = Home.DATA.GetPercent(item[keys].Stock, 20);
                        itemHtml += `
                                <div class="Home-item">
                                    <div class="item-textAlert">
                                        <span>庫存量不足警告</span>
                                    </div>
                                    <div class="item-content">
                                        <div class="item-section item-detail">
                                            <span>產品名稱</span>
                                            <span>${item[keys].Name}</span>
                                            <span>庫存量</span>
                                            <span>${item[keys].Stock}</span>
                                            <span>最低庫存量:20</span>
                                        </div>

                                        <div class="item-section">
                                            <div class="flex-wrapper">
                                                <div class="single-chart">
                                                    <svg viewBox="0 0 36 36" class="circular-chat orange">
                                                        <path class="circle-bg" d="M18 2.0845
                                                                      a 15.9155 15.9155 0 0 1 0 31.831
                                                                      a 15.9155 15.9155 0 0 1 0 -31.831"/>

                                                        <path class="circle" stroke-dasharray="${percent}, 100" d="M18 2.0845
                                                                      a 15.9155 15.9155 0 0 1 0 31.831
                                                                      a 15.9155 15.9155 0 0 1 0 -31.831"/>

                                                        <text x="18" y="20.35" class="percentage">${percent}%</text>
                                                    </svg>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>`;
                    }

                    $('#Home-container').html(itemHtml);
                }
            });

            //接收User回傳的訊息
            connection.on('ReceivePrivateFromUser', function (userNameFrom, userName, msg) {

                if (!$('#chatbox-support').hasClass('chatbox--active')) {
                    //視窗關閉狀態
                    $('#chatbox-support').addClass('chatbox--active');
                    $('#main-chat').toggle();
                    $('#sub-chat').toggle();
                    $userTarget.find('.user-incoming-msg').text(msg);
                }
                else
                {
                    //視窗開啟狀態                    
                    var $userTarget = $('#users-list>a').filter(function () {
                        return $(this).data('name') == userNameFrom
                    });                    

                    $userTarget.find('.user-incoming-msg').text(msg);
                }
                
                $('#btnSendToChat').attr('data-name', userName);
                
                $('#chat-box').append(`
                                <div class="chat incoming">
                                    <div class="user-img"></div>
                                    <div class="details">
                                        <p>${msg}</p>
                                    </div>
                                </div>`)

                var $div = $('#chat-box');
                $div.scrollTop($div[0].scrollHeight);
            })

            //接收伺服器回傳的在線列表
            connection.on('GetConList', function (ConList) {                
                console.table(ConList);

                if (ConList != undefined) {
                    Home.UC.ChatRoom.UC.SetBtn.UserListClick(ConList);
                }
            })

            connection.onclose(function (e) {
                console.log('連線已中斷');
            });

            //3. 接受群組回傳訊息
            connection.on('GetGroupMsg', function (groupName, userName, msg) {
                console.log({ msg })

                $('#btnSendToChat').attr('data-name', groupName);
                localStorage.setItem('connectedGroup', groupName);

                if (!$('#chatbox-support').hasClass('chatbox--active')) {
                    //視窗關閉狀態
                    $('#chatbox-support').addClass('chatbox--active');
                    $('#main-chat').toggle();
                    $('#sub-chat').toggle();

                    $('#chat-box').append(`
                                <div class="chat incoming">
                                    <div class="user-img"></div>
                                    <div class="details">
                                        <p>${msg}</p>
                                    </div>
                                </div>`)

                    var $div = $('#chat-box');
                    $div.scrollTop($div[0].scrollHeight);

                    $userTarget.find('.user-incoming-msg').text(msg);
                }
                else {
                    //視窗開啟狀態

                    var $userTarget = $('#users-list>a').filter(function () {
                        return $(this).data('name') == userName
                    });                    
                    
                    $userTarget.find('.user-incoming-msg').text(msg);
                }
                
                $('#chat-box').append(`
                                <div class="chat incoming">
                                    <div class="user-img"></div>
                                    <div class="details">
                                        <p>${msg}</p>
                                    </div>
                                </div>`)

                var $div = $('#chat-box');
                $div.scrollTop($div[0].scrollHeight);
            })
        },

        //1. 發起對話建立群組
        CreateGroup(userNameFrom, userNameTo) {

            groupName = this.uuid4();

            connection.invoke('CreateGroup', userNameFrom, userNameTo, groupName).catch(() => {
                console.error('建立對話時出現失敗')
            }).then((res) => {
                console.log({ res });
                //local端 設定GroupName
                localStorage.setItem('connectedGroup', res);                
            })            
        },

        uuid4() {
            return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
                (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
            );
        },

        //2. 傳送對話至群組
        SendGroupMsg(groupName, msg) {
            //取得已建立的群組名稱
            //groupName = localStorage.getItem('connectedGroup');
            userName = localStorage.getItem('user');
            console.log({ groupName });
            if (groupName == undefined) groupName = localStorage.getItem('connectedGroup');

            connection.invoke('SendToGroup',groupName, userName, msg).catch(() => {
                //群組對話失敗，連線消失?
                console.error('SendToGroup 1')
            })
        },

        RemoveGroup() {
            connection.invoke('RemoveGroup', localStorage.getItem('connectedGroup')).catch(() => {
                console.error('刪除群組時發生失敗')
            })
        },

        //通知前台用戶訂單狀態變更
        AlertFrontedUser(orderUser, orderId, stateMsg) {
            serverHub.invoke('SendMessageToFrontedUser', orderUser, orderId, stateMsg).catch(() => {
                console.error('通知變更時發生錯誤')
            })
        }
    }
}
