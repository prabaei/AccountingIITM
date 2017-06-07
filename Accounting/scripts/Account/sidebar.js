$(function () {
    $('[data-toggle="popover"]').popover({ animation: true, content: elem, html: true, placement: 'bottom' });
    var sidebarToggle = 0;
    
    $("#SideNavToggle").click(function (e) {
        e.preventDefault();
        switch (sidebarToggle) {
            case 1:
                $("#nav-sidebar").animate({
                    width: '0px'
                }, { duration: 200, queue: false });
                $("#main-content").animate({
                    'padding-left': '0px'
                }, { duration: 200, queue: false });
                sidebarToggle = 0;
                break;
            case 0:
                $("#nav-sidebar").animate({
                    width: '180px',
                    height:'100%'
                }, { duration: 200, queue: false });
                $("#main-content").animate({
                    'padding-left': '180px'
                }, { duration: 200, queue: false });
                sidebarToggle = 1;
                break;
        }
    });
    jQuery("li.child")
.on('click', function () {
    $(this).children(".children").slideToggle('fast');
    $('span:first', this).toggleClass('glyphicon-chevron-down');
})
.on('click', 'li', function (e) {
    e.stopPropagation();
});
});