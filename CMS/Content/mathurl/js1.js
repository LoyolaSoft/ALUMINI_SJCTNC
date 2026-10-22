//<![CDATA[

var t;
var lastlatex;
var latexhasfocus;
var maxlen;

function do_delay(func, delay) {
    clearTimeout(t);  // should be okay even if t is invalid
    if (delay > 0) {
        t = setTimeout(func, delay);
    } else {
        func();
    }
}
function latex_preview() {
    $('#loader').css("display", "block"); // since it takes some time to load
    var modestring = "";
    if (document.getElementById('mode').value == "text") {
        modestring = "\\textmode ";
    } else if (document.getElementById('mode').value == "inline") {
        modestring = "\\inlinemode ";
    }
    document.getElementById('image').src = "http://mathurl.com/render.cgi?" + escape(modestring + document.getElementById('latex').value + "\\nocache");
}
function delay_preview() {
    $('#loader').css("display", "block");
    do_delay(function () {
        latex_preview()
    }, 700);
}
new function ($) { // http://stackoverflow.com/questions/499126/jquery-set-cursor-position-in-text-area
    $.fn.setCursorPosition = function (pos) {
        if ($(this).get(0).setSelectionRange) {
            $(this).get(0).setSelectionRange(pos, pos);
        } else if ($(this).get(0).createTextRange) {
            var range = $(this).get(0).createTextRange();
            range.collapse(true);
            range.moveEnd('character', pos);
            range.moveStart('character', pos);
            range.select();
        }
    }
}(jQuery);
new function ($) {
    $.fn.replaceSelection = function (text) {
        if ($(this)[0].setSelectionRange) {
            $(this)[0].value = $(this)[0].value.substr(0, $(this)[0].selectionStart)
                               + text
                               + $(this)[0].value.substr($(this)[0].selectionEnd, $(this)[0].value.length);
        } else if ($(this)[0].createTextRange) {
            $(this)[0].focus();
            document.selection.createRange().text = text;
        }
    }
}(jQuery);


$(document).ready(function () {

    $('#image').src = "https://mathurl.com/5euwuy.png";

    $('#latexbuttons').css("display", "block");
   
    $('#latexbuttons').accordion({ header: "div.header", autoHeight: false, collapsible: true, active: false });

    $('#preview').onclick = latex_preview;
    $('#preview').type = "button";

    $('#loader').css("display", "block");
    $('#image').load(function () {
        $('#loader').css("display", "none");
        if (latexhasfocus) { // a hack to make safari redraw
            $('#container').css("padding-top", "1px");
        }
    });

    $('#latexbuttons .contents a').click(function (event) {
        event.preventDefault();
        var start = $('#latex')[0].selectionStart;
        var text = $(this).attr("title");
        var offset = text.search(/\{\}/) + 1;	// search for empty {} or amp
        if (offset == 0) offset = text.search(/ \&/);
        if (offset == 0) offset = text.search(/\$\$/);
        if (offset <= 0) offset = text.length;
        $('#latex').replaceSelection(text);
        $('#latex').focus();
        $('#latex')[0].setSelectionRange(start + offset, start + offset);
        // see http://groups.google.com/group/jquery-en/browse_thread/thread/febb1c9927b33d85
        delay_preview();
    });
    $('#latexbuttons .header').click(function (e) {
        $('#latex').focus();
    });

     $('#latexbuttons .header').mouseup(function (e) {
        var c= $(this).attr("aria-controls");
        $('#' + c + '').css("height", "200px");
    });


    $('#clear').click(function (e) {
        lastlatex = "";
        $('#latex').val('');
        $('#image').attr("src", "http://mathurl.com//images/noinput.png");
        $('#latex').focus();
    });
    lastlatex = $('#latex').val();
    maxlen = 8000;
    $('#latex').keyup(function (e) {
        if ($('#latex').val() != lastlatex) {
            if ($('#latex').val().length > maxlen) {
                $('#latex').val($('#latex').val().substring(0, maxlen));
                alert('Exceeded mathURL input size');
            }
            lastlatex = $('#latex').val();
            delay_preview();
        }
    });
    $('#latex').focus(function () { latexhasfocus = 1; });
    $('#latex').blur(function () { latexhasfocus = 0; });
    $('#latex').focus();
});
//]]>