var NewUser;
(function (NewUser) {
    var fromFrontPage = false;
    var isLiveSite = false;

    function Init(liveSite, fromFrontPage) {
        $("input[type=submit][id='createuser']").button().click(function (event) {
            event.preventDefault();
            NewUser.CreateNewUser();
        });

        NewUser.isLiveSite = false;
        if (liveSite == true) {
            $("#betaKeyText").hide();
            NewUser.isLiveSite = true;
        }

        NewUser.fromFrontPage = false;
        if (fromFrontPage == true) {
            NewUser.fromFrontPage = true;
        }
    }

    NewUser.Init = Init;

    function CreateNewUser() {
        var firstname_val = $("#fname").val();
        var lastname_val = $("#lname").val();
        ;
        var password_val = $("#password").val();
        var confirmPassword = $("#cpassword").val();
        var email_val = $("#email").val();
        var confirmEmail = $("#cemail").val();
        var betakey_val = $("#betaKeyText").val();


        if (NewUser.fromFrontPage) {
            // front page only requires a password and email address.
            // Fill in some temp data until users activate with a credit card
            confirmPassword = password_val;
            confirmEmail = email_val;
            firstname_val = "ಥ_ಥ";
            lastname_val = "ಥ_ಥ";
        }


        if (email_val != confirmEmail) {
            Helpers.ShowMessage("Email address and confirmation email address does not match.", "Email", Helpers.MessageType.Information);
            return;
        }

        if (password_val != confirmPassword) {
            Helpers.ShowMessage("Password and confirmation password does not match.", "Password", Helpers.MessageType.Information);
            return;
        }


        if ((betakey_val == "") && (NewUser.isLiveSite == false)) {
            Helpers.ShowMessage("A beta key must be entered", "Beta Key", Helpers.MessageType.Information);
            return;
        }

        Helpers.ShowLoading();
        $.ajax({
            type: "POST",
            url: "/signup/createuser",
            data: {
                email: email_val,
                emailconfirm: confirmEmail,
                password: password_val,
                passwordconfirm: confirmPassword,
                firstname: firstname_val,
                lastname: lastname_val,
                betakey: betakey_val
            },
            success: function (result) {
                document.location.href = "/accountcreated?status=ok";
            },
            error: function (result) {
                Helpers.HideLoading();
                if (result.status = 400) {
                    Helpers.ShowMessage(result.responseText, "Unknown Error", Helpers.MessageType.Error);
                } else {
                    Helpers.ShowMessage(result.responseText, "Unknown Error", Helpers.MessageType.Error);
                }
            },
            dataType: "html"
        });

    }

    NewUser.CreateNewUser = CreateNewUser;
})(NewUser || (NewUser = {}));
//# sourceMappingURL=newuser.js.map
