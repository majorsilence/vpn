using FluentMigrator;

namespace Majorsilence.Vpn.Logic.Migrations;

[Migration(3)]
public class TestBetaKeys : Migration
{
    public override void Up()
    {
        Insert.IntoTable("BetaKeys").Row(new { Code = "AbC56#", IsUsed = false });
        Insert.IntoTable("BetaKeys").Row(new { Code = "AbC56#2", IsUsed = false });
    }

    public override void Down()
    {
    }
}