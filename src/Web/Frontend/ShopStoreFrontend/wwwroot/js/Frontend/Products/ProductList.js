$(document).ready(function () {
    Product.Getproducts()
});

const Product = {
    Getproducts: function () {
        try {
            var md5 = JSON.parse(localStorage.getItem('item')).ajaxsign;            
        } catch {
            md5 = 0
        }
        
        $.ajax({
            async: false,
            type: 'GET',
            dataType: 'text',
            url: '/Products/ProductLists?md5string=' + md5,
            success: function (result) {
                var data = JSON.parse(result);
                if (data.success) {
                    Mainproperties.productData = data
                    localStorage.setItem('item', JSON.stringify(Mainproperties.productData))
                    console.log('%c MD5檢查碼不同，成功更新 localStorage', 'color:orange; font-size:20px;');
                    console.table(Mainproperties.productData.item);
                } else {
                    Mainproperties.productData = JSON.parse(localStorage.getItem('item'));
                    console.error('檢查碼相同，從localStorage 取出')
                    console.table(Mainproperties.productData.item);
                }
            },
            error: function () {
                alert("error");
            }
        })
    },

    ShowProductDetail: function (itemName, itemid) {
        var product = $.grep(Mainproperties.productData.item, (e, i) => {
            return e.f_pId == itemid
        })

        var p = product[0]

        if (itemName != undefined) {
            $('#navItemName').text(itemName);
        }

        $('#productList').toggle('normal');
        $('#subNav').toggle('normal');
        $('#productDetail').toggle('normal');

        if (itemid != undefined) {
            $('#prodetailCard-content').html(p.f_content)
            $('#prodetailCard-img').attr('src', p.f_picName)
            $('#prodetailCard-title b').text(p.f_name)
            $('#prodetailCard-inline p.l-grey span:eq(0)').text(p.f_categoryId)
            $('#prodetailCard-inline p.l-grey span:eq(1)').text(p.f_price)

            $('#prodetailCard-inline input:eq(0)').attr('data-id', p.f_id)
            $('#prodetailCard-inline input:eq(1)').attr('data-id', p.f_id)
        }
    },

    AddtoCart: function (item, itemname) {
        let itemid = $(item).attr('data-id');

        if ($('#cartItemCount').length > 0) {
            let cartNum = $('#cartItemCount').text();
            cartNum++;
            $('#cartItemCount').text(cartNum);
        } else {
            $('#myCart').append(`<span id='cartItemCount' class='badge badge-info'>1</span>`);
            $('#mymenu').attr('class', 'dropdown-menu show');
        }

        $.ajax({
            async: true,
            type: 'POST',
            contentType: false,
            processData: false,
            url: '/Cart/AddtoCart?id=' + itemid,
            success: function (result) {
                toastr.success('已加入購物車', itemname);
            },
            error: function () {
                toastr.error('取得清單失敗')
            }
        });
    },

    AddtoWishList: function (item) {

        var itemid = $(item).attr('data-id')
        var WishList = {
            user: user,
            item: []
        }
        var user = localStorage.getItem('user')

        if (localStorage.getItem(user) != null) {
            WishList = JSON.parse(localStorage.getItem(user))
        }

        if (WishList.item.indexOf(itemid) > -1) {
            toastr.error('已存在願望清單中')
        } else {
            WishList.item.push(itemid)
            toastr.success('成功加入願望清單')
        }

        localStorage.setItem(user, JSON.stringify(WishList))
    },

    SearchProducts: function () {
        let input, filter, table, card, h1, i, txtValue;
        input = $('#searchInput');
        filter = input.val().toUpperCase();
        table = $('#productList');
        card = $('.card');

        for (i = 0; i < card.length; i++) {
            h1 = card[i].querySelector('h1');
            if (card[i]) {
                txtValue = h1.innerText;
                if (txtValue.indexOf(filter) > -1) {
                    card[i].style.display = '';
                } else {
                    card[i].style.display = 'none';
                }
            }
        }
    },

    SortChange: function () {
        let selectedType = $('#drpSortList').val();
        let aDiv = $('.card');
        let arr = [];
        for (let i = 0; i < aDiv.length; i++) {
            arr.push(aDiv[i]);
        }
        arr.sort(function (a, b) {
            switch (selectedType) {
                case '1': //名稱
                    a = a.querySelector('h1').innerText.substr(0, 1);
                    b = b.querySelector('h1').innerText.substr(0, 1);
                    break;
                case '2': //價格
                    a = parseInt(a.querySelector('td.pl-md').innerText);
                    b = parseInt(b.querySelector('td.pl-md').innerText);
                    break;
                case '3': //熱門
                    break;
            }

            if (a > b) return 1;
            if (a < b) return -1;
            return 0;
        });
        for (let i = 0; i < arr.length; i++) {
            $('#productList').append(arr[i]);
        }
    }
}
