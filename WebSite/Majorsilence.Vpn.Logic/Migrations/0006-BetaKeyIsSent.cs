using System;
using FluentMigrator;
using FluentMigrator.Runner.Extensions;

namespace LibLogic.Migrations
{
    [Migration(6, TransactionBehavior.Default)]
    public class BetaKeyIsSent : Migration
    {

        public override void Up()
        {

            Alter.Table("BetaKeys").AddColumn("IsSent").AsBoolean().Nullable();
            Update.Table("BetaKeys").Set(new { IsSent = false}).AllRows();
            Alter.Table("BetaKeys").AlterColumn("IsSent").AsBoolean().NotNullable();

        }

        public override void Down()
        {
            Delete.Column("IsSent").FromTable("BetaKeys");
        }
    }
}

