using System;
using FluentMigrator;
using FluentMigrator.Runner.Extensions;

namespace Majorsilence.Vpn.Logic.Migrations
{
    [Migration(7, TransactionBehavior.Default)]
    public class PptpPasswords : Migration
    {
        public PptpPasswords()
        {
        }

        public override void Up()
        {

            Alter.Table("UserPptpInfo").AddColumn("Password").AsString().Nullable();
            Update.Table("UserPptpInfo").Set(new { Password = ""}).AllRows();
            Alter.Table("UserPptpInfo").AlterColumn("Password").AsString().NotNullable();

        }

        public override void Down()
        {
            Delete.Column("Password").FromTable("UserPptpInfo");
        }
    }
}

