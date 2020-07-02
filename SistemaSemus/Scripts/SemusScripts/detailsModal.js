$(function () {
    var ProdutoID;
    $('.detailsPro').click(function () {
        ProdutoID = $(this).attr('name').split('-');
        var href = "/Produto/Edit?id=" + ProdutoID[0] + "&estoqueID=" + $('#EstoqueID').html();
        $('#editarPro').attr('href', href);
        $('#Total').html(ProdutoID[1]);
        $('#Entrada').html(ProdutoID[2]);
        $('#dataEntrada').html(ProdutoID[3]);
        $('#Saida').html(ProdutoID[4]);
        $('#dataSaida').html(ProdutoID[5]);
        $('#user').html(ProdutoID[6]);
    });
});

$(function () {
    $('.detailsPed').click(function () {
        var pedido = $(this).attr('name').split('-');
        $('#idPedidoEstoque').html(pedido[0]);
        $('#estoqueSemus').html(pedido[1]);
        $('#dataEntrada').html(pedido[3]);
        $('#user').html(pedido[5]);

        if (pedido[2] == "1") {
            $('#tipoPedido').html("Medicamentos");
        }
        else {
            $('#tipoPedido').html("Suprimentos");
        }

        if (pedido[4] == "") {
            $('#dataFaturado').html("Em aberto");
        }
        else {
            $('#dataFaturado').html(pedido[4]);
        }
    });
});

$(function () {
    $('.detailsCom').click(function () {
        var compra = $(this).attr('name').split('-');
        $('#idPedidoCompra').html(compra[0]);
        $('#dataEntrada').html(compra[2]);
        $('#user').html(compra[4]);

        if (compra[1] == "1") {
            $('#tipoPedido').html("Medicamentos");
        }
        else {
            $('#tipoPedido').html("Suprimentos");
        }

        if (compra[3] == "") {
            $('#dataFaturado').html("Em aberto");
        }
        else {
            $('#dataFaturado').html(compra[3]);
        }
    });
});


