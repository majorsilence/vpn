using System;
using System.IO;

namespace Majorsilence.Vpn.Logic.Ssh;

public interface ISsh : IDisposable
{
    void Login(string host);

    void WriteLine(string value);

    string Read();
}