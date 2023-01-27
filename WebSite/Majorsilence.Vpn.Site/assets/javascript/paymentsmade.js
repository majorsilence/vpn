var PaymentsMade;
(function (PaymentsMade) {
    function Init() {
        $("#PaymentSatus").html("Pending.....");

        var payid = Helpers.GetQueryStringParams("PayerID");
        var token = Helpers.GetQueryStringParams("token");

        $.ajax({
            type: "POST",
            url: "/setup/makepayments.ashx",
            data: {PayerID: Helpers.EncodeURIC(payid), Token: Helpers.EncodeURIC(token)},
            success: function (result) {
                if (result) {
                    $("#PaymentSatus").html("Payment Success...");
                }
            },
            error: function (result) {
                $("#PaymentSatus").html("Payment Fail...");
                Helpers.HideLoading();
                Helpers.ShowMessage(result.responseText, "Unknown Error", Helpers.MessageType.Error);
            },
            dataType: "html"
        });

    }

    PaymentsMade.Init = Init;
})(PaymentsMade || (PaymentsMade = {}));