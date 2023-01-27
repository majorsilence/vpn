using NUnit.Framework;

namespace Majorsilence.Vpn.Site.TestsFast.ApiV2;
// See http://stephenwalther.com/archive/2008/07/01/asp-net-mvc-tip-12-faking-the-controller-context

[TestFixture]
public class LoginTest
{
    [SetUp]
    public void Setup()
    {
    }

    [TearDown]
    public void Cleanup()
    {
    }


    // TODO: split this code into a helper class.  Refactor test.  Give it a good name.
    /*
    [Test]
    public async void Test1()
    {
        // in an actual desktop app this will need to be setup as static
        var cookieContainer = new System.Net.CookieContainer();

        using (var handler = new System.Net.Http.HttpClientHandler() { CookieContainer = cookieContainer })
        using (var client = new System.Net.Http.HttpClient(handler))
        {

            var byteArray = System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", emailAddress, password));
            var header = new System.Net.Http.Headers.AuthenticationHeaderValue(
                             "Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Authorization = header;

            var response = await client.PostAsync("http://localhost:8080/apiv2/auth", null);
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode == System.Net.HttpStatusCode.OK);

            var response2 = await client.PostAsync("http://localhost:8080/apiv2/servers", null);
            var responseString2 = await response.Content.ReadAsStringAsync();
            Assert.That(response2.StatusCode == System.Net.HttpStatusCode.OK);
            Assert.That(responseString2 == "hi");

        }
    }
    */

    [Test]
    public void TestLoginHappyPath()
    {
        /*
            var response2 = await client.PostAsync("http://localhost:8080/apiv2/servers", null);
            var responseString2 = await response.Content.ReadAsStringAsync();
            Assert.That(response2.StatusCode == System.Net.HttpStatusCode.OK);
            Assert.That(responseString2 == "hi");
            */
    }
}