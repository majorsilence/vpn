using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Site.Controllers;
using Majorsilence.Vpn.Site.Helpers;
using Majorsilence.Vpn.Site.TestsFast.MvcFakes;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Majorsilence.Vpn.Site.TestsFast.ApiV2;

public class ServerListTest
{
    [Test]
    public void TestServerListHappyPath()
    {
        // in an actual desktop app this will need to be setup as static
        var cookieContainer = new CookieContainer();

        using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
        using (var client = new HttpClient(handler))
        {
            var header = new NameValueCollection();
            header.Add("VpnAuthToken", Setup.token1);
            header.Add("VpnUserId", Setup.userid.ToString());

            var mock = new Mock<ISessionVariables>();
            mock.SetupAllProperties();
            var sessionVars = mock.Object;
            var mockLogger = new Mock<ILogger<ApiV2Controller>>();
            var logger = mockLogger.Object;
            var keysMock = new Mock<IEncryptionKeysSettings>();
            var keys = keysMock.Object;
            var controller = new ApiV2Controller(sessionVars, logger, keys, Setup.DbSettings);

            FakeControllerContext.SetContext(controller, header);

            var data = controller.Servers();
            Assert.That(controller.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));


            var deserializedContent =
                JsonConvert.DeserializeObject<IEnumerable<UserServerDetailsInfo>>(
                    data.Content);

            Assert.That(deserializedContent.Count() > 1);


            var defaultVagrantServer = from a in deserializedContent
                where a.Address == "127.0.0.1" &&
                      a.VpnServerName == "default vagrant testing vpn authority"
                select a;


            Assert.That(defaultVagrantServer.Count() == 1);
        }
    }
}