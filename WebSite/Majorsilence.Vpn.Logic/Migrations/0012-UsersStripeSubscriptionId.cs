using FluentMigrator;

namespace Majorsilence.Vpn.Logic.Migrations;

[Migration(12)]
public class UsersStripeSubscriptionId : Migration
{
    public override void Up()
    {
        Alter.Table("Users").AddColumn("StripeSubscriptionId").AsString(255).Nullable();
        Update.Table("Users").Set(new { StripeSubscriptionId = "" }).AllRows();
        Alter.Table("Users").AlterColumn("StripeSubscriptionId").AsString(255).NotNullable();
    }

    public override void Down()
    {
        Delete.Column("StripeSubscriptionId").FromTable("Users");
    }
}