$("#btnSave").click(function () {
    var User = {
        Name: $("#Name").val(),
        Email: $("#Email").val(),
        Password: $("#Password").val()
    }

    $.ajax({
        url: Url.NewUser,
        dataType: "Json",
        method: "POST",
        data: {
            user: User,
            PasswordConfirm: $("#PasswordConfirm").val()
        },
        success: function (data) {
            $.toaster({ priority: 'success', title: '✅', message: 'Bem vindo ' + data.Name + '! Agora só falta confirmar seu email para realizar o login.' });

            setTimeout(function () {
                window.location.href = Url.Login;
            }, 5000);            
        },
        error: function (data) {
            $.toaster({ priority: 'warning', title: 'Hey', message: data.statusText });
        },
        beforeSend: function () {
            $('.loader').css({ display: "block" });
        },
        complete: function () {
            $('.loader').css({ display: "none" });
        }
    })
})