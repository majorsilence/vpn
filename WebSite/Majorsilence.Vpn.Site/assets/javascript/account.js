var Account;
(function (Account) {
    function Init(isLoggedIn, isPaymentExpired) {
        $("input[type=submit][name='cancelsubscription']").button().click(function (event) {
            event.preventDefault();
            CancelSubscription();

        });

        $("input[type=submit][name='saveprofile']").button().click(function (event) {
            event.preventDefault();
            UpdateProfile();

        });

        $("input[type=submit][name='savepassword']").button().click(function (event) {
            event.preventDefault();
            UpdatePassword();

        });


        if (!isLoggedIn) {
            HidePanelIsLoggedIn();
        } else {
            DisplayPaymentInfo(isPaymentExpired);
        }

        Helpers.TabPillsLoad("accounttabs");


    }

    Account.Init = Init;

    function HidePanelIsLoggedIn() {
        $("#PanelIsLoggedIn").hide();
    }

    function DisplayPaymentInfo(isPaymentExpired) {

        if (isPaymentExpired) {
            $("#PanelCancelPayments").hide();
        } else {
            $("#PanelMakePayment").hide();
        }
    }


    function UpdatePassword() {

        var oldpassword_val = $("#oldpassword").val();
        var newpassword_val = $("#newpassword").val();
        var confirmnewpassword_val = $("#confirmnewpassword").val();

        Helpers.ShowLoading();
        $.ajax({
            type: "POST",
            url: "/account/updatepassword",
            data: {
                oldpassword: oldpassword_val,
                newpassword: newpassword_val,
                confirmnewpassword: confirmnewpassword_val
            },
            success: function (result) {
                Helpers.HideLoading();
                Helpers.ShowMessage(result, "Updated", Helpers.MessageType.Information /* Information */);
            },
            error: function (result) {
                Helpers.HideLoading();
                Helpers.ShowMessage(result.responseText, "Unknown Error", Helpers.MessageType.Error);

            },
            dataType: "html"
        });


    }

    function UpdateProfile() {

        var firstname_val = $("#firstname").val();
        var lastname_val = $("#lastname").val();
        var email_val = $("#email").val();

        Helpers.ShowLoading();
        $.ajax({
            type: "POST",
            url: "/account/updateprofile",
            data: {
                email: email_val,
                firstname: firstname_val,
                lastname: lastname_val
            },
            success: function (result) {
                Helpers.HideLoading();
                Helpers.ShowMessage(result, "Updated", Helpers.MessageType.Information /* Information */);
            },
            error: function (result) {
                Helpers.HideLoading();
                Helpers.ShowMessage(result.responseText, "Unknown Error", Helpers.MessageType.Error);

            },
            dataType: "html"
        });


    }

    function CancelSubscription() {
        Helpers.ShowLoading();
        $.ajax({
            type: "POST",
            url: "/account/cancelsubscription",
            success: function (result) {
                Helpers.HideLoading();
                Helpers.ShowMessage("Your subscription has been cancelled.  You will still have access until the end of your current payment.",
                    "Updated", Helpers.MessageType.Information /* Information */);
            },
            error: function (result) {
                Helpers.HideLoading();
                Helpers.ShowMessage(result.responseText, "Unknown Error", Helpers.MessageType.Error);

            },
            dataType: "html"
        });

    }
})(Account || (Account = {}));
//# sourceMappingURL=setup.js.map
