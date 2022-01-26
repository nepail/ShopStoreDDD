var products = JSON.parse(localStorage['item'])

function ShowOrderDetail(itemid) {
    $("#" + itemid).slideToggle("slow");
}

function CancelOrder(ordernum) {
    
    swal({
        title: "確定取消嗎?",
        //text: "可能無法退款",
        type: "warning",
        showCancelButton: true,
        confirmButtonText: "確定",
        cancelButtonText: "取消",
        closeOnConfirm: false,
        showLoaderOnConfirm: true,
    },
        function (isConfirm) {
            if (isConfirm) { 
                setTimeout(function () {
                    $.ajax({
                        url: "/Order/CancelOrder/?ordernum=" + ordernum,
                        type: "get",                        
                        success: (res) => {
                            swal("取消", "編號 #" + ordernum + " 訂單已取消", "success");                            
                            var targetOrder = $(`#tb_${ordernum}`).find('span:eq(0)')
                            var targetClass = targetOrder.attr('class').split(' ')[2]
                            targetOrder.toggleClass(`${targetClass} bg-secondary`)
                            targetOrder.text('已取消')
                            $(`#order_${ordernum}`).find('input').parent().parent().remove()
                            EmptyOrder();
                        },
                        error: (res) => {
                            alert('fail')
                        }
                    });
                }, 1000);                
            }     
        });
}

function EmptyOrder() {
    if ($("#orderList tbody[id]").length == 0) {
        $('#orderList').append('<tbody><tr><td align="center" colspan="7">您的訂單目前是空的！</td></tr></tbody>');
    }    
}

function ShowProductDetail(itemname, itemid) {    
    $("#orderTable").toggle("normal");
    $("#prodetail").toggle("normal");
    if (itemid != undefined) {
        $("#prodetailConent").load("/Products/GetProductDetailById/?id=" + itemid);
/*        $("#prodetailConent").load("/Products/GetProductDetailById/?id=" + itemid);*/

    }
}