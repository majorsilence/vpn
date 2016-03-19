
var Setup;
(function (Setup) {
    function Init(isLoggedIn, isActiveAccount) {
    
    	
    
        $("input[type=submit][id='ButtonChangeVpnServer']").button().click(function (event) {
            event.preventDefault();

            // Do click stuff here
            var buttonName = $(this).attr("name");
            if (buttonName == "savecert") {
                SaveUserCert();
            } else {
                throw new Error();
            }
        });

		ConfigAccountDivs(isLoggedIn, isActiveAccount);

    }
    Setup.Init = Init;

	function ConfigAccountDivs(isLoggedIn, isActiveAccount)
	{
	
		if(!isLoggedIn)
		{
			$("#vpnaccountcreation").hide();
			return;
		}
	
	
		if(isActiveAccount)
		{
			$("#inactiveaccount").hide();		
		}
		else
		{
			$("#activeaccount").hide();
		}
	
	}

    function SaveUserCert(password) {

        var serverid = $('#SelectVpnServer option:selected').val();
        var serverName = $('#SelectVpnServer option:selected').text();

    
        Helpers.ShowLoading();
        $.ajax({
            type: "POST",
            url: "/saveuservpnserver",
            data: { VpnId: serverid, UserVpnPassword: encodeURI(password) },
            success: function (result) {
                Helpers.HideLoading();
                if (result) {
                    // $("#paymentApprove").show();
                    //$("#paymentApprove").load(result).dialog({ modal: true });

                    Helpers.ShowMessage("VPN Server Changed", "Vpn Saved", Helpers.MessageType.Information);
                    $("#currentvpnserver").html(serverName);
                    
                    var json = JSON.parse(result);
                    
                    $("#currentvpnserverip").html(json.PptpIP);
					$("#currentvpnserverpassword").html(json.PptpPassword);


                }
            },
            error: function (result) {
                Helpers.HideLoading();
                Helpers.ShowMessage(result.responseText, "Unknown Error", Helpers.MessageType.Error);
            },
            dataType: "html"
        });

    }

})(Setup || (Setup = {}));
//# sourceMappingURL=setup.js.map
