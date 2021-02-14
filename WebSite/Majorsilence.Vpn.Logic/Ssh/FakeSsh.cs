﻿using System;
using System.IO;

namespace LibLogic.Ssh
{
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

        TestingScenerios scenerios = TestingScenerios.None;

        public void Login(string host)
        {
         
        }

        public void WriteLine(string value)
        {
        }

        public string Read()
        {

            System.Console.WriteLine("Scenerio: " + scenerios.ToString());

            if (scenerios == TestingScenerios.OpenVpnHappyPath)
            {
              
                return "certificate is to be certified until";
                
            }
            else if (scenerios == TestingScenerios.OpenVpnErrorNumber2)
            {

                return "txt_db error number 2";
            }
           
            return "";
        }

        public void Dispose()
        {

        }
    }
}

