$(function () {
    Index.InitPage();
})

var Index = {

    InitPage() {
        Index.UC.SetInput();


        $('form').submit((e) => {
            e.preventDefault();            

            $.ajax({                
                type: 'post',
                url: '/manager/Login',
                data: $("form").serialize(),
                success: (res) => {

                    if (res.success) {
                        //swal(`${res.username}，歡迎回來`, '', 'success');
                        localStorage.setItem('user', res.username);
                        swal({
                            title: `${res.username}，歡迎回來`,
                            text: '',
                            type: 'success',
                            showLoaderOnConfirm: true,
                        }, function (isConfirm) {
                            
                            window.location.href = '/Manager/Home'
                        });
                    } else {
                        swal('登入失敗', '', 'error');
                    }
                },
                error: (res) => {
                    swal('登入失敗', '網路錯誤', 'error')
                }
            });
        })

    },


    UC: {
        SetInput() {
            $(document).on('focus', '.input', this.FocusFunc);
            $(document).on('blur', '.input', this.BlurFunc);
        },

        FocusFunc() {
            $(this).parent().parent().addClass('focus');
        },

        BlurFunc() {
            $(this).parent().parent().removeClass('focus')
        }
    },

    DATA: {

    }
}