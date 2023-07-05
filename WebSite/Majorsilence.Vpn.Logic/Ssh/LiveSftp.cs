using System;
using System.IO;
using Renci.SshNet;

namespace Majorsilence.Vpn.Logic.Ssh;

public class LiveSftp : ISftp
{
    private readonly string password;
    private readonly int port;
    private readonly string username;
    private SftpClient client;
    private bool disposed;
    private bool isLoggedIn;

    public LiveSftp(int port, string username, string password)
    {
        this.port = port;
        this.username = username;
        this.password = password;
    }

    public void Login(string host)
    {
        if (isLoggedIn) return;

        client = new SftpClient(host, port, username, password);
        client.Connect();

        isLoggedIn = true;
    }

    public void DownloadFile(string path, Stream output)
    {
        client.DownloadFile(path, output);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
            if (client != null)
            {
                if (client.IsConnected) client.Disconnect();
                client.Dispose();
            }

        disposed = true;
    }
}