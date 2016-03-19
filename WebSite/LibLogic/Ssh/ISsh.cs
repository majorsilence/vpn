using System;
using System.IO;

namespace LibLogic.Ssh
{
    public interface ISsh : IDisposable
    {

        void Login(string host);

        void WriteLine(string value);

        string Read();

    }
}

