using System;
using System.IO;

namespace Majorsilence.Vpn.Logic.Ssh;

public class FakeSsh : ISsh
{
    public FakeSsh()
    {
    }

    public FakeSsh(TestingScenerios scenerios)
    {
        this.scenerios = scenerios;
    }

    public enum TestingScenerios
    {
        None = 0,
        OpenVpnHappyPath = 1,
        OpenVpnErrorNumber2 = 2
    }

    private TestingScenerios scenerios = TestingScenerios.None;

    public void Login(string host)
    {
    }

    public void WriteLine(string value)
    {
    }

    public string Read()
    {
        Console.WriteLine("Scenerio: " + scenerios.ToString());

        if (scenerios == TestingScenerios.OpenVpnHappyPath)
            return "certificate is to be certified until";
        else if (scenerios == TestingScenerios.OpenVpnErrorNumber2) return "txt_db error number 2";

        return "";
    }

    public void Dispose()
    {
    }
}