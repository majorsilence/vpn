var PasswordReset;
(function (PasswordReset) {
    function Init() {
        $("input[type=submit][id='resetmypassword']").button().click(function (event) {
            event.preventDefault();
            PasswordReset.ResetPassword();
        });
    }

    PasswordReset.Init = Init;

    function ResetPassword() {
        var email = $("#email").val();

        if (email == "") {
            Helpers.ShowMessage("Email address must be filled in.", "Email Address", Helpers.MessageType.Error /* Error */);
            return;
        }
        Helpers.ShowLoading();
        $.ajax({
            type: "POST",
            url: "/Generic/ResetPasswordSend",
            data: {username: email},
            success: function (result) {
                if (result.status = 250) {
                    document.location.href = "/validatecode";
                } else {
                    document.location.href = "/";
                }
                Helpers.HideLoading();
            },
            error: function (result) {
                Helpers.HideLoading();
                if (result.status = 403) {
                    Helpers.ShowMessage("Incorrect username entered.  Try again.", "Password Reset Error", Helpers.MessageType.Error /* Error */);
                } else {
                    Helpers.ShowMessage(result.responseText, "Unknown Error", Helpers.MessageType.Error /* Error */);
                }
            },
            dataType: "html"
        });
    }

    PasswordReset.ResetPassword = ResetPassword;
})(PasswordReset || (PasswordReset = {}));
//# sourceMappingURL=login.js.map
