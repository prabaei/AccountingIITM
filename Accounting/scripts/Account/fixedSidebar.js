$(function () {
   


    $('[data-toggle="popover"]').popover({ animation: true, content: elem, html: true, placement: 'bottom' });
    jQuery("li.child")
.on('click', function () {
    $(this).children(".children").slideToggle('fast');
    $('span:first', this).toggleClass('glyphicon-chevron-down');
})
.on('click', 'li', function (e) {
    e.stopPropagation();
});
});