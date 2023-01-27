using System;
using FluentMigrator;
using FluentMigrator.Runner.Extensions;

namespace Majorsilence.Vpn.Logic.Migrations;

[Migration(11, TransactionBehavior.Default)]
public class ChangePaymentRates : Migration
{
    public override void Up()
    {
        Update.Table("PaymentRates").Set(new { CurrentMonthlyRate = 5 }).AllRows();
        Update.Table("PaymentRates").Set(new { CurrentYearlyRate = 50 }).AllRows();
    }

    public override void Down()
    {
        Update.Table("PaymentRates").Set(new { CurrentMonthlyRate = 9.97 }).AllRows();
        Update.Table("PaymentRates").Set(new { CurrentYearlyRate = 99.97 }).AllRows();
    }
}