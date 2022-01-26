var pdEditor;

//不進行表單中 ckEditor 驗證
$('FORM').validate({
    ignore: ".ck"
});

$('.custom-file-input').on('change', function () {
    let fileName = $(this).val().split("\\").pop();
    console.log(fileName);
    $(this).siblings('.custom-file-label').addClass('selected').html(fileName);
});

$('label[required]').before('<span style="color:red">* </span>');

$('#customFile').on('change', function (e) {
    const file = this.files[0];
    console.log(file)
    const objectURL = URL.createObjectURL(file);
    console.log(objectURL)
    $('#preview').attr('src', objectURL);
});

ClassicEditor.create(document.querySelector('#editor'))
    .then(editor => {
        pdEditor = editor;
    })
    .catch(error => {
        console.error(error);
    });

$('form').submit(function (e) {
    //阻止元素默認發生的行為
    e.preventDefault();    

    var form = new FormData($('form').get(0));
    
    var content = pdEditor.getData()
    console.log('123')
    console.log(content)
    form.append('ContentText', content)

    console.log(JSON.stringify(Object.fromEntries(form)));

    console.log('addneproducts error?')
    $.ajax({
        async: false,
        type: $('form').attr('method'),
        url: $('form').attr('action'),
        //url: 'Products/CreateNewProduct?contentText=' + content,
        data: form,
        processData: false,
        contentType: false,
        success: (res) => {
            if (res.success) {
                swal({
                    title: res.message,
                    text: '',
                    type: 'success',
                    showLoaderOnConfirm: true,
                }, function (isConfirm) {
                    //if (isConfirm) {
                    //    $('#app').load('/Manager/AddNewProducts')
                    //}                    
                });
            } else {
                swal(res.message, '', 'error')
            }
        },
        error: (res) => {
            swal(res.message, '', 'error')
            register = false;
        }
    });
});