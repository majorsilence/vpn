using System;

namespace SiteTests;

internal class MainClass
{
    public static void Main(string[] args)
    {
        //var ssh = new Tests.SshTest();
        //ssh.CreateCert();

        //var daily = new Tests.DailyProcessingTest();
        //daily.Start();

        //var ssh = new Tests.PPTPServerTest();
        //ssh.CreatePPT();


        var test = new Setup();
        test.BringUp();
        test.TearDown();
    }
}