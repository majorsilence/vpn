﻿using System;
using System.IO;

namespace LibLogic.Ssh
{
    public class LiveSsh : ISsh
    {

        Renci.SshNet.SshClient client = null;
        Renci.SshNet.ShellStream stream = null;
        StreamReader reader;
        StreamWriter writer;
        int port;
        string username;
        string password;
        bool isLoggedIn = false;
        bool disposed = false;

        public LiveSsh(int port, string username, string password)
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

            client = new Renci.SshNet.SshClient(host, port, username, password);
            client.Connect();

            stream = client.CreateShellStream("xterm", 80, 24, 800, 600, 1024);

            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            isLoggedIn = true;
        }

        public void WriteLine(string value)
        {
            writer.WriteLine(value);
        }

        public string Read()
        {
            return reader.ReadToEnd();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); 
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return; 

            if (disposing)
            {

                if (writer != null)
                {
                    writer.Flush();
                }
                if (reader != null)
                {
                    reader.Dispose();
                }

                if (stream != null)
                {
                    stream.Dispose();
                }
          
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

