var user = localStorage['user'],
productList,
wishList;

$(document).ready(function () {
    Wish.GetProductList();
    Wish.SetWishList();
    Wish.SetbtnRemove();
    Wish.SetbtnAddtoCart();
    Wish.SetCheckAll();
})

var Wish = {
    //取所有產品
    GetProductList: function () {
        productList = JSON.parse(localStorage['item']);
    },
    //取願望清單
    SetWishList: function () {

        if (localStorage.getItem(user) === null) {
            Wish.ShowEmptyMsg();
            return;
        }


        wishList = JSON.parse(localStorage[user]);
        Wish.CheckWishList();

        if (wishList.item.length > 0) {
            //$.each(wishList.item, function (i, v) {

            //    var p = $.grep(productList.item, function (e) {
            //        return e.f_id == v
            //    })

            //    $('#WishTable')
            //        .append(`<tr id="${p[0].f_id}" data-id="${p[0].f_id}">
            //                    <td class="align-middle" align="center"><input type="checkbox"/></td>
            //                    <td align="center"> <img src="/img/products/images/${p[0].f_picName}"></td>
            //                    <td class="align-middle"> ${p[0].f_name} </td>
            //                    <td class="align-middle" align="center">NT$ ${p[0].f_price} </td>
            //                    <td class="align-middle" align="center"> ${p[0].categoryName} </td>
            //                    <td class="align-middle" align="center"><input type="button" class="btn btn-sm btn-outline-danger btnRemoveWish" value="刪除"></td>
            //            </tr>`)
            //})

            var content;            

            for (var i = 0, wishLength = wishList.item.length; i < wishLength; i++) {

                var p = $.grep(productList.item, function (e) {
                    return e.f_id == wishList.item[i];
                })

                content += `<tr id="${p[0].f_id}" data-id="${p[0].f_id}">
                                    <td class="align-middle" align="center"><input type="checkbox"/></td>
                                    <td align="center"> <img src="/img/products/images/${p[0].f_picName}"></td>
                                    <td class="align-middle"> ${p[0].f_name} </td>
                                    <td class="align-middle" align="center">NT$ ${p[0].f_price} </td>
                                    <td class="align-middle" align="center"> ${p[0].categoryName} </td>
                                    <td class="align-middle" align="center"><input type="button" class="btn btn-sm btn-outline-danger btnRemoveWish" value="刪除"></td>
                            </tr>`;
            }

            content += `<tr align="center"><td colspan="6"><input id="btnAddtoCart" type="button" class="btn btn-outline-primary" value="加入購物車"/></td></tr>`;
            $('#WishTable').html(content);
            //$('#WishTable').append(`<tr align="center"><td colspan="6"><input id="btnAddtoCart" type="button" class="btn btn-outline-primary" value="加入購物車"/></td></tr>`)
        } else {
            Wish.ShowEmptyMsg();
        }
    },

    SetbtnRemove: function () {
        $('.btnRemoveWish').on('click', function () {
            var row = $(this).parent().parent();
            Wish.RemoveWishItem(row.attr('data-id'));
        })
    },

    SetbtnAddtoCart: function () {
        $('#btnAddtoCart').on('click', function () {
            var addtoCartList = $(this).parent().parent().parent().find('input:checkbox:checked');
            if (addtoCartList.length > 0) {

                var postdata = [];

                //addtoCartList.each(function () {
                //    var itemid = $(this).parent().parent().attr('data-id')
                //    Wish.RemoveWishItem(itemid)
                //    postdata.push(itemid)
                //})
               
                for (var i = 0, cartLength = addtoCartList.length; i < cartLength; i++) {
                    var thisitem = $(addtoCartList[i]).parent().parent();
                    var itemid = thisitem.attr('data-id');                    
                    Wish.RemoveWishItem(itemid);
                    postdata.push(itemid);
                }

                Wish.SendData(postdata);

            } else {
                swal('請先勾選', '', 'error');
            }
        })
    },

    SetCheckAll: function () {
        $('#checkAll').click(function () {
            $('input:checkbox').not(this).prop('checked', this.checked);
        })

        //$('#checkAll').delegate('input', 'click', function () {
        //    $('input:checkbox').not(this).prop('checked', this.checked);
        //})
    },

    //移除願望項目    
    RemoveWishItem: function (item) {

        $('#' + item).remove();
        wishList.item = $.grep(wishList.item, function (e) {
            return e != item;
        });

        localStorage[user] = JSON.stringify(wishList);
        Wish.CheckWishList();
    },

    //顯示清單訊息
    ShowEmptyMsg: () =>
        //$('#WishTable')
        //    .append(`<tr>
        //                 <td align="center" colspan="6">您的清單目前是空的！</td>
        //             </tr>`)

        $('#WishTable')
            .html(`<tr>
                         <td align="center" colspan="6">您的清單目前是空的！</td>
                     </tr>`)
    ,

    //檢查WishLsit
    CheckWishList: () => {
        if (wishList.item.length == 0) {
            Wish.ShowEmptyMsg();
            $('#btnAddtoCart').remove();
        }
    },

    //傳送資料
    SendData: function (list) {
        var postData = {
            list: list
        };

        $.ajax({
            url: '/Cart/AddListToCart',
            type: 'post',
            data: postData,
            success: function (res) {
                if (res.success) {
                    swal('新增購物車成功', '', 'success');

                    if ($('#cartItemCount').length > 0) {
                        var cartNum = $('#cartItemCount').text();
                        cartNum++;
                        $('#cartItemCount').text(cartNum);
                    } else {
                        //$('#myCart').append(`<span id='cartItemCount' class='badge badge-info'>${res.addedItem}</span>`);
                        $('#myCart').html(`購物車 <span id='cartItemCount' class='badge badge-info'>${res.addedItem}</span>`);
                        $('#mymenu').attr('class', 'dropdown-menu show');
                    }

                } else {

                }
            },
            error: function (res) {
                swal(res.message, '', 'error');
            }
        })
    }
}
