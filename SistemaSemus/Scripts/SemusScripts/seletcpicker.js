$(function () {
    $('select').each(function () {
        $(this).attr('data-live-search', true);
        $(this).addClass('selectpicker');
    });
});