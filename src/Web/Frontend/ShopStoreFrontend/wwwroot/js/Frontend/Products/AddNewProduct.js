//不進行表單中 ckEditor 驗證
$('FORM').validate({
    ignore: ".ck"
});

$(".custom-file-input").on("change", function () {
    let fileName = $(this).val().split("\\").pop();
    $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
});

$('label[required]').before('<span style="color:red">* </span>');

$("#customFile").on("change", function (e) {
    const file = this.files[0];
    const objectURL = URL.createObjectURL(file);
    $("#preview").attr("src", objectURL);
});

ClassicEditor.create(document.querySelector('#editor'))
    .then(editor => {
        
    })
    .catch(error => {
        console.error(error);
    });



//window.onload = function () {
//    CKEDITOR.instances.mytext.on('key', function () {
//        var str = CKEDITOR.instances.mytext.getData();
//        if (str.length > 50) {
//            CKEDITOR.instances.mytext.setData(str.substring(0, 50));
//        }
//    });
//};

//var maxlength = 200;
//_editor = ClassicEditor.replace("editor", { height: '130px' });
//_editor.on('key', function (event) {
//    var oldhtml = _editor.document.getBody().getHtml();
//    var description = oldhtml.replace(/<.*?>/ig, "");
//    var etop = $("#cke_1_top");
//    var _slen = maxlength - description.length;
//    var canwrite = $("<label id='canwrite'>可以輸入200字</label>");
//    if (etop.find("#canwrite").length < 1) {
//        canwrite.css({ border: '1px #f1f1f1 solid', 'line-height': '28px', color: '#999' });
//        etop.prepend(canwrite);
//    }
//    var _label = etop.find("#canwrite");
//    if (description.length > maxlength) {
//        //alert("最多可以输入"+maxlength+"個文字，您已達到最大字數限制");
//        _editor.setData(oldhtml);
//        _label.html("還可以輸入0字");
//    } else {
//        _label.html("還可以輸入" + _slen + "字");
//    }
//});

$("form").submit(function (e) {
    //阻止元素默認發生的行為
    e.preventDefault();
    $.ajax({
        async: false,
        type: $("form").attr("method"),
        url: $("form").attr("action"),
        data: $("form").serialize(),
        success: (res) => {

            if (res.success) {
                swal({
                    title: res.message,
                    text: "",
                    type: "success",
                    showLoaderOnConfirm: true,
                }, function (isConfirm) {

                });
            } else {
                swal(res.message, "", "error")
            }
        },
        error: (res) => {
            swal(res.message, "", "error")
            register = false;
        }
    });
});