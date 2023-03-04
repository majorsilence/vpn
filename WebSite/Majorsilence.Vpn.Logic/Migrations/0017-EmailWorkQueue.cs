using FluentMigrator;

namespace Majorsilence.Vpn.Logic.Migrations;

[Migration(17)]
public class EmailWorkQueue : Migration
{
    public override void Up()
    {
        Create.Table("EmailWorkQueue")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("SendTo").AsCustom("Text").NotNullable()
            .WithColumn("PlainTextMessage").AsCustom("TEXT").NotNullable()
            .WithColumn("HtmlMessage").AsCustom("TEXT").NotNullable()
            .WithColumn("Subject").AsCustom("TEXT").NotNullable()
            .WithColumn("ToSendDateUtc").AsDateTime().NotNullable()
            .WithColumn("SentDateTimeUtc").AsDateTime().Nullable()
            .WithColumn("Attachment").AsBinary().Nullable();
    }

    public override void Down()
    {
        Delete.Table("EmailWorkQueue");
    }
}