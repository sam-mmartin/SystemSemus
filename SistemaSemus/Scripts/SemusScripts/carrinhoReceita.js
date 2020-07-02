$(function () {
    var ProdutoID;
    var Quantidade;
    var estoqueID;
    $('#btnFarmaco').click(function () {
        ProdutoID = $("#Medicamentos option:selected").val();
        Quantidade = $("#quant_farmaco").val();
        estoqueID = $("#EstoqueSemusID").val();
        $.getJSON('/VerificarDisponibilidade/CarrinhoReceita?produtoID=' + ProdutoID + '&quantidade=' + Quantidade + '&estoqueID=' + estoqueID, carrinhoReceitaCallBack);
    });

    function carrinhoReceitaCallBack(json) {
        var $lista = document.querySelector('#listaMedicamentos');
        var $tableTitle = document.querySelector('#tableTitle');
        var $tableRow = document.querySelector('#tableRows');

        if (json.Permitir) {
            if ($tableTitle.childElementCount != 0) {
                $('#infoExcedeu').html("Selecione o produto da lista, indique a quantidade e adicione ao carrinho.");
                $("#tableTitle th").remove();
                $("#tableRows td").remove();
            }
            if (Quantidade == "") {
                alert('Informe a Quantidade!');
            } else {
                li = "<li class='list-group-item d-flex justify-content-between lh-condensed'>";
                div = "<div><h6 class='my-0'>Medicamento</h6>";
                small = "<small class='text-muted'>";
                input = "<input onclick='desabilitar()' type='checkbox' name='remedios' checked value='" + ProdutoID + "/" + Quantidade + "'/> ";
                tagFechar = $("#Medicamentos option:selected").html() + "</small></div>";
                span = "<span class='text-muted'>" + Quantidade + "</span></li>"
                HTMLNOVO = li + div + small + input + tagFechar + span;

                $lista.insertAdjacentHTML('afterbegin', HTMLNOVO);
                var contador = parseInt($('#cart_quantidade').html(), 10) + 1;
                $('#cart_quantidade').html(contador)
            }
        } else {
            th = "<th>Descrição</th><th>Quantidade</th>";
            td = "<td>" + json.Desc_Produto + "</td><td>" + json.Quant_Produto + "</td>";

            $("#tableTitle th").remove();
            $("#tableRows td").remove();
            $tableTitle.insertAdjacentHTML('afterbegin', th);
            $tableRow.insertAdjacentHTML('afterbegin', td);
            $('#infoExcedeu').html('A quantidade informada excede a quantidade total deste produto no estoque!');
            $('#tableVerificaProduto').show();
        }
    }
});

function desabilitar() {
    $("input[name='remedios']").each(function () {
        $(this).change(function () {
            $(this).closest('li').remove();
        });
    });
    var contador = parseInt($('#cart_quantidade').html(), 10) - 1;
    $('#cart_quantidade').html(contador);
}