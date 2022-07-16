using System;

namespace Majorsilence.Vpn.Site.TestsFastDebugger
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var x = new Majorsilence.Vpn.Site.TestsFast.ApiV2.Setup();
            try
            {
                x.BringUp();

                var y = new Majorsilence.Vpn.Site.TestsFast.ApiV2.LoginTest();
                y.Setup();

                y.TestLoginHappyPath();

                y.Cleanup();

                var zz = new Majorsilence.Vpn.Site.TestsFast.ApiV2.ServerListTest();
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
}
