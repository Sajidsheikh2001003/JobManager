// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function confirmDelete(uniqueId, isDeleteClicked) {
    var deleteSpan = 'deleteSpan_' + uniqueId;
    var confirmDeleteSpan = 'confirmDeleteSpan_' + uniqueId;

    if (isDeleteClicked) {
        $('#' + deleteSpan).hide();
        $('#' + confirmDeleteSpan).show();
    } else {
        $('#' + deleteSpan).show();
        $('#' + confirmDeleteSpan).hide();
    }
}

function inlineModel(uniqueId, isModelClicked) {
    var parentDiv = 'parentDiv_' + uniqueId;
    var childDiv = 'childDiv_' + uniqueId;

    if (isModelClicked) {
        $('#' + parentDiv).hide();
        $('#' + childDiv).show();
    } else {
        $('#' + parentDiv).show();
        $('#' + childDiv).hide();
    }
}




function Print2Printer(printdivname, title, stylesheet, customStyle) {
    var contents = $("#" + printdivname).html();
    var frame1 = $('<iframe />');
    frame1[0].name = "frame1";
    frame1.css({ "position": "absolute", "top": "-1000000px" });
    $("body").append(frame1);
    var frameDoc = frame1[0].contentWindow ? frame1[0].contentWindow : frame1[0].contentDocument.document ? frame1[0].contentDocument.document : frame1[0].contentDocument;
    frameDoc.document.open();
    //Create a new HTML document.
    frameDoc.document.write('<html><head><title>' + title + '</title>');
    if (customStyle != "") {
        frameDoc.document.write(customStyle);
    }


    frameDoc.document.write('</head><body class="stretched"><div id="wrapper" class="clearfix">');
    //Append the external CSS file.
    if (stylesheet != "") {
        //frameDoc.document.write('<link href="' + stylesheetpath + '" rel="stylesheet" type="text/css" />');
        frameDoc.document.write(stylesheet);
    }
    //Append the DIV contents.
    frameDoc.document.write(contents);
    frameDoc.document.write("</div>");
    //Append Scripts

    frameDoc.document.write('<script src="/Canvas/HTML/js/jquery.js"></script><script src="/Canvas/HTML/js/plugins.min.js"></script>');

    frameDoc.document.write('</body></html>');
    frameDoc.document.close();
    setTimeout(function () {
        window.frames["frame1"].focus();
        window.frames["frame1"].print();
        frame1.remove();
    }, 500);
}







function ConvertToDate(date) {

    let dt = new Date(date)
    var dd = String(dt.getDate()).padStart(2, '0');
    var mm = String(dt.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = dt.getFullYear();
    var dateFinal = dd + "/" + mm + "/" + yyyy;
    return dateFinal;
}
// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
