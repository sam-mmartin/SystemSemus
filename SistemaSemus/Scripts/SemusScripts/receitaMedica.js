// Insere produtos do estoque específico no carrinho
$(function () {
    $('#btnFarmaco').click(function () {
        var ProdutoID = $("#Medicamentos option:selected").val();
        var Quantidade = $('#quant_farmaco').val();

        if (Quantidade == "") {
            alert('Informe a Quantidade!');
        } else {
            var $lista = document.querySelector('#listaMedicamentos');

            li = "<li class='list-group-item d-flex justify-content-between lh-condensed'>";
            div = div = "<div><h6 class='my-0'>Medicamento</h6>";
            small = "<small class='text-muted'>";
            input = "<input onclick='desabilitar()' type='checkbox' name='produtos' checked value='" + ProdutoID + "/" + Quantidade + "'/> ";
            tagFechar = $("#Medicamentos option:selected").html() + "</small></div>";
            span = "<span class='text-muted'>" + Quantidade + "</span></li>"
            HTMLNOVO = li + div + small + input + tagFechar + span;

            $lista.insertAdjacentHTML('afterbegin', HTMLNOVO);
            var contador = parseInt($('#cart_quantidade').html(), 10) + 1;
            $('#cart_quantidade').html(contador)
            $('#quant_farmaco').val("");
        }
    });
});

// Insere produtos não cadastrados no carrinho
$(function () {
    $('#btnNaoCadastrado').click(function () {
        var Produto = $('#descricaoProduto').val();
        var Quantidade = $('#quantidade').val();

        if (Quantidade == "") {
            alert('Informe a Quantidade!');
        } else {
            var $lista = document.querySelector('#listaMedicamentos');

            li = "<li class='list-group-item d-flex justify-content-between lh-condensed'>";
            div = div = "<div><h6 class='my-0'>Medicamento</h6>";
            small = "<small class='text-muted'>";

            if (Produto == "") {
                Produto = $("#itemSemEstoque option:selected").val();
                input = "<input onclick='desabilitar3()' type='checkbox' name='produtos3' checked value='" + Produto + "-" + Quantidade + "'/> ";
                tagFechar = $("#itemSemEstoque option:selected").html() + "</small></div>";
            } else {
                input = "<input onclick='desabilitar2()' type='checkbox' name='produtos2' checked value='" + Produto + "|" + Quantidade + "'/> ";
                tagFechar = Produto + "</small></div>";
                $('#descricaoProduto').val("");
            }

            span = "<span class='text-muted'>" + Quantidade + "</span></li>"
            HTMLNOVO = li + div + small + input + tagFechar + span;

            $lista.insertAdjacentHTML('afterbegin', HTMLNOVO);
            var contador = parseInt($('#cart_quantidade').html(), 10) + 1;
            $('#cart_quantidade').html(contador)
            $('#quantidade').val("");
        }
    });
});

function desabilitar() {
    $("input[name='produtos']").each(function () {
        $(this).change(function () {
            $(this).closest('li').remove();
        });
    });
    var contador = parseInt($('#cart_quantidade').html(), 10) - 1;
    $('#cart_quantidade').html(contador);
}

function desabilitar2() {
    $("input[name='produtos2']").each(function () {
        $(this).change(function () {
            $(this).closest('li').remove();
        });
    });
    var contador = parseInt($('#cart_quantidade').html(), 10) - 1;
    $('#cart_quantidade').html(contador);
}

function desabilitar3() {
    $("input[name='produtos3']").each(function () {
        $(this).change(function () {
            $(this).closest('li').remove();
        });
    });
    var contador = parseInt($('#cart_quantidade').html(), 10) - 1;
    $('#cart_quantidade').html(contador);
}

function collapseView() {
    $('#collapseExample').show();
}