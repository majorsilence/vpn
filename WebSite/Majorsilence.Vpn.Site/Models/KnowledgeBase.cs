using System.IO;
using MarkdownDeep;

namespace Majorsilence.Vpn.Site.Models;

public class KnowledgeBase
{
    public KnowledgeBase(string code, string appDataPath)
    {
        var readFilePath = "";
        var input = "Something went wrong";

        if (code == "pptpwindows")
        {
            readFilePath = Path.Combine(appDataPath,
                "knowledgebase", "pptp", "windows.md");
            input = File.ReadAllText(readFilePath);
        }
        else if (code == "pptpandroid")
        {
            readFilePath = Path.Combine(appDataPath,
                "knowledgebase", "pptp", "android.md");
            input = File.ReadAllText(readFilePath);
        }
        else if (code == "pptpubuntu")
        {
            readFilePath = Path.Combine(appDataPath,
                "knowledgebase", "pptp", "ubuntu.md");
            input = File.ReadAllText(readFilePath);
        }
        else if (code == "openvpnubuntu")
        {
            readFilePath = Path.Combine(appDataPath,
                "knowledgebase", "openvpn", "ubuntu.md");
            input = File.ReadAllText(readFilePath);
        }
        else if (code == "openvpnwindows")
        {
            readFilePath = Path.Combine(appDataPath,
                "knowledgebase", "openvpn", "windows.md");
            input = File.ReadAllText(readFilePath);
        }
        else if (code == "openvpnandroid")
        {
            readFilePath = Path.Combine(appDataPath,
                "knowledgebase", "openvpn", "android.md");
            input = File.ReadAllText(readFilePath);
        }

        var md = new Markdown();
        md.ExtraMode = true;
        md.SafeMode = false;


        Output = md.Transform(input);
    }

    public string Output { get; }
}