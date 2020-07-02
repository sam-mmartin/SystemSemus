$(function () {
    $('#SearchString').focusin(function () {
        var id = $('#EstoqueID').html().split('-');
        var action = "/Produto/Index?estoqueID=" + id[0];
        $('.navbar').find("form").attr('action', action);
    });
});