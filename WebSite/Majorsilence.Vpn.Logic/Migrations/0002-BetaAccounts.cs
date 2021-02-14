using System;
using FluentMigrator;
using FluentMigrator.Runner.Extensions;

namespace Majorsilence.Vpn.Logic.Migrations
{
    [Migration(2, TransactionBehavior.Default)]
    public class BetaAccounts : Migration
    {
        public BetaAccounts()
        {
        }

        public override void Up()
        {

            Alter.Table("Users").AddColumn("IsBetaUser").AsBoolean().Nullable();
            Update.Table("Users").Set(new { IsBetaUser = false}).AllRows();
            Alter.Table("Users").AlterColumn("IsBetaUser").AsBoolean().NotNullable();
          
            Create.Table("BetaKeys")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("Code").AsString(255).NotNullable().Unique()
                    .WithColumn("IsUsed").AsBoolean().NotNullable();


            for (int i = 0; i<100; i++)
            {
                string betaKey = System.Guid.NewGuid().ToString();
                Insert.IntoTable("BetaKeys").Row(new {Code=betaKey, IsUsed=false});
            }

        }

        public override void Down()
        {
            Delete.Column("IsBetaUser").FromTable("Users");
            Delete.Table("BetaKeys");
 
        }
    }
}

