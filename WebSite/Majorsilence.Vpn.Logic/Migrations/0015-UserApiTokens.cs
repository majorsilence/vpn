using FluentMigrator;

namespace Majorsilence.Vpn.Logic.Migrations;

[Migration(15)]
public class UserApiTokens : Migration
{
    public override void Up()
    {
        Create.Table("UsersApiTokens")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("UserId").AsInt32().Unique()
            .WithColumn("Token1").AsString(255).NotNullable()
            .WithColumn("Token1ExpireTime").AsDateTime().NotNullable()
            .WithColumn("Token2").AsString(255).NotNullable()
            .WithColumn("Token2ExpireTime").AsDateTime().NotNullable();

        Create.ForeignKey("fk_UsersApiTokens_Users_Id")
            .FromTable("UsersApiTokens").ForeignColumn("UserId")
            .ToTable("Users").PrimaryColumn("Id");
    }

    public override void Down()
    {
        Delete.Table("UsersApiTokens");
    }
}