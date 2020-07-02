$(function () {
    $('.unirPedidos').click(function () {
        var pedido = $(this).html().split(':');
        var pedidoID = pedido[1];
        var verifica = $(this).hasClass("badge-success");

        if (verifica) {
            $(this).removeClass('badge-success');
            $(this).addClass('badge-warning');
            removerPedido(pedidoID);
        } else {
            $(this).removeClass('badge-warning');
            $(this).addClass('badge-success');

            var $lista = document.querySelector('#todosPedidos');
            tagA = "<a href='#' class='list-group-item list-group-item-action flex-column align-items-start'>";
            tagDiv = "<div class='d-flex w-100 justify-content-between'>";
            tagH6 = "<h6 class='mb-1'>" + pedido[0] + ": " + pedido[1] + "</h6 >";
            input = "<input onclick='desabilitar()' type='checkbox' name='pedidos' checked value='" + pedidoID + "'/> ";
            tagFechar = "</div></a>";
            HTMLNOVO = tagA + tagDiv + tagH6 + input + tagFechar;
            $lista.insertAdjacentHTML('afterbegin', HTMLNOVO);
        }
    });
});

$(function () {
    $('#btnModalUC').click(function () {
        var estoque = $('#EstoqueID').html().split(':');
        tipoProduto = estoque[1];
        tipoProduto = tipoProduto.replace("<small", "").replace("class=", "").replace('"', "").replace('text-muted', "").replace('"', "").replace('>', "").replace("</small>", "");
        tipoProduto = tipoProduto.trim();
        if (tipoProduto.toString() == 'Medicamentos') {
            $("#ModalUnirCompras input[name='tipoProduto']").val('1');
            $("#ModalUnirCompras form").attr('action', '/PedidoCompra/UnirCompras');
            $("#ModalUnirCompras form").attr('method', 'post');
        } else {
            $("#ModalUnirCompras input[name='tipoProduto']").val('2');
            $("#ModalUnirCompras form").attr('action', '/PedidoCompra/UnirCompras');
            $("#ModalUnirCompras form").attr('method', 'post');
        }
    });
});

function desabilitar() {
    $("input[name='pedidos']").each(function () {
        $(this).change(function () {
            $(this).closest('a').remove();
        });
    });
}

function removerPedido(pedidoID) {
    $("input[name='pedidos']").each(function () {
        if ($(this).val() == pedidoID) {
            $(this).closest('a').remove();
        }
    });
}