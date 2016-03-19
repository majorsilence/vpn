using System;
using FluentMigrator;
using FluentMigrator.Runner.Extensions;

namespace LibLogic.Migrations
{
    [Migration(9, TransactionBehavior.Default)]
    public class StripePublicKey : Migration
    {
        public StripePublicKey()
        {
        }

        public override void Up()
        {

            Alter.Table("SiteInfo").AddColumn("StripeAPIPublicKey").AsString().Nullable();
            Update.Table("SiteInfo").Set(new { StripeAPIPublicKey = ""}).AllRows();
            Alter.Table("SiteInfo").AlterColumn("StripeAPIPublicKey").AsString().NotNullable();

        }

        public override void Down()
        {
            Delete.Column("StripeAPIPublicKey").FromTable("SiteInfo");
        }

    }
}

