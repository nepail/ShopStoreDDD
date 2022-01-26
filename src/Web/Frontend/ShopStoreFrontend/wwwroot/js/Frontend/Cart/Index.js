$(document).ready(function () {
    Cart.InitPage();
});


var products = JSON.parse(localStorage.getItem('item'));

//window.onload = function () {
//    total();
//    $(".txtSubTotal").bind('DOMNodeInserted', function (e) {
//        total();
//    })
//};


var Cart = {
    InitPage() {
        this.UC.Total();
        $('.txtSubTotal').bind('DOMNodeInserted', function (e) {
            Cart.UC.Total();
        });
    },

    DATA: {

    },

    UC: {
        Total() {
            var total = 0;
            $('.txtSubTotal').each(function () {
                total += parseInt($(this).text());
                $('#cartTotal').text(total);
            });
        }
    }

}

//function total() {
//    let total = 0;
//    $(".txtSubTotal").each(function () {
//        total += parseInt($(this).text())
//        $("#cartTotal").text(total);
//    });
//}

function add(item) {
    console.log(item)
    let num = $(item + " input[type=text]").val();
    let uniprice = parseInt($(item + " .txtUniPrice").text());
    let subTotal = parseInt($(item + " .txtSubTotal").text());

    subTotal += uniprice;
    num++;

    console.log(subTotal)

    $(item + " input[type=text]").val(num);
    $(item + " .txtSubTotal").text(subTotal);
}

function minus(item) {
    let num = $(item + " input[type=text]").val();
    let uniprice = parseInt($(item + " .txtUniPrice").text());
    let subTotal = parseInt($(item + " .txtSubTotal").text());

    subTotal -= uniprice;
    if (num < 2) return;
    num--;

    $(item + " input[type=text]").val(num);
    $(item + " .txtSubTotal").text(subTotal);
}

function inputRecalculate(item, itemid) {

    if (item < 1) {
        alert("請輸入大於1的數字");
        $(itemid + " input[type=text]").val(1)
        return;
    }
    let uniPrice = $(itemid + " span[class='txtUniPrice']").text();
    let subTotal = $(itemid + " span[class='txtSubTotal']").text();

    subTotal = uniPrice * item;
    $(itemid + " span[class='txtSubTotal']").text(subTotal);
}

function btnDelItem(item) {
    let data = { id: $(item).attr("data-id") }
    console.log(data);

    $.ajax({
        //url: '@Url.Action("RemoveItem", "Cart")',
        url: '/Cart/RemoveItem',
        type: "post",
        contentType: 'application/x-www-form-urlencoded',
        data: data,
        success: function (result) {
            console.log(result);

            if (result.success) {
                //$("#testdiv").html("<p>" + result.message + "</p>");
                console.log(item);
                $("#item_" + $(item).attr("data-id")).remove();
                total();
            }

            if ($("#cartList tr[id]").length == 2) {
                $("#trCheckOut").html('<td align="center" colspan="6"> 您的購物車目前是空的！</td>');
                $("#trTotal").hide();
                $("#btnCheckOut").toggle();
            }
        },
        error: function () {
            $("#testdiv").html("<p>error</p>");
        }
    });
}

function orderIn() {    

    swal({
        title: "確定成立訂單嗎?",
        //text: "",
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
                        url: "/Cart/CreateNewOrder",
                        type: "POST",
                        dataType: "Json",
                        contentType: "application/json",
                        data: countOrder(),
                        success: (res) => {
                            if (res.success) {                                
                                swal("成功", "已成功新增訂單", "success");
                                emptyCart();
                                $("#myOrder").append('<span id="cartItemCount" class="badge badge-info">1</span>');
                                $("#mymenu").attr("class", "dropdown-menu show");
                            } else {
                                swal("訂單建立失敗", res.message, "error")
                            }
                        },
                        error: (res) => {
                            alert("server side error")
                        }
                    });
                }, 1000);
            }
        });
}

function countOrder() {
    let orderlist = $("#cartList tr[id*='item']");
    let orderItem = [];

    orderlist.each(function (i, e) {
        let itemid = $("#cartList tr[id*='item']")[i].id;
        let ordercount = $("#cartList tr input[type=text]")[i].value;
        orderItem.push({
            f_productid: itemid.substr(5, itemid.length),
            f_amount: parseInt(ordercount),
        });
    });

    var data = JSON.stringify(orderItem);
    return data;
}

function emptyCart() {
    $("#cartList tr[id*='item']").remove();
    $("#trCheckOut").html('<td align="center" colspan="6"> 您的購物車目前是空的！</td>');
    $("#trTotal").hide();
    $("#btnCheckOut").toggle();
    
}

function showProductDetail(itemname, itemid) {    
    $("#cartList").toggle("normal");
    $("#btnCheckOut").toggle();
    $("#prodetail").toggle("normal");
    if (itemid != undefined) {
        $("#prodetailContent").load("/Products/GetProductDetailById/?id=" + itemid, function () {
        });        
    }    
}