using System;
using System.IO;

namespace LibLogic.Ssh
{
    public interface ISftp : IDisposable
    {
        void Login(string host);

        void DownloadFile(string path, Stream output);
    }
}

