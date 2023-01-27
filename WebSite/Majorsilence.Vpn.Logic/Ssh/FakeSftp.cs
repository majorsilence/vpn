using System;

namespace Majorsilence.Vpn.Logic.Ssh;

public class FakeSftp : ISftp
{
    public FakeSftp()
    {
    }


    public void Login(string host)
    {
    }

    public void DownloadFile(string path, System.IO.Stream output)
    {
    }

    public void Dispose()
    {
    }
}