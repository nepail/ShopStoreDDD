
$(document).ready(function () {
    ResetPcode.InitPage.on();
})


var mail = '';

var ResetPcode = {

    InitPage: {
        on() {
            $(document).on('submit', '#formForgetPcode', function (e) {
                e.preventDefault();
                console.log('dee')
                ResetPcode.DATA.CheckEmail();
            });

            $(document).on('submit', '#formCheckPcode', function (e) {
                e.preventDefault();
                console.log('ee')
                ResetPcode.DATA.CheckCode();
            });

            $(document).on('submit', '#formResetPcode', function (e) {
                e.preventDefault();
                console.log('le')
                var inputPcode = $('#inputPcode').val();
                var confirmPcode = $('#confirmPcode').val();
                var message = $('#message');

                if (inputPcode != confirmPcode) {
                    message.css('color', 'red').text('兩次輸入密碼不相同');
                    return;
                } else {
                    ResetPcode.DATA.ResetCode(inputPcode);
                }
            });


            $('label[required]').before('<span style="color:red">* </span>');

            $('#btnLogin').click(function () {                
                localStorage.setItem('user', $('#inputAccount').val());
                //建立長連接                
            });

            $('#forgetPcode').click(function () {
                ResetPcode.UC.ShowCheckEmailPage();
            })
        }
    },



    DATA: {
        CheckEmail() {
            $.ajax({
                url: '/Member/ForgetPcode?mail=' + $('#inputEmail').val(),
                success: res => {
                    if (res.success) {
                        swal({
                            title: '成功',
                            text: '請於10分鐘內至信箱確認',
                            type: 'success',
                            showLoaderOnConfirm: true,
                        }, function (isConfirm) {
                            mail = $('#inputEmail').val();
                            ResetPcode.UC.ShowCheckCodePage();
                        });
                    } else {
                        swal('寄認證信失敗', res.msg, 'error')
                    }
                },
                error: res => {
                    swal('無法連接伺服器', '網路錯誤', 'error')
                }
            })
        },

        CheckCode() {
            
            $.ajax({
                url: `/Member/CheckCode?code=${$('#inputCode').val()}&mail=${mail}`,
                success: res => {

                    if (res.success) {
                        swal({
                            title: '認證成功',
                            text: '',
                            type: 'success',
                            showLoaderOnConfirm: true,
                        }, function (isConfirm) {                            
                            ResetPcode.UC.ShowResetPcodePage();
                        });
                    } else {
                        swal('認證失敗', res.msg, 'error')
                    }
                },
                error: res => {
                    swal('無法連接伺服器', '網路錯誤', 'error')
                }
            })
        },

        ResetCode(code) {
            $.ajax({
                url: `/Member/ResetPcode?code=${code}&mail=${mail}`,                
                success: res => {
                    if (res.success) {
                        swal({
                            title: '密碼重設成功',
                            text: '請重新登入',
                            type: 'success',
                            showLoaderOnConfirm: true,
                        }, function (isConfirm) {
                            mail = '';
                            window.location.href = '/Member/Login';
                        });
                    } else {
                        swal('重設失敗', res.msg, 'error')
                    }
                },
                error: res => {
                    swal('無法連接伺服器', '網路錯誤', 'error')
                }
            })
        }
    },

    UC: {
        ShowCheckEmailPage() {
            $('#memberLogin').html(`
                <div>
                    <h4>忘記密碼</h4>
                    <hr />
                </div>

                <div>
                    <form id="formForgetPcode" enctype="multipart/form-data" asp-action="forgetPassword">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <input type="email" id="inputEmail" class="form-control" name="Email" value="" placeholder="輸入註冊時的 Email"  onblur="this.focus()" autofocus required />
                                </div>
                            </div>
                        </div>

                        <div>
                            <input id="btnCheckEmail" class="btn btn-lg btn-primary" type="submit" value="送出驗證信件"  />
                        </div>
                    </form>
                </div>`)
        },

        ShowCheckCodePage() {
            $('#memberLogin').html(`
                <div>
                    <h4>輸入重置認證碼</h4>
                    <hr />
                </div>

                <div>
                    <form id="formCheckPcode">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <input id="inputCode" class="form-control" placeholder="輸入認證碼"  onblur="this.focus()" autofocus required />
                                </div>
                            </div>
                        </div>

                        <div>
                            <input id="btnCheckCode" class="btn btn-lg btn-primary" type="submit" value="認證" />
                        </div>
                    </form>
                </div>`)
        },

        ShowResetPcodePage() {
            $('#memberLogin').html(`
                            <div>
                                <h4>重置密碼</h4>
                                <hr />
                            </div>

                            <div>
                                <form id="formResetPcode" enctype="multipart/form-data">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <input id="inputPcode" type="password" class="form-control" placeholder="請輸入密碼"  required />
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <input id="confirmPcode" type="password" class="form-control" placeholder="輸入相同密碼" required />
                                                <p id="message"></p>
                                            </div>
                                        </div>
                                    </div>

                                    <div>
                                        <input id="btnResetPcode" class="btn btn-lg btn-primary" type="submit" value="重置密碼"/>                            
                                    </div>
                                </form>
                            </div>`)
        }
    }
}