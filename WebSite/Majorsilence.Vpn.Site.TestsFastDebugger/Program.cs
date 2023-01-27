using System;
using Majorsilence.Vpn.Site.TestsFast.ApiV2;

namespace Majorsilence.Vpn.Site.TestsFastDebugger;

internal class MainClass
{
    public static void Main(string[] args)
    {
        var x = new Setup();
        try
        {
            x.BringUp();

            var y = new LoginTest();
            y.Setup();

            y.TestLoginHappyPath();

            y.Cleanup();

            var zz = new ServerListTest();
            zz.TestServerListHappyPath();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
        finally
        {
            x.TearDown();
        }

        Console.WriteLine("Hello World!");
    }
}