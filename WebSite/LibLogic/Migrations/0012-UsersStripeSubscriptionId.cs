using System;
using FluentMigrator;
using FluentMigrator.Runner.Extensions;

namespace LibLogic.Migrations
{
    [Migration(12, TransactionBehavior.Default)]
    public class UsersStripeSubscriptionId : Migration
    {
        public UsersStripeSubscriptionId()
        {
        }

        public override void Up()
        {

            Alter.Table("Users").AddColumn("StripeSubscriptionId").AsString(255).Nullable();
            Update.Table("Users").Set(new { StripeSubscriptionId = ""}).AllRows();
            Alter.Table("Users").AlterColumn("StripeSubscriptionId").AsString(255).NotNullable();

        }

        public override void Down()
        {
            Delete.Column("StripeSubscriptionId").FromTable("Users");
        }

    }
}

