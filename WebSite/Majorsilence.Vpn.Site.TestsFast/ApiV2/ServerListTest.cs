using System;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using Moq;
using System.Collections.Specialized;
using Majorsilence.Vpn.Site.TestsFast.MvcFakes;

namespace Majorsilence.Vpn.Site.TestsFast.ApiV2;

public class ServerListTest
{
    public ServerListTest()
    {
    }

    [Test]
    public void TestServerListHappyPath()
    {
        // in an actual desktop app this will need to be setup as static
        var cookieContainer = new System.Net.CookieContainer();

        using (var handler = new System.Net.Http.HttpClientHandler() { CookieContainer = cookieContainer })
        using (var client = new System.Net.Http.HttpClient(handler))
        {
            var header = new NameValueCollection();
            header.Add("VpnAuthToken", Setup.token1);
            header.Add("VpnUserId", Setup.userid.ToString());

            var mock = new Mock<Helpers.ISessionVariables>();
            mock.SetupAllProperties();

            var sessionVars = mock.Object;
            var controller = new Controllers.ApiV2Controller(sessionVars);

            FakeControllerContext.SetContext(controller, header);

            var data = controller.Servers();
            Assert.That(controller.Response.StatusCode, Is.EqualTo((int)System.Net.HttpStatusCode.OK));


            var deserializedContent =
                Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Logic.Accounts.UserServerDetailsInfo>>(
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