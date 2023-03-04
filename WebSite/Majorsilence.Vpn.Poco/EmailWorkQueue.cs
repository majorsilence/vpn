using System;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Poco;

[Table("EmailWorkQueue")]
public class EmailWorkQueue
{
    [Key] 
    public ulong Id { get; init; }
    public string SendTo { get; init; }
    public string PlainTextMessage { get; init; }
    public string HtmlMessage { get; init; }
    public string Subject { get; init; }
    public DateTime ToSendDateUtc { get; init; }
    public DateTime? SentDateTimeUtc { get; set; }
    public byte[]? Attachment { get; init; }
}