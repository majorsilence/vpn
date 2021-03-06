using System;
using FluentMigrator;
using FluentMigrator.Runner.Extensions;

namespace Majorsilence.Vpn.Logic.Migrations
{
    [Migration(6, TransactionBehavior.Default)]
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
}

