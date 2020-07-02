$(function () {
    $('#Medicamentos').click(function () {
        var estoqueID = $('#EstoqueID').html().split('-');
        var href = "/Produto/Index?estoqueID=" + estoqueID[0] + "&TipoProduto=1";
        $('#Medicamentos').attr('href', href);
    });

    $('#Suprimentos').click(function () {
        var estoqueID = $('#EstoqueID').html().split('-');
        var href = "/Produto/Index?estoqueID=" + estoqueID[0] + "&TipoProduto=2";
        $('#Suprimentos').attr('href', href);
    });

    $('#pedidos1').click(function () {
        var estoqueID = $('#EstoqueID').html().split('-');
        var href = "/PedidoEstoque/Index?estoqueID=" + estoqueID[0] + "&tipoProduto=1";
        $('#pedidos1').attr('href', href);
    });

    $('#pedidos2').click(function () {
        var estoqueID = $('#EstoqueID').html().split('-');
        var href = "/PedidoEstoque/Index?estoqueID=" + estoqueID[0] + "&tipoProduto=2";
        $('#pedidos2').attr('href', href);
    });

    $('#novoPedido1').click(function () {
        var estoqueID = $('#EstoqueID').html().split('-');
        var href = "/PedidoEstoque/Create?estoqueID=" + estoqueID[0] + "&TipoProduto=1";
        $('#novoPedido1').attr('href', href);
    });

    $('#novoPedido2').click(function () {
        var estoqueID = $('#EstoqueID').html().split('-');
        var href = "/PedidoEstoque/Create?estoqueID=" + estoqueID[0] + "&TipoProduto=2";
        $('#novoPedido2').attr('href', href);
    });

    $('#compras1').click(function () {
        var href = "/PedidoCompra/Index?tipoProduto=1";
        $('#compras1').attr('href', href);
    });

    $('#compras2').click(function () {
        var href = "/PedidoCompra/Index?tipoProduto=2";
        $('#compras2').attr('href', href);
    });

    $('#novaCompras1').click(function () {
        var href = "/PedidoCompra/Create?tipoProduto=1";
        $('#novaCompras1').attr('href', href);
    })

    $('#novaCompras2').click(function () {
        var href = "/PedidoCompra/Create?tipoProduto=2";
        $('#novaCompras2').attr('href', href);
    })

    $('#FaturarPedido').click(function () {
        $('#FaturarPedido').attr('data-toggle', 'modal');
        $('#FaturarPedido').attr('data-target', '#ModalFaturarPedido');
        $("#ModalFaturarPedido form").attr('action', '/PedidoEstoque/Details');
        $("#ModalFaturarPedido form").attr('method', 'post');
    });
});

