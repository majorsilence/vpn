using FluentMigrator;

namespace Majorsilence.Vpn.Logic.Migrations;

[Migration(6)]
public class BetaKeyIsSent : Migration
{
    public override void Up()
    {
        Alter.Table("BetaKeys").AddColumn("IsSent").AsBoolean().WithDefaultValue(false).NotNullable();
    }

    public override void Down()
    {
        Delete.Column("IsSent").FromTable("BetaKeys");
    }
}