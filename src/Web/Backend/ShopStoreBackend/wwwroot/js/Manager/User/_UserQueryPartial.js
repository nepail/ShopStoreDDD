$(document).ready(function () {
    User.DATA.GetUsers();
    User.InitUser();
})


var postData = MainProperties.User.postData;
var identity;

var User = {

    InitUser: function () {
        User.UC.SetForm();
        User.UC.SetBtnClick();
        User.UC.SetSearch();
    },

    DATA: {
        GetUsers: function () {
            $.ajax({
                url: '/Manager/GetUsers',
                type: 'get',
                success: (res) => {
                    if (res.success) {
                        MainProperties.User.data = res.item;
                        MainProperties.User.group = res.group;
                        User.UC.SetUserList();                                    
                    } else {
                        console.log(res)
                        swal('載入失敗', '資料庫出現錯誤', 'error');
                    }
                },
                error: (res) => {
                    swal('載入失敗', '網路出現錯誤', 'error');
                    
                }
            })
        },

        GetUserGroup: function (md5String) {

            $.ajax({
                url: '/Manager/GetUserGroup?md5=' + md5String,
                type: 'get',
                success: (res) => {
                    if (res.success) {

                        var userGroup = {
                            sign: res.sign,
                            group: res.item
                        }
                        localStorage.setItem('UserGroup', JSON.stringify(userGroup))
                        User.UC.SetUserGroupSelect();
                    } else {
                        //從localstroage取出
                        
                    }
                },
                error: (res) => {
                    swal('載入失敗', '網路出現錯誤', 'error');
                }
            })
        },

        GetUserPermission: function (userid) {

            $.ajax({
                url: '/Manager/GetUserPermissionsByID?userId=' + userid,
                type: 'get',
                statusCode: {
                    200(res) {
                        if (res.success) {
                            User.UC.SetUserPermissions(res.groupList);
                        }
                    },
                    401 (){
                        alert('閒置太久未動作，您已被登出');
                        window.location.href = '/Manager';
                    }
                },
                //success: (res) => {
                //    if (res.success) {
                //        User.UC.SetUserPermissions(res.groupList)
                //    } else {
                //        console.log(res)
                //        swal('載入失敗', '資料庫出現錯誤', 'error');
                //    }
                //},
                //error: (res) => {
                //    if (res.status == 401) {
                //        window.location.href = '/Manager';
                //    };
                //    swal('載入失敗', '網路出現錯誤', 'error');
                //}
            })
        },

        AddUser: function (data) {
            $.ajax({
                url: '/Manager/AddUser',
                type: 'post',
                data: data,
                success: (res) => {
                    if (res.success) {
                        swal('新增成功', ' ', 'success');
                        User.UC.ResetForm();
                        User.DATA.GetUsers();
                    } else {
                        console.log(res)
                        swal('新增失敗', '資料庫出現錯誤', 'error');
                    }
                },
                error: (res) => {
                    swal('新增失敗', '網路出現錯誤', 'error');
                }
            })
        },

        UpdateUser: function () {

        },

        UpdateUserPermissions: function (item) {

            if (postData['PermissionData'] != undefined) {
                $.ajax({
                    url: '/Manager/UpdatePermissionsByID',
                    type: 'post',
                    cache: false,
                    data: postData,
                    success: res => {
                        if (res.success) {
                            swal('更新成功', ' ', 'success');
                            User.UC.SetUserEditSwitch();

                            var useritem;
                            useritem = $('#userList>.box-content').filter(function (e) {
                                return $(this).data('id') == $('#userId').attr('data-id')
                            });

                            User.UC.ShowUserDetail(useritem[0]);

                            $('#userGName').text(identity)

                            User.DATA.GetUsers();

                        } else {
                            swal('資料庫錯誤', ' ', 'error')
                        }
                    },
                    error: res => {
                        swal('網路錯誤', ' ', 'error')
                    }
                })
            } else {
                User.UC.SetUserEditSwitch();
            }
        },

        TempPostData: function (item) {
            var fn = $(item).find('input[type=checkbox]');

            var codeStr = '';

            for (var i = 0; i < fn.length; i++) {
                codeStr += $(fn[i]).attr('data-id');
            }

            var userid = parseInt($(item).parent().parent().siblings('.section-content').attr('data-id'));
            var menuid = parseInt($(item).parent().attr('id')[3]);
            var code = parseInt(codeStr, 2);

            //if (postData[userid] == undefined || postData == undefined) {
            //    postData = {
            //        PermissionData: {
            //            userId: userid,
            //            PermissionDetails: {
            //                [menuid]: {
            //                    menuId: menuid,
            //                    permissionsCode: code,
            //                }
            //            }
            //        }
            //    }
            //}

            if (postData['PermissionData'] == undefined || postData == undefined) {
                postData = {
                    PermissionData: {
                        userId: userid,
                        PermissionDetails: [{
                            menuId: menuid,
                            permissionsCode: code,
                        }]
                    }
                }
            }

            var index = postData['PermissionData'].PermissionDetails.findIndex(p => p.menuId == menuid)
            if (index < 0) {
                postData['PermissionData'].PermissionDetails.push({
                    menuId: menuid,
                    permissionsCode: code,
                })
            } else {
                postData['PermissionData'].PermissionDetails[index] = {
                    menuId: menuid,
                    permissionsCode: code,
                }
            }
        }
    },

    UC: {

        //更新fn上的DataId
        UpdatePermissionsDataId: function (item) {
            ($(item).is(':checked')) == true ? $(item).attr('data-id', '1') : $(item).attr('data-id', '0')
        },

        SetUserList: function (arr) {

            var userList;
            if (arr == undefined) {
                userList = MainProperties.User.data;
            } else {
                userList = arr;
            }
          
            var userContent =
            `<div class="box listfn">
                <i id="btnSortAtoZ" class='bx bx-sort-a-z'></i>
                <i id="btnSortDown" class='bx bx-sort-down' ></i>
             </div>`;

            for (var i = 0, len = userList.length; i < len; i++) {
                var r = userList[i];
                userContent += `
                    <div class="box box-content" data-id="${r.id}">
                        <div class="box-section">
                            <div class="box-img">${r.name[0]}</div>
                        </div>
                        <div class="box-section tx">
                            ${r.name}
                            <p>${r.account}</p>
                            <p>ID : ${r.id}</p>
                        </div>
                    </div>`
            };

            $('#userList').html(userContent);
        },

        SetUserPermissions: function (groupList) {

            //groupList = {
            //    '產品管理': 15,
            //    '會員管理': 6,
            //    '訂單管理': 12
            //};                        

            var groupContent = '';

            for (var i = 0, len = groupList.length; i < len; i++) {

                var permissionsCode = (groupList[i].permissionDetail.permissionCode).toString('2').padStart(4, 0);

                var _read = {
                    switchStr: permissionsCode[0] == 1 ? 'checked' : '',
                    value: permissionsCode[0] == 1 ? 1 : 0
                }

                var _insert = {
                    switchStr: permissionsCode[1] == 1 ? 'checked' : '',
                    value: permissionsCode[1] == 1 ? 1 : 0
                }

                var _update = {
                    switchStr: permissionsCode[2] == 1 ? 'checked' : '',
                    value: permissionsCode[2] == 1 ? 1 : 0
                }

                var _delete = {
                    switchStr: permissionsCode[3] == 1 ? 'checked' : '',
                    value: permissionsCode[3] == 1 ? 1 : 0
                }

                groupContent += `
                    <div id="fn_${groupList[i].menuId}" class="section-function" >
                        <div class="main-row" onChange="User.DATA.TempPostData(this)">
                            <div class="function-content">
                                <span>${groupList[i].permissionDetail.menuName}</span>                                
                            </div>
                            <div class="function-content">
                                讀取<br />

                                <div>
                                    <label>
                                        <input type="checkbox" onChange="User.UC.UpdatePermissionsDataId(this)" class="checkbox read" ${_read.switchStr} data-id="${_read.value}" disabled/>
                                        <span class="btn-box">
                                            <span class="btnb"></span>
                                        </span>
                                    </label>
                                </div>
                            </div>
                            <div class="function-content">
                                新增<br />
                                <div>
                                    <label>
                                        <input type="checkbox" onChange="User.UC.UpdatePermissionsDataId(this)" class="checkbox insert" ${_insert.switchStr} data-id="${_insert.value}" disabled/>
                                        <span class="btn-box">
                                            <span class="btnb"></span>
                                        </span>
                                    </label>
                                </div>
                            </div>
                            <div class="function-content">
                                修改<br />
                                <div>
                                    <label>
                                        <input type="checkbox" onChange="User.UC.UpdatePermissionsDataId(this)" class="checkbox update" ${_update.switchStr} data-id="${_update.value}" disabled/>
                                        <span class="btn-box">
                                            <span class="btnb"></span>
                                        </span>
                                    </label>
                                </div>
                            </div>
                            <div class="function-content">
                                刪除<br />
                                <div>
                                    <label>
                                        <input type="checkbox" onChange="User.UC.UpdatePermissionsDataId(this)" class="checkbox delete" ${_delete.switchStr} data-id="${_delete.value}" disabled/>
                                        <span class="btn-box">
                                            <span class="btnb"></span>
                                        </span>
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>`;
            }

            $('#group_permissions').html(groupContent)
        },

        /**
         * 設定按鈕
         * */
        SetBtnClick: function () {
            $('#btnAddUser').off('click').click(function () {
                User.UC.ResetForm();
                User.UC.SetUserGroupSelect();
            });

            $(document).on('click', '.box-content' ,function () {
                User.UC.ShowUserDetail(this);
            });

            $(document).on('click', '#btnSortAtoZ', function () {
                var arr = MainProperties.User.data;


                arr.sort(function (a, b) {
                    //a = (a.querySelector('.tx').firstChild.nodeValue).trim()[0];
                    //b = (b.querySelector('.tx').firstChild.nodeValue).trim()[0];
                    a = a.name[0];
                    b = b.name[0];

                    var reg = /[a-zA-Z0-9]/;
                    if (reg.test(a) || reg.test(b)) {                        
                        if (a > b) return 1;
                        if (a < b) return -1;
                        return 0;
                    } else {                        
                        return a.localeCompare(b);
                    }

                    //return a.localeCompare(b)
                    //return b.localeCompare(b)

                    //if (a > b) return 1;
                    //if (a < b) return -1;
                    //return 0;
                });
                
                User.UC.SetUserList(arr)                               
            });

            $(document).on('click', '#btnSortDown', function () {
                var arr = MainProperties.User.data;
                arr.sort(function (a, b) {
                    return a.id - b.id;
                });

                User.UC.SetUserList(arr);
            })

            User.UC.SetUserEditBtn();
            User.UC.SetUserDeleteBtn();
        },

        SwitchSort: function () {
            $('#btnSortAtoZ').toggleClass('bx-sort-z-a bx-sort-a-z');
            console.log('111')
        },


        ShowUserDetail: function (item) {

            if ($('#userEdit').hasClass('bxs-save')) User.UC.SetUserEditSwitch();

            if ($('#containerR').is(':visible')) {
                $('#containerR').toggle()
            }

            if (!$('#containerP').is(':visible')) {
                $('#containerP').toggle()
            }

            var id = $(item).attr('data-id');
            var user = $.grep(MainProperties.User.data, function (e) {
                return e.id == id
            })[0]


            $('#smWord').text(user.name[0])
            $('#userName').text(user.name);
            $('#userAccount').text(user.account);
            //$('#userId').text(user.id);
            $('#userId').attr('data-id', user.id);
            $('#userId').attr('data-groupId', user.groupId);
            $('#userGName').text(user.groupName);
            $('#userName').text(user.name);
            $('#userCreatTime').text('建立時間：' + user.createTime);
            $('#userUpdateTime').text('修改時間：' + user.updateTime);

            User.DATA.GetUserPermission($(item).attr('data-id'))
        },

        SetUserEditBtn: function () {
            $('#userEdit').click(function () {

                if ($('#userGName').is('span')) {
                    $('#userGName')
                        .replaceWith(`
                            <select id="userGName" class="userIdentityEdit" onChange="User.UC.TempIdentity(this)" >
                                <option value="1">Admin</option>
                                <option value="2">Normal</option>
                            </select>`);

                    var useritem = MainProperties.User.data.filter(function (e) {
                        return e.id == $('#userId').attr('data-id')
                    })[0];

                    //刷新使用者資訊
                    $('#userGName').val(useritem.groupId)                                        
                }

                $(this).toggleClass('bxs-pencil bxs-save')
                $('#group_permissions input[type=checkbox]').attr('disabled', false)
                $('#group_permissions input[type=checkbox]').siblings('.btn-box').children().css('border', '2px solid black')

                $(this).off('click').click(function () {
                    User.DATA.UpdateUserPermissions(this);
                })
            })
        },

        SetUserEditSwitch: function () {

            if ($('#userGName').is('select')) {
                $('#userGName')
                    .replaceWith(`
                            <span id="userGName">${identity}</span>`)
            }

            $('#userEdit').toggleClass('bxs-pencil bxs-save');
            $('#userEdit').off('click');
            $('#group_permissions input[type=checkbox]').attr('disabled', true);
            $('#group_permissions input[type=checkbox]').siblings('.btn-box').children().css('border', 'none');
            User.UC.SetUserEditBtn();
            postData = {};
        },

        SetUserDeleteBtn: function () {
            $('#userDelete').click(function () {
                var userId = $(this).parent().parent().parent().attr('data-id');

                swal({
                    title: "確定執行此操作嗎?",
                    text: "刪除後的資料將無法復原",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonText: "確定",
                    cancelButtonText: "取消",
                    closeOnConfirm: false,
                    showLoaderOnConfirm: false,
                },
                    function (isConfirm) {
                        ///Manager/DeleteUserByID
                        if (isConfirm) {
                            $.ajax({
                                url: '/Manager/RemoveUserByID?userId=' + userId,
                                type: 'get',
                                success: (res) => {
                                    if (res.success) {
                                        swal('刪除成功', ' ', 'success');
                                        $('#containerP').toggle();


                                        $('#userList>.box-content').filter(function (e) {
                                            return $(this).data('id') == userId
                                        }).remove();
                                    } else {
                                        swal('刪除失敗', '資料庫異常', 'error');
                                    }
                                },
                                error: (res) => {
                                    swal('刪除失敗', '網路異常', 'error');
                                }
                            });
                        } else {

                        }
                    });
            })
        },

        /**
         * 搜尋功能
         */
        SetSearch: function () {
            $('#userSearch').on('keyup', function () {
                var value = $(this).val().toLowerCase();
                $('#userList>.box-content').filter(function () {
                    $(this).toggle($(this).find('.tx').text().toLowerCase().indexOf(value) > -1);
                })
            })
        },
        /**
         * 設定表單
         * */
        SetForm: function () {
            $('#rgForm').validate({
                rules: {
                    name: {
                        rangelength: [2, 5],
                        required: true,
                    },

                    account: {
                        rangelength: [5, 10],
                        required: true,
                    },

                    password: {
                        required: true,
                        minlength: 5
                    },
                    password_confirm: {
                        required: true,
                        minlength: 5,
                        equalTo: "#rgpCode"
                    },

                    messages: {
                        password: {
                            required: '請輸入密碼',
                            minlength: '不得小於5字元'
                        }
                    }
                }
            });

            $('#submit').click(function (e) {
                e.preventDefault();
                if (!$('#rgForm').valid()) return;


                postData = {
                    f_account: $('#rgAccount').val(),
                    f_pcode: $('#rgpCode').val(),
                    f_groupId: $('#rgSelect :selected').val(),
                    f_name: $('#rgName').val()
                }

                User.DATA.AddUser(postData);
            });

            $.extend($.validator.messages, {
                required: "這是必填項目",
                remote: "請修正此項目",
                email: "請輸入有效的電子郵件地址",
                url: "請輸入有效的網址",
                date: "請輸入有效的日期",
                dateISO: "請輸入有效的日期 (YYYY-MM-DD)",
                number: "請輸入有效的數字",
                digits: "只能輸入數字",
                creditcard: "請輸入有效的信用卡號碼",
                equalTo: "請輸入相同的密碼",
                extension: "請輸入有效的後綴",
                maxlength: $.validator.format("最多可以輸入 {0} 個字元"),
                minlength: $.validator.format("最少要輸入 {0} 個字元"),
                rangelength: $.validator.format("請輸入長度在 {0} 到 {1} 之間的字元"),
                range: $.validator.format("請輸入範圍在 {0} 到 {1} 之間的數值"),
                max: $.validator.format("請輸入不大於 {0} 的數值"),
                min: $.validator.format("請輸入不小於 {0} 的數值")
            });
        },
        /**
         * 重置表單
         * */
        ResetForm: function () {
            if ($('#containerP').is(':visible')) {
                $('#containerP').toggle()
            }
            $('#containerR').toggle();
            $('form')[0].reset();
        },

        SetUserGroupSelect: function () {

            if (localStorage.getItem('UserGroup') == null) {
                User.DATA.GetUserGroup(0);
                return;
            }

            var userGroup = JSON.parse(localStorage.getItem('UserGroup'));
            User.DATA.GetUserGroup(userGroup.sign);

            var selectContent = '<option selected value="" class="required">請選擇</option>';

            for (key in userGroup.group) {

                selectContent += `<option value="${key}">${userGroup.group[key]}</option>`;                
            }

            $('#rgSelect').html(selectContent);
        },

        TempIdentity: function () {
            identity = $('#userGName :selected').text();

            if (postData['PermissionData'] == undefined) {
                postData = {
                    PermissionData: {
                        PermissionDetails: [],
                        userId: 1,
                        groupId: 1
                    }
                }
            }

            postData['PermissionData']['userId'] = parseInt($('#userId').attr('data-id'));
            postData['PermissionData']['groupId'] = parseInt($('#userGName :selected').val());
        }
    }
}



