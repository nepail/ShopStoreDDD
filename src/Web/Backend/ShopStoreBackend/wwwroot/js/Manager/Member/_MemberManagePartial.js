$(document).ready(function () {
    Member.InitPage();

})

var Member = {

    InitPage: function () {
        Member.DATA.GetMemberList();
        Member.UC.SetBtnSave();
        Member.UC.SetDropDownList();
    },

    DATA: {
        //取得Member資料
        GetMemberList: function () {
            $.ajax({
                url: '/Manager/GetMemberList',
                type: 'get',
                success: function (res) {
                    if (res.success) {
                        MainProperties.Member.data = res.result;
                        Member.UC.RenderMemberList();
                    } else {
                        swal('系統錯誤', '資料庫錯誤', 'error');
                    }
                },
                error: function () {
                    swal('網路錯誤', '無法連上伺服器', 'error');
                }
            })
        },

        //更新會員資料
        SendData: function () {

            var postTarget = MainProperties.Member.data.filter(t => t.isUpdate == 1);

            if (postTarget == 0) {

                return;
            }

            var data = {
                MemberModel: postTarget
            }

            $.ajax({
                url: '/Manager/UpdateMember',
                type: 'post',
                data: data,
                success: (res) => {
                    if (res.success) {
                        swal('保存成功', ' ', 'success');
                        MainProperties.Member.data.filter(t => t.isUpdate == 1)
                            .forEach(t => t.isUpdate = 0);
                    } else {
                        swal('保存失敗', '資料庫錯誤', 'error');
                    }
                },
                error: (res) => {
                    swal('保存失敗', '網路異常', 'error');
                }
            })
        },

        //會員停權功能
        SuspendByID: function (item) {

            var memberId = parseInt($(item).parent().siblings('.bcol:eq(1)').text());
            var isSuspend = $(item).hasClass('btn-outline-danger') ? 1 : 0;

            swal({
                title: '確定執行此操作?',
                type: 'warning',
                showCancelButton: true,
                confirmButtonText: '確定',
                cancelButtonText: '取消',
                closeOnConfirm: false,
                showLoaderOnConfirm: true,
            },
                function (isConfirm) {
                    if (isConfirm) {
                        setTimeout(function () {
                            $.ajax({
                                url: `/Manager/SuspendByMemberId?memberId=${memberId}&isSuspend=${isSuspend}`,
                                type: 'put',
                                success: (res) => {
                                    if (res.success) {
                                        swal('執行成功', ' ', 'success');
                                        $(item).parent().prev().text(isSuspend == 1 ? '是' : '否');
                                        //$(item).attr('disabled', true);
                                        //$(item).css('visibility', 'hidden');
                                        $(item).toggleClass('btn-outline-danger btn-outline-success');
                                        $(item).val(isSuspend == 1 ? '復權' : '停權');
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

    },

    UC: {
        //渲染Order列表
        RenderMemberList: function () {

            var listContent = `            
                            <div class="box brow boxtitle">
                                <div class="box bcol rowckbox">
                                    <input type="checkbox" class="visibility" />
                                </div>
                                <div class="box bcol">會員編號</div>
                                <div class="box bcol">姓名</div>
                                <div class="box bcol">暱稱</div>
                                <div class="box bcol">帳號</div>
                                <div class="box bcol">等級</div>
                                
                                <div class="box bcol">是否停權</div>
                                <div class="box bcol rowckbox">
                                    <input type="button" class="btn btn-sm btn-outline-danger" value="停權" style="visibility: hidden" />
                                    <i class="bx bx-search"></i>
                                    <input id="searchInput" type="search" placeholder="Search..." />
                                </div>
                            </div>`;
            //<div class="box bcol">購物金</div>

            for (var i = 0, len = MainProperties.Member.data.length; i < len; i++) {

                var m = MainProperties.Member.data[i];

                var btnSuspend =
                {
                    badge: m.isSuspend == 1 ? 'success' : 'danger',
                    text: m.isSuspend == 1 ? '復權' : '停權'
                }

                var statusCSS = MainProperties.Member.GetStatusCSS(m.level);

                listContent += `
                            <div id="member" class="box brow order">
                                <div class="box bcol rowckbox">
                                    <input type="checkbox" class="visibility" />
                                </div>
                                <div class="box bcol tx"><span>${m.id}</span></div>
                                <div class="box bcol tx"><span>${m.name}</span></div>
                                <div class="box bcol tx"><span>${m.nickName}</span></div>
                                <div class="box bcol tx"><span>${m.account}</span></div>
                                <div class="box bcol">
                                    <div class="size">
                                        <span class="bbadge field ${statusCSS}">LV ${m.level}</span>
                                        <ul menu-type="0" class="list">
                                            <li data-type="1">LV 1</li>
                                            <li data-type="2">LV 2</li>
                                            <li data-type="3">LV 3</li>
                                            <li data-type="4">LV 4</li>
                                            <li data-type="5">LV 5</li>
                                            <li data-type="6">LV 6</li>
                                        </ul>
                                    </div>
                                </div>
                                
                                <div class="box bcol tx"><span>${m.isSuspend == 1 ? '是' : '否'}</span></div>
                                <div class="box bcol rowckbox">
                                    <input id="btnSuspend" type="button" class="btn btn-sm btn-outline-${btnSuspend.badge}" value="${btnSuspend.text}" onclick="Member.DATA.SuspendByID(this)"/>
                                </div>
                            </div>`;
            }
            //<div class="box bcol tx">NT$<span>${m.money}</span></div>
            $('#memberList').html(listContent);
            Member.UC.SetSearch();
        },

        //搜尋功能
        SetSearch: function () {
            $('#searchInput').on('keyup', function () {
                var value = $(this).val().toLowerCase();
                $('#memberList>.order').filter(function () {
                    $(this).toggle($(this).find('.tx, .field').text().toLowerCase().indexOf(value) > -1);
                })
            })
        },

        //保存功能
        SetBtnSave: function () {
            $('#btnSave').click(function () {
                Member.DATA.SendData();
            })
        },

        //狀態清單下拉
        SetDropDownList: function () {

            //註冊styleddropdown
            $.fn.styleddropdown = function () {
                return this.each(function () {
                    var obj = $(this).off('click');
                    obj.find('.field').off('click').click(function () {
                        obj.find('.list').fadeIn(400);

                        $(document).off('keyup').keyup(function (event) {
                            if (event.keyCode == 27) {
                                obj.find('.list').fadeOut(400);
                            }
                        });

                        obj.find('.list').hover(function () { },
                            function () {
                                $(this).fadeOut(400);
                            });
                    });

                    obj.find('.list li').off('click').click(function () {

                        var type = $(this).attr('data-type');
                        var memberid = obj.find('.field').parent().parent().siblings().eq(1).find('span').text();
                        var targetClass = obj.find('.field').attr('class').split(' ')[2];
                        obj.find('.field')
                            .text($(this).html())
                            .toggleClass(`${targetClass} ${Member.UC.ChangeStatus(type, memberid)}`);

                        obj.find('.list').fadeOut(400);
                    });
                });
            };

            //.size 繫結 click 事件
            $(document).off('click').on('click', '.size', function () {
                $('.size').styleddropdown();
            })
        },

        /**
         * 等級變更功能
         * @param {Number} statusCode f_status 的狀態碼
         * @param {Number} type Menu 的類型     
         * @param {Number} meberid 會員ID
         * @returns 狀態的CSS Style
         */
        ChangeStatus: function (statusCode, memberid) {

            var target = (MainProperties.Member.data.filter(x => x.id == memberid))[0];
            target.level = parseInt(statusCode);
            target.isUpdate = 1; //在緩存註記已更新資料

            return MainProperties.Member.GetStatusCSS(statusCode);
        },
    },

}

