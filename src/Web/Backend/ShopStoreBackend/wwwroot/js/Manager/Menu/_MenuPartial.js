//ajax 開關
var post_flag = false;
//AddBtn的 輸入框
var inputValue;
//AddBtn的開關
var menuAppend = true;
var focusout = false

//更新的MenuItem
var upMenuItem = [];
//新建的MenuItem
var newMenuItem = [];
var newMenuItemNum = -1;
//主選單array
var newMainMenuItem = [];

//主選單的ID
var mainMenuid = Number($('.box>.item:last>.content').attr('data-menuid'))
var mainMenuAdd = false

var clickTimeout = {
    _timeout: null,
    set: function (fn) {
        var that = this
        that.clear()
        that._timeout = setTimeout(fn, 200)        
    },
    clear: function () {
        var that = this
        if (that._timeout) {
            clearTimeout(that._timeout)
        }
    }
}

//input focus
$(document).on('focus', '.rowx input', function () {
    inputValue = $(this).val()
    focusout = false
})

//input submenu focusout
$(document).on('focusout', '.rowx input', function () {
    if(focusout) return

    var item = $(this)
    if (item.val() == '') alert('請輸入')
    if (item.val() == inputValue) return    
    menuAppend = true

    //menuItem
    var num = item.attr('data-id')
    var menuid = Number(item.parent().parent().attr('data-menuid'))
    var name = item.val()
    var controllerName = '/Manager/thisisTest'
    var isOpen = 1

    ////
    if (item.next().hasClass('bx-show') == false) isOpen = 0    
    ////

    if (num == 0) {
        newMenuItem[newMenuItemNum]= {
            f_id: newMenuItemNum,
            f_menuid: menuid,
            f_name: name,
            f_controller: controllerName,
            f_isopen: 1,
            f_level: 1
        }        
    } else {
        upMenuItem[Number(num)] = {
            f_id: Number(num),
            f_menuid: menuid,
            f_name: name,
            f_controller: controllerName,
            f_level: 1,
            f_isopen: 1,
            f_isdel: 0
        }
    }
    focusout = true
})


//主選單 focusout
$(document).on('focusout', '.mainInput', function () {    
    console.log($(this).val())
    console.log($(this).parent().parent().siblings('.content').attr('data-menuid'))

    var id = Number($(this).parent().parent().siblings('.content').attr('data-menuid'))
    var name = $(this).val()

    if (name == '') return

    newMainMenuItem[id] = {
        f_id: id,
        f_name: name,
        f_icon: 'bx-windows',
        f_level: 1,
        f_isopen: 1,
        f_issys: 0,
        f_isdel: 0
    }
})


//選單下拉動畫
$(document).on('click', '.container>.box>.item>.title>.edit', function (event) {

    var that = $(this)
    clickTimeout.set(function () {
        event.preventDefault();
        //$(this).parent().siblings('.content').slideToggle()
        //$(this).next().toggle()
        that.parent().siblings('.content').slideToggle()
        that.next().toggle()                
    })
})

//新增子選單
$(document).on('click', '.container>.box>.item>.title>.bx-add-to-queue', function (event) {
    if (menuAppend) {
        newMenuItemNum++
        $(this)
            .parent()
            .siblings('.content')
            .append(`<div class="rowx">
                        <input type="text" value="" data-id="0" data-newid="${newMenuItemNum}" placeholder="請輸入..." /><i class='bx bx-show' onclick="RemoveSelf(this)"></i>
                    </div>`)
        menuAppend = false
    }
})

//$(document).click(':not', function () {
//    if ($('.popmenu').is(':visible')) $('.popmenu').hide()
    
//    console.log('tt')
//})

//顯示icon的選單
$(document).on('click', '.title .bx:first-child', function (event) {
    $(this).siblings('.popmenu').toggle();
})

