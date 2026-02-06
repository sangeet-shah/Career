function SetActiveMenu(menuId) {
    setCookie("active-menu-name", menuId);
}

function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}
function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}
function eraseCookie(name) {
    document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}

$(document).ready(function () {
    // active menu script
    $("." + getCookie("active-menu-name")).addClass("active-link");
});

function DisplayAjaxLoading() {    
    document.getElementById("loader").style.display = 'block';
}

function StopAjaxLoading() {
    document.getElementById("loader").style.display = 'none';
}

function CallGtagEventShare(eventName, methodName, contentType, itemId) {
    gtag('event', eventName, { 'method': methodName, 'content_type': contentType, 'item_id': itemId });
}
function CallGtagEventPromotion(eventName, creativeName, creativeSlot, promotionId, promotionName) {
    gtag('event', eventName, {
        'creative_name': creativeName,
        'creative_slot': creativeSlot,
        'promotion_id': promotionId,
        'promotion_name': promotionName
    });
}