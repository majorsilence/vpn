using System;
using System.IO;

namespace Majorsilence.Vpn.Logic.Ssh
{
    public interface ISftp : IDisposable
    {
        void Login(string host);

        void DownloadFile(string path, Stream output);
    }
}

