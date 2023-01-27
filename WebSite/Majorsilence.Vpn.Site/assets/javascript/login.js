var Login;
(function (Login) {
    function Init() {
        $("input[type=submit][id='logmein']").button().click(function (event) {
            event.preventDefault();
            Login.LogmeIn();
        });
        $("input[type=submit][id='logmein2']").button().click(function (event) {
            event.preventDefault();
            Login.LogmeIn();
        });
    }

    Login.Init = Init;

    function LogmeIn() {
        var email = "";
        var password_val = "";
        if (location.pathname == "/login") {
            email = $("#email_login2").val();
            password_val = $("#password_login2").val();
        } else {
            email = $("#email_login").val();
            password_val = $("#password_login").val();
        }

        if (email == "") {
            Helpers.ShowMessage("Email address must be filled in.", "Email Address", Helpers.MessageType.Error /* Error */);
            return;
        }
        if (password_val == "") {
            Helpers.ShowMessage("Password address must be filled in.", "Password", Helpers.MessageType.Error /* Error */);
            return;
        }

        Helpers.ShowLoading();
        $.ajax({
            type: "POST",
            url: "/Generic/LoginValidation",
            data: {
                username: email,
                password: password_val
            },
            success: function (result) {
                if (result.status == 250) {
                    document.location.href = "/account#billing";
                } else {
                    document.location.href = "/setup";
                }
            },
            error: function (result) {
                Helpers.HideLoading();
                if (result.status == 403) {
                    Helpers.ShowMessage("Incorrect username or password entered.  Try again or <b><a href=\"/resetpassword?email=" + encodeURI(email) +
                        "\">Reset Password</a></b>",
                        "Login Error", Helpers.MessageType.Error /* Error */);
                } else {
                    Helpers.ShowMessage(result.responseText, "Unknown Error", Helpers.MessageType.Error /* Error */);
                }
            },
            dataType: "html"
        });
    }

    Login.LogmeIn = LogmeIn;
})(Login || (Login = {}));
//# sourceMappingURL=login.js.map
