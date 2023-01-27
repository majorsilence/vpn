using FluentMigrator;

namespace Majorsilence.Vpn.Logic.Migrations;

[Migration(9)]
public class StripePublicKey : Migration
{
    public override void Up()
    {
        Alter.Table("SiteInfo").AddColumn("StripeAPIPublicKey").AsString().Nullable();
        Update.Table("SiteInfo").Set(new { StripeAPIPublicKey = "" }).AllRows();
        Alter.Table("SiteInfo").AlterColumn("StripeAPIPublicKey").AsString().NotNullable();
    }

    public override void Down()
    {
        Delete.Column("StripeAPIPublicKey").FromTable("SiteInfo");
    }
}