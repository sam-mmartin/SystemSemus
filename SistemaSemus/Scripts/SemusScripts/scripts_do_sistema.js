$(function () {
    $('#SearchString').click(function () {
        var idTable = document.getElementById('All');
        var idTitle = document.getElementById('idTitle').innerHTML;
        var action = "";

        if (idTable == null) {
            idTable = document.getElementById('Paciente');

            if (idTable == null) {
                idTable = document.getElementById('Medico');
                action = "/Receita?Registers=Medico&ID=" + idTitle;
                $('#navbarsExampleDefault').find("form").attr('action', action);
            } else {
                action = "/Receita?Registers=Paciente&ID=" + idTitle;
                $('#navbarsExampleDefault').find("form").attr('action', action);
            }
        } else {
            action = idTitle == "" ? "/Receita?Registers=All" : "/Receita?Registers=All&ID=" + idTitle;
            $('#navbarsExampleDefault').find("form").attr('action', action);
        }
    });
});

$(function () {
    $("#tipoBusca").click(function () {
        var tipobusca = $("#tipoBusca option:selected").val();
        if (tipobusca == "1") {
            $("#searchID").unmask();
            $("#searchID").val("");
            $("#searchID").attr({ placeholder: "Digite o Nº da receita" });
        } else if (tipobusca == "2") {
            $("#searchID").attr({ placeholder: "Digite o CPF do paciente" });
            $("#searchID").val("");
            $("#searchID").mask('000.000.000-00', { reverse: true });
        }
    });
});

function buscaReceitaMedica() {
    var tipoBusca = $("#tipoBusca option:selected").val();
    var id = $("#searchID").val();
    $.ajax({
        type: 'GET',
        url: '/Receita/BuscarReceitaMedica?tipoBusca=' + tipoBusca + '&id=' + id,
        dataType: 'html',
        cache: false,
        async: true,
        success: function (data) {
            $('#modalReceitasMedicas').html(data);
        }
    });
    $('#exampleModal').modal('hide');
    $('#modalReceitasMedicas').modal('show');
}
