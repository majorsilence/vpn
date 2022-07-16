using System;
using FluentMigrator;
using FluentMigrator.Runner.Extensions;

namespace Majorsilence.Vpn.Logic.Migrations
{
    [Migration(16, TransactionBehavior.Default)]
    public class ActionLog : Migration
    {
        public ActionLog()
        {
        }

        public override void Up()
        {
            Create.Table("ActionLog")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("UserId").AsInt32().Unique()
                .WithColumn("Action").AsCustom("TEXT").NotNullable()
                .WithColumn("ActionDate").AsDateTime().NotNullable();

            Create.ForeignKey("fk_ActionLog_Users_Id")
                .FromTable("ActionLog").ForeignColumn("UserId")
                .ToTable("Users").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.Table("ActionLog");
        }

    }
}

