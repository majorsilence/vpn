using System;
using FluentMigrator;
using FluentMigrator.Runner.Extensions;

namespace Majorsilence.Vpn.Logic.Migrations;

[Migration(10, TransactionBehavior.Default)]
public class TestBetaKeys2 : Migration
{
    public TestBetaKeys2()
    {
    }

    public override void Up()
    {
        Insert.IntoTable("BetaKeys").Row(new { Code = "AbC56#3", IsUsed = false });
    }

    public override void Down()
    {
    }
}