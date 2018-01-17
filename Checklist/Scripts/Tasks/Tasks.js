//TODO: Testando aqui
$(document).ready(function () {

    $('.date').mask('00/00/0000');

    //$.get(Url.GetDescriptionPriority, function (data) {
    //    $.each(data, function (key, value) {
    //        console.log(value);
    //        $(GeneralSettings.PrioritySelect).append($("<option />").val(key + 1).html(value.DescriptionPriority))
    //    });
    //}, 'json');

    //LoadStep();
});

//function LoadStep() {
//    $(GeneralSettings.Step).append($("<option />").val(0).text('Selecionar...'));
//    $.getJSON(Url.GetDescriptionStep, function (data) {
//        $.each(data, function (key, item) {
//            $(GeneralSettings.Step).append($("<option />").val(item.IdStep).text(item.DescriptionStep));
//        });
//    });
//}

//Salvar tarefa
$(GeneralSettings.BtnSave).click(function () {

    var task = {
        Title: $(GeneralSettings.Title).val(),
        Description: $(GeneralSettings.Description).val(),
        StartDate: $(GeneralSettings.StartDate).val(),
        EndDate: $(GeneralSettings.EndDate).val(),
        IdPriority: $(GeneralSettings.PrioritySelect).val()
    };

    if (task.Description != "" && (task.Description).trim()) {
        $.ajax({
            url: Url.NewTask,
            dataType: "json",
            method: "POST",
            data: {
                newTask: task
            },
            success: function (data) {
                $.toaster({ priority: 'success', title: '✅', message: 'Você adicionou uma nova tarefa! Verifique no fim da lista.' });

                //Limpando os campos
                $(GeneralSettings.Title).val("");
                $(GeneralSettings.Description).val("");
                $(GeneralSettings.StartDate).val("");
                $(GeneralSettings.EndDate).val("");
                $(GeneralSettings.PrioritySelect).val(0);

                //Adicionando na Lista
                $("#description").append(
                '<tr id="' + data.IdTask + '">' +
                '<td>' +
                '<h6><b>' + data.Title + '</b></h6>' +
                '<span class="ProgressDescription Description">' + data.Description + '</label><br />' +
                '<span title="Click aqui para para mudar o progresso." onclick="getStepTask(' + data.IdTask + ');" class="ProgressStep Status">⌚️ Aguardando Início...</span>' +
                '</td>' +
                '<td><p onclick="getEditTask(' + data.IdTask + ');" id="editTask"><img src="/Images/editar.png" style="width:30px; height:30px;" title="Clicke aqui para editar..."/></p></td> ' +
                '<td><p onclick="getDeleteTask(' + data.IdTask + ');" id="deleteTask"><img src="/Images/delete.png" style="width:30px; height:30px;" title="Click aqui para deletar..."/></p></td>' +
                '</tr>'
                );

                $(".notify").hide();
            },
            error: function (error) {
                $.toaster({ priority: 'danger', title: '❌', message: 'Ops, não foi possível salvar sua tarefa!\n' + error.statusText });
            }
        });
    }
    else {
        $.toaster({ priority: 'danger', title: 'Hey', message: 'Você ainda não preencheu sua tarefa!' });
    }
});

//Editar status datarefa
var idUpgrade = 0;

//Alterar progresso da tarefa
function getStepTask(id) {
    var taskView = {
        IdTask: id
    };

    $.ajax({
        url: Url.GetTaskById,
        dataType: "Json",
        method: "GET",
        data: {
            idTask: taskView.IdTask
        },
        success: function (data) {
            var response = JSON.parse(data);
            idUpgrade = response.IdTask;
            $(GeneralSettings.ChangeStep).val(response.IdStep);
            $(GeneralSettings.DescriptionReason).val(response.LastDescriptionReason);
            if (response.IdStep == 1 || response.IdStep == 2 || response.IdStep == 6) {
                $(".reason-step").addClass("hide");
            }
            else {
                $(".reason-step").removeClass("hide");
            }
            $('.changeStepTask').modal();
        },
        error: function (error) {
            console.log(error);
            $.toaster({ priority: 'warning', title: 'Ops', message: 'não foi possível buscar a tarefa!\nErro: ' + error.statusText });
        },
        beforeSend: function () {
            $('.loader').css({ display: "block" });
        },
        complete: function () {
            $('.loader').css({ display: "none" });
        }
    })
}

$(GeneralSettings.ChangeStepBtn).click(function () {
    var reason = {
        IdTask: idUpgrade,
        IdStep: $(GeneralSettings.ChangeStep).val(),
        Description: $(GeneralSettings.DescriptionReason).val()
    }

    if (reason.IdStep != 0) {
        
        $.ajax({
            url: Url.EditStep,
            dataType: "json",
            method: "POST",
            data: {
                editStep: reason
            },
            success: function (data) {
                console.log("[SUCCESS]Step Changed:" + data);
                $('.bd-modal-sm').modal('hide');
                $('#' + idUpgrade + ' .ProgressStep').html("⌚️ " + $(GeneralSettings.ChangeStep + " option[value='" + data.IdStep + "']").text());
                $(GeneralSettings.DescriptionReason).val("");
                $.toaster({ priority: 'success', title: '✅', message: 'Status atualizado!' });
            },
            error: function (xhr, status, error) {
                var err = eval("(" + xhr.responseText + ")");
                console.log("[ERROR]Step Changed:" + err.message);
                $.toaster({ priority: 'danger', title: 'Ops', message: 'não foi possível atualizar o progresso! \n' + error.statusText });
            },
            beforeSend: function () {
                $('.loader').css({ display: "block" });
            },
            complete: function () {
                $('.loader').css({ display: "none" });
            }
        })
    }
    else {
        $.toaster({ priority: 'danger', title: 'Ops', message: 'Tarefa inválida!' });
    }
});

