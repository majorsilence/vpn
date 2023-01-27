using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Majorsilence.Vpn.Site.Helpers;

/// <summary>
/// Invoke javascript Helpers.Init() and Helpers.ShowMessage function from c# code behind files.
/// </summary>
public class Message
{
    public enum MessageType
    {
        Error = 0,
        Success = 1,
        Warning = 2,
        Information = 3
    }

    /// <summary>
    /// Creates a message that can be displayed using sites javascript message helper.  Create a javascript document ready function and
    /// calls Helpers.ShowMessage.
    /// </summary>
    /// <returns>The message.</returns>
    /// <param name="message">Message.</param>
    /// <param name="title">Title.</param>
    /// <param name="mType">M type.</param>
    /// <example>
    /// <code>
    /// var msg = VpnSite.Helpers.Message.CreateMessage("Changes Saved", "Changes Saved", VpnSite.Helpers.Message.MessageType.Information);
    /// @Html.Raw(msg);
    /// </code>
    /// </example>
    public static string CreateMessage(string message, string title, MessageType mType)
    {
        // omg
        var msg = "<script type=\"text/javascript\">$(document).ready(function () {";
        msg += string.Format("Helpers.Init();Helpers.ShowMessage(\"{0}\", \"{1}\", Helpers.MessageType.{2});", message,
            title, mType);
        msg += " });</script>";

        return msg;
    }
}