using System;

namespace Majorsilence.Vpn.Site.TestsFastDebugger;

internal class MainClass
{
    public static void Main(string[] args)
    {
        var x = new TestsFast.ApiV2.Setup();
        try
        {
            x.BringUp();

            var y = new TestsFast.ApiV2.LoginTest();
            y.Setup();

            y.TestLoginHappyPath();

            y.Cleanup();

            var zz = new TestsFast.ApiV2.ServerListTest();
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