using System;
using FluentMigrator;
using FluentMigrator.Runner.Extensions;

namespace LibLogic.Migrations
{
    [Migration(3, TransactionBehavior.Default)]
    public class TestBetaKeys :Migration
    {
        public TestBetaKeys()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("BetaKeys").Row(new {Code="AbC56#", IsUsed=false});
            Insert.IntoTable("BetaKeys").Row(new {Code="AbC56#2", IsUsed=false});

        }

        public override void Down()
        {

        }
    }
}

