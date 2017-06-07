function ClearTestBox(selector) {
    $(selector).val('');
};
function ClearLabel(selector) {
    $(selector).html('');
}
function searchIcon(selector) {
    $(selector).addClass("glyphicon-search");
    $(selector).removeClass("glyphicon-triangle-bottom");
    $(selector).removeClass("glyphicon-ok");
}
function BottomTriangleIcon(selector) {
    $(selector).removeClass("glyphicon-search");
    $(selector).addClass("glyphicon-triangle-bottom");
    $(selector).removeClass("glyphicon-ok");
}
function OkIcon(selector) {
    $(selector).removeClass("glyphicon-search");
    $(selector).removeClass("glyphicon-triangle-bottom");
    $(selector).addClass("glyphicon-ok");
}
function removeIcon(selector) {
    $(selector).removeClass("glyphicon-ok");
    $(selector).removeClass("glyphicon-search");
    $(selector).removeClass("glyphicon-triangle-bottom");
}
function ShowLoader(selector) {

    $(selector).show();
}
function HideLoader(selector) {
    $(selector).hide();
}