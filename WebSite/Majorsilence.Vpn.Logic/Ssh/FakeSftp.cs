using System.IO;

namespace Majorsilence.Vpn.Logic.Ssh;

public class FakeSftp : ISftp
{
    public void Login(string host)
    {
    }

    public void DownloadFile(string path, Stream output)
    {
    }

    public void Dispose()
    {
    }
}