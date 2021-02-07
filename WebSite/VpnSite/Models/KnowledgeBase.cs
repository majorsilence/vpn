using System;
using MarkdownDeep;
using Microsoft.AspNetCore.Http;

namespace Majorsilence.Vpn.Site.Models
{
    public class KnowledgeBase
    {
        public KnowledgeBase(string code)
        {
            string appDataPath = System.Web.HttpContext.Current.Server.MapPath("~/assets");
            string readFilePath = "";
            string input = "Something went wrong";

            if (code == "pptpwindows")
            {
                readFilePath = System.IO.Path.Combine(appDataPath, 
                    "knowledgebase", "pptp", "windows.md");
                input = System.IO.File.ReadAllText(readFilePath);
            }
            else if (code == "pptpandroid")
            {
                readFilePath = System.IO.Path.Combine(appDataPath,
                    "knowledgebase", "pptp", "android.md");
                input = System.IO.File.ReadAllText(readFilePath);
            }
            else if (code == "pptpubuntu")
            {
                readFilePath = System.IO.Path.Combine(appDataPath, 
                    "knowledgebase", "pptp", "ubuntu.md");
                input = System.IO.File.ReadAllText(readFilePath);
            }
            else if (code == "openvpnubuntu")
            {
                readFilePath = System.IO.Path.Combine(appDataPath, 
                    "knowledgebase", "openvpn", "ubuntu.md");
                input = System.IO.File.ReadAllText(readFilePath);
            }
            else if (code == "openvpnwindows")
            {
                readFilePath = System.IO.Path.Combine(appDataPath, 
                    "knowledgebase", "openvpn", "windows.md");
                input = System.IO.File.ReadAllText(readFilePath);
            }
            else if (code == "openvpnandroid")
            {
                readFilePath = System.IO.Path.Combine(appDataPath, 
                    "knowledgebase", "openvpn", "android.md");
                input = System.IO.File.ReadAllText(readFilePath);
            }

            var md = new MarkdownDeep.Markdown();
            md.ExtraMode = true;
            md.SafeMode = false;


            _output = md.Transform(input);
        }

        private string _output;

        public string Output
        { 
            get
            {
                return _output;
            }
        }
    }
}

