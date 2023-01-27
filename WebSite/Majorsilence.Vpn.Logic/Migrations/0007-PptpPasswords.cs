using FluentMigrator;

namespace Majorsilence.Vpn.Logic.Migrations;

[Migration(7)]
public class PptpPasswords : Migration
{
    public override void Up()
    {
        Alter.Table("UserPptpInfo").AddColumn("Password").AsString().Nullable();
        Update.Table("UserPptpInfo").Set(new { Password = "" }).AllRows();
        Alter.Table("UserPptpInfo").AlterColumn("Password").AsString().NotNullable();
    }

    public override void Down()
    {
        Delete.Column("Password").FromTable("UserPptpInfo");
    }
}