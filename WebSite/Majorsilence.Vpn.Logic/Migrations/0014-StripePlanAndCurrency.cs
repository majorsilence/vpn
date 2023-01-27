using FluentMigrator;

namespace Majorsilence.Vpn.Logic.Migrations;

[Migration(14)]
public class StripePlanAndCurrency : Migration
{
    public override void Up()
    {
        Alter.Table("SiteInfo").AddColumn("StripePlanId").AsString(255).Nullable();
        Update.Table("SiteInfo").Set(new { StripePlanId = "monthly5" }).AllRows();
        Alter.Table("SiteInfo").AlterColumn("StripePlanId").AsString(255).NotNullable();

        Alter.Table("SiteInfo").AddColumn("Currency").AsString(255).Nullable();
        Update.Table("SiteInfo").Set(new { Currency = "CAD" }).AllRows();
        Alter.Table("SiteInfo").AlterColumn("Currency").AsString(255).NotNullable();
    }

    public override void Down()
    {
        Delete.Column("StripePlanId").FromTable("SiteInfo");
        Delete.Column("Currency").FromTable("SiteInfo");
    }
}