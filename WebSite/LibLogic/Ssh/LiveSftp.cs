using System;

namespace LibLogic.Ssh
{
    public class LiveSftp : ISftp
    {
        int port;
        string username;
        string password;
        bool isLoggedIn = false;
        Renci.SshNet.SftpClient client;
        bool disposed = false;

        public LiveSftp(int port, string username, string password)
        {
            this.port = port;
            this.username = username;
            this.password = password;
        }

        public void Login(string host)
        {
            if (isLoggedIn)
            {
                return;
            }

            client = new Renci.SshNet.SftpClient(host, port, username, password);
            client.Connect();

            isLoggedIn = true;
        }

        public void DownloadFile(string path, System.IO.Stream output)
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
            {
                if (client != null)
                {
                    if (client.IsConnected)
                    {
                        client.Disconnect();
                    }
                    client.Dispose();
                }
            }

            disposed = true;
        }
            
    }
}

