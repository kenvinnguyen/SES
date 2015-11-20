$(document).ready(function () {
    document.title = "Đăng nhập";

    var headerH = $('#header').height();
    //: 80px 60px;
    var teamp = $('#content .row:eq(0)');
    $('#content .row:eq(0)').css({
        'height': $(window).height() - headerH - 100
        , 'background-size': '100% 100%'
    });
    //css header
    $("#registry-form").hide();
    $('#header').css({ 'background-size': '100% 100%' });
    $("#radioLogin").click(function () {
        $("#login-form").show();
        $("#registry-form").hide();
    });
    $("#radioRegistry").click(function () {
        $("#login-form").hide();
        $("#registry-form").show();
    });
    $("#registry-form").validate({
        rules: {
            Username: {
                required: true
            },
            Email: {
                required: true,
                email: true
            },
            Password: {
                required: true,
                minlength: 3,
                maxlength: 20
            },
            PasswordConfirm: {
                required: true,
                minlength: 3,
                maxlength: 20,
                equalTo: '#password'
            },

        },

        messages: {
            Username: {
                required: 'Thông tin bắt buộc'
            },
            Email: {
                required: "Thông tin bắt buộc",
                email: 'Email không hợp lệ'
            },
            Password: {
                required: 'Mật khẩu chưa được nhập'
            },
            PasswordConfirm: {
                required: 'Mật khẩu chưa được xác nhận',
                equalTo: 'Xác nhận mật khẩu chưa đúng'
            },

        },

        errorPlacement: function (error, element) {
            $(error).css('color', 'red');
            error.insertAfter(element);
        },
        submitHandler: function (form) {
            $.ajax({
                type: $(form).attr('method'),
                url: $(form).attr('action'),
                data: $(form).serialize(),
                dataType: 'json',
                success: function (data) {
                    if (data.success) {
                        alertBox("Thành công!", " Đăng ký thành công", true, 5000);
                        location.reload();
                    }
                    else {
                        alertBox("Báo lỗi! ", data.message, false, 3000);
                        console.log(data.message);
                    }

                }
            })
            //.done(function (response) {
            //    if (response.success == 'success') {
            //        alert('success');
            //    } else {
            //        alert('fail');
            //    }
            //});
            return false;
        }
    });
    $("#UserName").focus();

    $("#login-form").validate({
        rules: {
            UserName: {
                required: true
            },
            Password: {
                required: true
            }
        },
        messages: {
            UserName: {
                required: 'Thông tin bắt buộc'
            },
            Password: {
                required: 'Thông tin bắt buộc'
            }
        },

        errorPlacement: function (error, element) {
            $(error).css('color', 'red');
            error.insertAfter(element.parent());
        },

        submitHandler: function (form) {
            form.submit();
        }
    });
});

function alertBox(title, content, flag, timeout) {
    $.smallBox({
        title: title,
        content: content,
        color: flag ? "#e0efd8" : "#f2dedf",
        timeout: timeout
    });
}