//主選單標題下拉
$(document).on('dblclick', '.edit', function (event) {
    clickTimeout.clear()    
    $(this).find('input').attr('disabled', false)
    $(this).find('input').focus()
})


$('.box').bind('change', Change)
//$('.box').bind('DOMNodeInserted', Change)

function Change() {    
    $('#btnSave').attr('disabled', false)
    mainMenuAdd = false
    $('#btnAdd').attr('disabled', false)
}

//顯示項目可見或不可見
function RemoveSelf(obj) {
    var input = $(obj).parent().find('input')

    if (input.val() == '') {
        alert('請先輸入')
        input.focus()
        return
    }

    var subMenuId = $(obj).prev().attr('data-id')    

    //新建的menuSubItem
    if (subMenuId == 0) {
        var thisSubIndex = $(obj).prev().attr('data-newid')

        if ($(obj).hasClass('bx-show') == false) {
            newMenuItem[thisSubIndex].f_isopen = 1            
        } else {
            newMenuItem[thisSubIndex].f_isopen = 0
        }        
    } else {

        var dataid = $(obj).prev().attr('data-id')
        upMenuItem[dataid] = {
            f_id: Number(dataid),
            f_menuid: Number($(obj).parent().parent().attr('data-menuid')),
            f_name: $(obj).prev().val(),
            f_controller: 'testtest',
            f_level: 1,
            f_isopen: 1,
            f_isdel: 0
        }

        if ($(obj).hasClass('bx-show') == false) {
            upMenuItem[dataid].f_isopen = 1
        } else {
            upMenuItem[dataid].f_isopen = 0
        }        
    }

    $(obj).parent().toggleClass('del')
    //$(obj).toggleClass('bx-minus-circle bx-plus-circle')
    $(obj).toggleClass('bx-show bx-hide')

    if ($(obj).parent().hasClass('del')) {
        input.attr('disabled', true)        
    } else {
        input.attr('disabled', false)        
    }

    $('#btnSave').attr('disabled', false)
}

//傳送資料
function SendData(btn) {

    if(post_flag) return    
    $(btn).attr('disabled', true)

    //去除 Array 的 Null
    upMenuItem = upMenuItem.filter(el => el)
    newMainMenuItem = newMainMenuItem.filter(el => el)

    var postData = {
        MenuSubModels: upMenuItem,
        SubItems: newMenuItem,
        MainMenuItems: newMainMenuItem
    }

    post_flag = true
    $.ajax({
        async: false,
        method: 'POST',
        url: '/Menu/AddSubMenu',
        dataType: 'json',
        contentType: 'application/json',
        processData: false,
        cache: false,
        //traditional: true,
        data: JSON.stringify(postData),
        success: function(res) {
            swal(res.message, '', 'success')
            $(btn).attr('disabled', false)
            post_flag = false
            upMenuItem = []
            newMenuItem = []
            newMenuItemNum = -1
            newMainMenuItem = []
        },
        error: function (res) {
            swal(res.message, '', 'error')
            $(btn).attr('disabled', false)
            post_flag = false
        }
    })
}

//新增主選單按鈕
function AddMenu() {
    if (mainMenuAdd) return

    mainMenuid += 1
    var name    

    $('.box').append(`<div class="item">
                    <div class="title">
                        <i class="bx bx-windows"></i>
                            <div class="edit">
                                <input class="mainInput" type="text" value="" placeholder="請輸入" />
                            </div>
                        <i class="bx bx-add-to-queue" style="position:relative;"><span data-tooltip="新增子選單" style="position:absolute; left:10px; color:rgba(0,0,0,0);">1</span></i>
                    </div>

                    <div class="content" data-menuid="${mainMenuid}">
                        <hr>


                    </div>
                </div>`)

    mainMenuAdd = true
    $('#btnAdd').attr('disabled', true)

    //$(".box").animate({ scrollTop: $('.box').offset().top }, 5000);
    $('.box').animate({ scrollTop: 5000}, 1000)
}