$(GeneralSettings.ChangeStep).change(function () {

    $(GeneralSettings.DescriptionReason).val("");

    var check = $(GeneralSettings.ChangeStep).val();

    if (check == 3 || check == 4 || check == 5) {
        $(".reason-step").removeClass("hide");
    }
    else {
        $(".reason-step").addClass("hide");
    }
});

//Buscar tarefa para edição
function getEditTask(id) {
    
    var taskView = {
        IdTask: id
    };

    $.ajax({
        url: Url.GetTaskById,
        dataType: "Json",
        method: "GET",
        data: {
            idTask: taskView.IdTask
        },
        success: function (data) {
            var response = JSON.parse(data);
            console.log(data);
            $('.ed-modal-lg').modal();
            $('.date').mask('00/00/0000');
            $(GeneralSettings.TitleModal).val(response.Title);
            $(GeneralSettings.DescriptionModal).val(response.Description);
            $(GeneralSettings.PriorityModal).val(response.IdPriority);
            $(GeneralSettings.StartDateModal).val(moment(response.StartDate).format('L'));
            $(GeneralSettings.EndDateModal).val(moment(response.EndDate).format('L'));
            idUpgrade = response.IdTask;
        },
        error: function (error) {
            console.log(error);
            $.toaster({ priority: 'warning', title: 'Ops', message: 'não foi possível buscar a tarefa!\nErro: ' + error.statusText });
        },
        beforeSend: function () {
            $('.loader').css({ display: "block" });
        },
        complete: function () {
            $('.loader').css({ display: "none" });
        }
    })
}

//Buscar tarefa para exclusão
var idDelete = 0;
function getDeleteTask(id) {
    var taskView = {
        IdTask: id
    };

    $.ajax({
        url: Url.GetTaskById,
        dataType: "Json",
        method: "GET",
        data: {
            idTask: taskView.IdTask
        },
        success: function (data) {
            var response = JSON.parse(data);
            $('.delete-task').modal();
            idDelete = response.IdTask;
        },
        error: function (error) {
            $.toaster({ priority: 'warning', title: 'Ops', message: 'não foi possível buscar a tarefa!\nErro: ' + error.statusText });
        },
        beforeSend: function () {
            $('.loader').css({ display: "block" });
        },
        complete: function () {
            $('.loader').css({ display: "none" });
        }
    })
}

//Editar tarefa
$(GeneralSettings.BtnDescription).click(function () {
    var taskView = {
        IdTask: idUpgrade,
        Title: $(GeneralSettings.TitleModal).val(),
        Description: $(GeneralSettings.DescriptionModal).val(),
        StartDate: moment($(GeneralSettings.StartDateModal).val(), "MM-DD-YYYY").format("DD-MM-YYYY"),
        EndDate: moment($(GeneralSettings.EndDateModal).val(), "MM-DD-YYYY").format("DD-MM-YYYY"),
        IdPriority: $(GeneralSettings.PriorityModal).val()
    };
   
    if (taskView.IdTask != 0 && taskView.Description != "") {
        $.ajax({
            url: Url.Edit,
            dataType: "json",
            method: "POST",
            data: {
                task: taskView
            },
            success: function (data) {
                $('.ed-modal-lg').modal('hide');
                //TODO: Ao adicionar uma tarefa e mudar o status para pronto e edita-la o status some..
                $("#" + data.IdTask + ' .Title').html(data.Title);
                $("#" + data.IdTask + ' .Description').html(data.Description);
                $.toaster({ priority: 'success', title: '✅', message: 'tarefa atualizada com sucesso!' });
            },
            error: function (error) {
                $.toaster({ priority: 'danger', title: 'Ops', message: 'não foi possível atualizar a tarefa! \n' + error.statusText });
            },
            beforeSend: function () {
                $('.loader').css({ display: "block" });
            },
            complete: function () {
                $('.loader').css({ display: "none" });
            }
        })
    }
    else {
        $.toaster({ priority: 'warning', title: 'Ops', message: 'tarefa inválida ou não preenchida!' });
    }
})

//Deletar tarefa
$(GeneralSettings.BtnDeleteTask).click(function () {
    var taskView = {
        IdTask: idDelete
    }

    if (taskView.IdTask != 0) {
        $.ajax({
            url: Url.DeleteTask,
            dataType: "json",
            method: "POST",
            data: {
                task: taskView
            },
            success: function (data) {
                $('.bd-modal-sm').modal('hide');
                $.toaster({ priority: 'success', title: '✅', message: 'Você deletou sua tarefa!' });
                $("#" + data.IdTask).remove();
            },
            error: function (data) {
                $.toaster({ priority: 'danger', title: 'Ops', message: 'não foi possível deletar esta tarefa!\n' + data.statusText });
            },
            beforeSend: function () {
                $('.loader').css({ display: "block" });
            },
            complete: function () {
                $('.loader').css({ display: "none" });
            }
        })
    }
    else {
        $.toaster({ priority: 'danger', title: 'Ops', message: 'Tarefa inválida!' });
    }
})

//----------------------------------------Google Settings----------------------------------------
//function onSignIn(googleUser) {
//    var profile = googleUser.getBasicProfile();
//    console.log('ID: ' + profile.getId()); // Do not send to your backend! Use an ID token instead.
//    console.log('Name: ' + profile.getName());
//    console.log('Image URL: ' + profile.getImageUrl());
//    console.log('Email: ' + profile.getEmail()); // This is null if the 'email' scope is not present.
//}

//function signOut() {
//    var auth2 = gapi.auth2.getAuthInstance();
//    auth2.signOut().then(function () {
//        console.log('User signed out.');
//    });
//}
