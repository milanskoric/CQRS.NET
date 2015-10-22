function alert_init() {
    var el = $('.alert-overlay');
    if (!el || !el.html()) {
        var strHtml = '<div class="alert-overlay"><div class="alert-box"></div></div>';
        $('body').append(strHtml);
        el = $('.alert-overlay');
    }
    return el;
}
function alertshow(type, sHeader, sMessage, sHtml, fConfirm) {
    var overlay = alert_init();
    var box = overlay.find('.alert-box');
    if (!type)
        type = 1;
    if (!sMessage)
        sMessage = "";
 
    if (!sHeader){
        if (type == 0 || type == "Info" || type == "0")
            sHeader = "Info";
        else if (type == 1 || type == "1")
            sHeader = "Successful";
        else if (type == 2 || type == "Error" || type == "2")
            sHeader = "Error";
        else if (type == 3 || type == "3")
            sHeader = "Warning";
        else
            sHeader = null;
    }
     
    var css = "alert-box-info";

    if (type == 1 || type == "1")
        css = "alert-box-success";
    else if (type == 2 || type == "Error" || type == "2")
        css = "alert-box-danger";
    else if (type == 3 || type == "3")
        css = "alert-box-warning";
     
 
    var iHtml = '<div class="container"><div class="row"><div class="col-md-8 col-md-offset-2">';

    if (!sHeader)
        iHtml = iHtml + '<h1>' + sMessage + '</h1>';
    else
        iHtml = iHtml + '<h1>' + sHeader + '</h1>' + '<h2>' + sMessage + '</h2>';

    if (sHtml)
        iHtml = iHtml + sHtml;

    iHtml = iHtml + '<div class="alert-actions">';

    iHtml = iHtml + '<button class="alert-box-ok" type="button">Ok</button> ';

    iHtml = iHtml + '</div>';

    iHtml = iHtml + '</div></div></div>';

    box.html('');
    box.append(iHtml);
    box.removeClass("alert-box-success");
    box.removeClass("alert-box-danger");
    box.removeClass("alert-box-info");
    box.removeClass("alert-box-warning");
    overlay.addClass("show");
    box.addClass(css);
    box.fadeIn(600);

    $(".alert-box").click(function () {
        overlay.removeClass("show");
        box.fadeOut(500);
    });

    $(".alert-box-ok").click(function () {
        if (fConfirm && (typeof fConfirm == "function")) {
            try {
                fConfirm();
            } catch (e) {

            }
        }
        overlay.removeClass("show");
        box.fadeOut(500);
    });
}
