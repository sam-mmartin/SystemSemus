$(function () {
    $(".crmLink").click(function () {
        var ID = $(this).html();
        $.getJSON('/Medico/DetailsAsync?id=' + ID, DetailsCallBack);
    });
    function DetailsCallBack(json) {
        $(".user-display-info .name").html(json.Nome);
        $(".user-display-info .nick").html('<span class="mdi mdi-account"></span> ' + json.ID);
        $("#receitas").html(json.Receitas);
        $("#CRM").html(json.ID);
        $("#Area").html(json.Area);
        $("#Cadastro").html(json.Cadastro);
    }
});