using FluentMigrator;

namespace Majorsilence.Vpn.Logic.Migrations;

[Migration(10)]
public class TestBetaKeys2 : Migration
{
    public override void Up()
    {
        Insert.IntoTable("BetaKeys").Row(new { Code = "AbC56#3", IsUsed = false });
    }

    public override void Down()
    {
    }
}