$(document).ready(function() {
    Order.GetOrderList();
    Order.GetOrderStatus();
    Order.SetBtnSave();
    Order.SetDropDownList();
})

var postData = [];
var postQueue = [];

var Order = {
    /**
     * 取得所有Order資料
     */
    GetOrderList: function() {
        $.ajax({
            url: '/Manager/GetOrderList',
            type: 'get',
            success: function(res) {
                if (res.success) {
                    MainProperties.Order.data = res.result;
                    Order.InitOrderList();
                } else {
                    swal('系統錯誤', '資料庫錯誤', 'error');
                }
            },
            error: function() {
                swal('網路錯誤', '無法連上伺服器', 'error');
            }
        })
    },


    GetOrderStatus: function () {
        $.ajax({
            url: '/Manager/GetOrderStatus',
            type: 'get',
            success: function (res) {
                if (res.success) {
                    MainProperties.Order.OrderStatus.cartgoState = res.result.cartgoState;
                    MainProperties.Order.OrderStatus.sipState = res.result.sipState;
                } else {
                    swal('系統錯誤', '資料庫錯誤', 'error');
                }
            },
            error: function () {
                swal('網路錯誤', '無法連上伺服器', 'error');
            }
        })
    },

    Return: function(item) {
        Order.ReturnOfgood($(item).parent().siblings('.bcol:eq(1)').text());
    },
    /**
     * 渲染Order列表
     */
    InitOrderList: function () {
        
        var odContent = `
            <!--Title部分-->
            <div class="box brow boxtitle">
                <div class="box bcol rowckbox">
                    <input type="checkbox" class="visibility" />
                </div>
                <div class="box bcol">訂單編號</div>
                <div class="box bcol">日期</div>
                <div class="box bcol">會員帳號</div>
                <div class="box bcol">狀態</div>
                <div class="box bcol">運送方式</div>
                <div class="box bcol">總金額</div>
                <div class="box bcol rowckbox">
                    <input type="button" class="btn btn-sm btn-outline-danger" value="退貨" style="visibility: hidden" />
                    <i class="bx bx-search"></i>
                    <input id="searchInput" type="search" placeholder="Search..." />
                </div>
            </div>`;
        

        var cartgoState = '';

        for(keys in MainProperties.Order.OrderStatus.cartgoState){
            cartgoState += `<li data-type="${keys}">${MainProperties.Order.OrderStatus.cartgoState[keys].name}</li>`;
        }

        var sipState = '';
        for (keys in MainProperties.Order.OrderStatus.sipState) {
            sipState += `<li data-type="${keys}">${MainProperties.Order.OrderStatus.sipState[keys].name}</li>`;
        }

        for (var i = 0, odLength = MainProperties.Order.data.length; i < odLength; i++) {

            var btnSwitch = MainProperties.Order.data[i].isDel == 1 ? 'style="visibility: hidden;"' : '';
            odContent += `
                    <div id="ordernum_${MainProperties.Order.data[i].num}" class="box brow order">
                        <div class="box bcol rowckbox">
                            <input type="checkbox" class="visibility"/>
                        </div>
                        <div class="box bcol tx"><span ordernum="${i}" >${MainProperties.Order.data[i].num}</span></div>
                        <div class="box bcol tx"><span>${MainProperties.Order.data[i].date}</span></div>
                        <div class="box bcol tx"><span>${MainProperties.Order.data[i].memberAccount}</span></div>
                        <div class="box bcol">
                            <div class="size">
                                <span class="bbadge field bg-${MainProperties.Order.data[i].statusBadge}">${MainProperties.Order.data[i].status}</span>
                                    <ul menu-type="0" class="list">
                                        ${cartgoState}
                                    </ul>
                            </div>
                        </div>
                        <div class="box bcol">
                            <div class="size">
                                <span class="bbadge field ${MainProperties.Order.data[i].shippingBadge}">${MainProperties.Order.data[i].shippingMethod}</span>
                                <ul menu-type="1" class="list">
                                    ${sipState}
                                </ul>
                            </div>
                        </div>
                        <div class="box bcol tx"><span>NT$ ${MainProperties.Order.data[i].total}</span></div>
                        <div class="box bcol rowckbox">
                            <input id="btnReturn" type="button" class="btn btn-sm btn-outline-danger" value="退貨" onclick="Order.Return(this)" ${btnSwitch}/>
                        </div>
                    </div>`;
        }

        $('#orderList').html(odContent);
        Order.SetSearch();
    },

    /**
     * 退貨功能
     * @param {Number} ordernum 
     */
    ReturnOfgood: function(ordernum) {
        swal({
                title: '確定執行此操作?',
                type: 'warning',
                showCancelButton: true,
                confirmButtonText: '確定',
                cancelButtonText: '取消',
                closeOnConfirm: false,
                showLoaderOnConfirm: true,
            },
            function(isConfirm) {
                if (isConfirm) {
                    setTimeout(function() {
                        $.ajax({
                            url: '/Manager/RemoveOrder?id=' + ordernum,
                            type: 'get',
                            success: (res) => {
                                if (res.success) {
                                    swal('執行成功', '訂單' + ordernum + '已退貨', 'success');

                                    var targetItem = $('#ordernum_' + ordernum).find('.bcol:eq(4) span');
                                    var targetClass = $('#ordernum_' + ordernum).find('.bcol:eq(4) span').attr('class').split(' ')[2];
                                    targetItem.toggleClass(`${targetClass} bg-danger`);
                                    targetItem.text('已退貨');
                                    $('#ordernum_' + ordernum).find('input[type="button"]').css('visibility', 'hidden');
                                } else {
                                    swal('執行失敗', '資料庫錯誤', 'error');
                                }
                            },
                            error: (res) => {
                                swal('執行失敗', '伺服器錯誤', 'error');
                            }
                        })
                    })
                }
            }
        )
    },

    /**
     * 狀態變更功能
     * @param {Number} statusCode f_status 的狀態
     * @param {Number} type Menu 的類型
     * @param {Number} ordernum f_id
     * @param {Number} orderid 
     * @returns 狀態的的 CSS Style
     */
    GetStatus: function(statusCode, type, ordernum, orderid, orderUser) {

        if (postData[ordernum] == undefined) {
            postData[ordernum] = {
                account:'',
                id: '',
                status: 0,
                ShippingMethod: 0
            }
        }

        //消息佇列
        if (postQueue[ordernum] == undefined) {
            postQueue[ordernum] = {
                orderUser: '',
                orderId: 0,
                statMsg: ''
            }
        }


        if (type == 0) {

            postData[ordernum].account = orderUser;
            postData[ordernum].id = orderid;
            postData[ordernum].status = parseInt(statusCode.slice(-1));

            postQueue[ordernum].orderUser = orderUser;
            postQueue[ordernum].orderId = orderid;
            postQueue[ordernum].statMsg = `訂單狀態已變更為${MainProperties.Order.OrderStatus.cartgoState[statusCode].name}`;

            
            //Home.CONNECTION.AlertFrontedUser(orderUser, orderid, `訂單狀態已變更為${statusCode}`)

            return MainProperties.Order.OrderStatus.cartgoState[statusCode].style;
        }

        if (type == 1) {

            postData[ordernum].account = orderUser;
            postData[ordernum].id = orderid;
            postData[ordernum].ShippingMethod = parseInt(statusCode.slice(-1));

            postQueue[ordernum].orderUser = orderUser;
            postQueue[ordernum].orderId = orderid;
            postQueue[ordernum].statMsg = `訂單運送方式已變更為${MainProperties.Order.OrderStatus.sipState[statusCode].name}`;

            //Home.CONNECTION.AlertFrontedUser(orderUser, orderid, `配送狀態已變更為${statusCode}`)

            return MainProperties.Order.OrderStatus.sipState[statusCode].style;
        }
    },

    /**
     * 搜尋功能
     */
    SetSearch: function() {
        $('#searchInput').on('keyup', function() {
            var value = $(this).val().toLowerCase();
            $('#orderList>.order').filter(function() {                
                $(this).toggle($(this).find('.tx, .field').text().toLowerCase().indexOf(value) > -1);
            })
        })
    },

    /**
     * 保存功能
     */
    SetBtnSave: function() {
        $('#btnSave').click(function() {
            Order.SendData();
        })
    },

    /**
     * 更新訂單資料
     * @returns 成功與否
     */
    SendData: function() {

        if (postData.length == 0) {
            swal('沒有要變更的資料，請先選擇', ' ', 'info');
            return;
        }

        postData = postData.filter(el => el);
        postQueue = postQueue.filter(el => el);

        var data = {            
            orders: postData,
            postQueues: postQueue
        }

        console.table(postData);
        //console.table(postQueue);

        $.ajax({
            url: '/Manager/UpdateOrder',
            type: 'post',
            data: data,
            success: (res) => {
                if (res.success) {
                    swal('保存成功', ' ', 'success');

                    console.table(postQueue)

                    for (var i = 0, len = postQueue.length; i < len; i++) {
                        Home.CONNECTION.AlertFrontedUser(postQueue[i].orderUser, postQueue[i].orderId, postQueue[i].statMsg)
                    }

                    postData = [];
                    postQueue = [];
                }
            },
            error: (res) => {
                swal('保存失敗', '網路異常', 'error');
            }
        })
    },

    /**
     * 狀態清單下拉
     */
    SetDropDownList: function() {
        $(document).off('click').on('click', '.size', function() {
            $('.size').styleddropdown();
        })
    }
}

$.fn.styleddropdown = function() {
    return this.each(function() {
        var obj = $(this).off('click');
        obj.find('.field').off('click').click(function() {
            obj.find('.list').fadeIn(400);

            $(document).off('keyup').keyup(function(event) {
                if (event.keyCode == 27) {
                    obj.find('.list').fadeOut(400);
                }
            });

            obj.find('.list').hover(function() {},
                function() {
                    $(this).fadeOut(400);
                });
        });

        obj.find('.list li').off('click').click(function() {

            //var type = $(this).attr('data-type').slice(-1);            
            var type = $(this).attr('data-type');
            var menuType = $(this).parent().attr('menu-type');
            var orderid = obj.find('.field').parent().parent().siblings().eq(1).find('span').text();            
            var orderUser = obj.find('.field').parent().parent().siblings().eq(3).find('span').text();            
            var ordernum = obj.find('.field').parent().parent().siblings().eq(1).find('span').attr('ordernum');
            var targetClass = obj.find('.field').attr('class').split(' ')[2];
            obj.find('.field')
                .text($(this).html())
                .toggleClass(`${targetClass} ${Order.GetStatus(type, menuType, ordernum, orderid, orderUser)}`);            

            obj.find('.list').fadeOut(400);
        });
    });
};