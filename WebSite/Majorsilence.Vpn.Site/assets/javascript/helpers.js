var Helpers;
(function (Helpers) {
    function Init() {

        Helpers.PreloadImage('/assets/images/busy.gif');
        $("#messagedialog").dialog({
            autoOpen: false
        });
        $("#loadingdialog").dialog({
            autoOpen: false
        });

        Helpers.LanguageChanger();

    }

    Helpers.Init = Init;

    function LanguageChanger() {
        $("#lanSelect").change(function (e) {
            var val = this.value;
            Helpers.CreateCookie("lang", val);
            window.location.reload(true);
        });
        var langval = Helpers.ReadCookie("lang");
        if (langval) {
            $("#lanSelect").val(langval);
        }
    }

    Helpers.LanguageChanger = LanguageChanger;

    /*! Copyright 2011, Ben Lin (http://dreamerslab.com/)
	* Licensed under the MIT License (LICENSE.txt).
	*
	* Version: 1.0.3
	*
	* Requires: jQuery 1.2.3+
	* See https://github.com/dreamerslab/jquery.preload/commit/970d0da05f8702da0a9c03d8dce8e76d3a3530dc#diff-081323b85ef993b51856a991e5e899eb
	*/
    function PreloadImage(arguments) {

        var imgs = Object.prototype.toString.call(arguments[0]) === '[object Array]'
            ? arguments[0] : arguments;

        var tmp = [];
        var i = imgs.length;

        // reverse loop run faster
        for (; i--;) tmp.push($('<img />').attr('src', imgs[i]));


    }

    Helpers.PreloadImage = PreloadImage;

    function ShowLoading() {
        // show loading div from master template

        $.blockUI({message: '<h1 style="display: inline;"><img src="/assets/images/busy.gif" style="vertical-align:middle;" /> Just a moment...</h1>'});

    }

    Helpers.ShowLoading = ShowLoading;

    function HideLoading() {
        // hide loading div from master template
        $.unblockUI();

    }

    Helpers.HideLoading = HideLoading;

    function ShowMessage(msg, title, type) {

        if (type == Helpers.MessageType.Error /* Error */) {

            $('#message').notify({
                message: {html: msg}, type: "danger"
            }).show();

        } else if (type == Helpers.MessageType.Success /* Success */) {
            $('#message').notify({
                message: {html: msg},
                type: "success"
            }).show();
        } else if (type == Helpers.MessageType.Warning /* Warning */) {
            $('#message').notify({
                message: {html: msg}, type: "warning"
            }).show();
        } else if (type == Helpers.MessageType.Information /* Information */) {
            $('#message').notify({
                message: {html: msg}, type: "info"
            }).show();
        }

    }

    Helpers.ShowMessage = ShowMessage;

    (function (MessageType) {
        MessageType[MessageType["Error"] = 0] = "Error";
        MessageType[MessageType["Success"] = 1] = "Success";
        MessageType[MessageType["Warning"] = 2] = "Warning";
        MessageType[MessageType["Information"] = 3] = "Information";
    })(Helpers.MessageType || (Helpers.MessageType = {}));
    var MessageType = Helpers.MessageType;

    // Helper functions to Encode the URI
    function EncodeURIC(str) {
        return encodeURIComponent(str);
    }

    Helpers.EncodeURIC = EncodeURIC;

    // Helper functions to Decode the URI
    function DecodeURIC(str) {
        return decodeURIComponent(str.toString());
    }

    Helpers.DecodeURIC = DecodeURIC;

    function GetQueryStringParams(sParam) {
        var results = new RegExp('[\\?&]' + sParam + '=([^&#]*)').exec(window.location.href);
        if (!results) {
            return 0;
        }
        return results[1] || 0;
    }

    Helpers.GetQueryStringParams = GetQueryStringParams;

    function ReadCookie(name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) {
                return c.substring(nameEQ.length, c.length);
            }
        }
        return null;
    }

    Helpers.ReadCookie = ReadCookie;

    function CreateCookie(name, value, days) {
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            var expires = "; expires=" + date.toGMTString();
        } else
            var expires = "";
        document.cookie = name + "=" + value + expires + "; path=/";
    }

    Helpers.CreateCookie = CreateCookie;

    function TabPillsLoad(ulId) {

        // See http://stackoverflow.com/questions/18999501/bootstrap-3-keep-selected-tab-on-page-refresh
        // and https://github.com/twbs/bootstrap/issues/2415
        // See http://redotheweb.com/2012/05/17/enable-back-button-handling-with-twitter-bootstrap-tabs-plugin.html for history support

        $('#' + ulId + ' a').click(function (e) {
            e.preventDefault();
            history.pushState(null, null, $(this).attr('href'));
            $(this).tab('show');
        });

        // store the currently selected tab in the hash value
        $("ul.nav-pills > li > a").on("shown.bs.tab", function (e) {
            var id = $(e.target).attr("href").substr(1);
            window.location.hash = id;
            window.scrollTo(0, 0);
        });

        // on load of the page: switch to the currently selected tab
        var hash = window.location.hash;
        $('#' + ulId + ' a[href="' + hash + '"]').tab('show');

        // navigate to a tab when the history changes
        window.addEventListener("popstate", function (e) {
            var activeTab = $('[href=' + location.hash + ']');
            if (activeTab.length) {
                activeTab.tab('show');
            } else {
                $('.nav-tabs a:first').tab('show');
            }
        });
    }

    Helpers.TabPillsLoad = TabPillsLoad;

})(Helpers || (Helpers = {}));
//# sourceMappingURL=helpers.js.map
