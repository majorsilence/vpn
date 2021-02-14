var ValidateResetCode;
(function (ValidateResetCode) {
    function Init() {
        $("input[type=submit][id='validatemyresetcode']").button().click(function (event) {
            event.preventDefault();
            ValidateResetCode.ValidateCode();
        });
    }
    ValidateResetCode.Init = Init;

    function ValidateCode() {
        var codeval = $("#resetCode").val();
        var newpswval = $("#newpsw").val();
        var cnewpswval = $("#cnewpsw").val();

        if (codeval == "") {
            Helpers.ShowMessage("Reset Code must be filled in.", "Reset Code", Helpers.MessageType.Error /* Error */);
            return;
        }

        if (newpswval == "") {
            Helpers.ShowMessage("New password must be filled in.", "Reset Code", Helpers.MessageType.Error /* Error */);
            return;
        }

        if (cnewpswval == "") {
            Helpers.ShowMessage("Confirm password must be filled in.", "Reset Code", Helpers.MessageType.Error /* Error */);
            return;
        }

        if (newpswval != cnewpswval) {
            Helpers.ShowMessage("Password and Confirm Password should match.", "Reset Code", Helpers.MessageType.Error /* Error */);
            return;
        }
        Helpers.ShowLoading();
        $.ajax({
            type: "POST",
            url: "/Generic/ResetCodeValidation",
            data: {
                code: codeval,
                cnewpsw: cnewpswval,
                newpsw: newpswval
            },
            success: function (result) {
                Helpers.ShowMessage("Sucessfull.", "Password Reset", Helpers.MessageType.Success /* Success */);

                Helpers.HideLoading();
            },
            error: function (result) {
                Helpers.HideLoading();
                if (result.status = 403) {
                    Helpers.ShowMessage("Incorrect Reset Code entered.  Try again.", "Password Reset Error", Helpers.MessageType.Error /* Error */);
                } else {
                    Helpers.ShowMessage(result.responseText, "Unknown Error", Helpers.MessageType.Error /* Error */);
                }
            },
            dataType: "html"
        });
    }
    ValidateResetCode.ValidateCode = ValidateCode;
})(ValidateResetCode || (ValidateResetCode = {}));
//# sourceMappingURL=login.js.map
