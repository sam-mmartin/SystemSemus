$(function () {
    $('#CPF').mask('000.000.000-00', { reverse: true });
    $("#CRM").focusout(function () {
        var CRM = $("#CRM").val();
        CRM = CRM.replace('-', "").replace('/', "");
        if (CRM.length === 6) {
            $("#CRM").mask('0000-AA', { reverse: true });
        } else if (CRM.length === 7) {
            $("#CRM").mask('00000-AA', { reverse: true });
        } else if (CRM.length === 8) {
            $("#CRM").mask('000000-AA', { reverse: true });
        } else if (CRM.length === 9) {
            $("#CRM").mask('0000000-AA', { reverse: true });
        } else if (CRM.length === 10) {
            $("#CRM").mask('00000000-AA', { reverse: true });
        } else if (CRM.length === 11) {
            $("#CRM").mask('000000000-AA', { reverse: true });
        } else if (CRM.length === 12) {
            $("#CRM").mask('0000000000-AA', { reverse: true });
        }
    });

    $(".DataMask").mask('00/00/0000', { reverse: true });
    $(".phone").mask('(00) 00000-0000', { reverse: false });
});

$(function () {
    $('.noborder').css('border-style', 'hidden');
});