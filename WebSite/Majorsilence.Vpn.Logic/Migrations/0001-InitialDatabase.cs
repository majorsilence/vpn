using System;
using FluentMigrator;
using FluentMigrator.Runner.Extensions;


namespace Majorsilence.Vpn.Logic.Migrations
{
    [Migration(1, TransactionBehavior.Default)]
    public class InitialDatabase : Migration
    {

        public override void Up()
        {
            Create.Table("DatabaseInfo")
				.WithColumn("Id").AsInt32().PrimaryKey().Identity()
				.WithColumn("VersionId").AsString(255).NotNullable()
				.WithColumn("CreateTime").AsDateTime().NotNullable()
				.WithColumn("LastDailyProcess").AsDateTime().NotNullable();

            Create.Table("Users")
				.WithColumn("Id").AsInt32().PrimaryKey().Identity()
				.WithColumn("Email").AsString(255).NotNullable().Unique()
				.WithColumn("Password").AsString(4000).NotNullable()
				.WithColumn("Salt").AsString(255).NotNullable()
				.WithColumn("FirstName").AsString(255).NotNullable()
				.WithColumn("LastName").AsString(255).NotNullable()
				.WithColumn("Admin").AsBoolean().NotNullable()
				.WithColumn("CreateTime").AsDateTime().NotNullable()
				.WithColumn("StripeCustomerAccount").AsString(255).NotNullable()
				.WithColumn("PasswordResetCode").AsString(255).NotNullable();


            Create.Table("Regions")
				.WithColumn("Id").AsInt32().PrimaryKey().Identity()
				.WithColumn("Description").AsString(255).NotNullable()
				.WithColumn("Active").AsBoolean().NotNullable();


            Create.Table("VpnServers")
				.WithColumn("Id").AsInt32().PrimaryKey().Identity()
				.WithColumn("Address").AsString(255).NotNullable().Unique()
				.WithColumn("VpnPort").AsInt32().NotNullable()
				.WithColumn("Description").AsString(4000).NotNullable()
				.WithColumn("RegionId").AsInt32().NotNullable()
				.WithColumn("Active").AsBoolean().NotNullable();
            Create.ForeignKey("fk_VpnServers_Regions_Id")
				.FromTable("VpnServers").ForeignColumn("RegionId")
				.ToTable("Regions").PrimaryColumn("Id");

            Create.Table("UserOpenVpnCerts")
				.WithColumn("Id").AsInt32().PrimaryKey().Identity()
				.WithColumn("UserId").AsInt32().Unique()
				.WithColumn("CertName").AsString(255).NotNullable()
				.WithColumn("CertCa").AsBinary().NotNullable()
				.WithColumn("CertCrt").AsBinary().NotNullable()
				.WithColumn("CertKey").AsBinary().NotNullable()
				.WithColumn("Expired").AsBoolean().NotNullable()
				.WithColumn("CreateTime").AsDateTime().NotNullable()
				.WithColumn("VpnServersId").AsInt32().NotNullable();
            Create.ForeignKey("fk_UserOpenVpnCerts_Users_Id")
				.FromTable("UserOpenVpnCerts").ForeignColumn("UserId")
				.ToTable("Users").PrimaryColumn("Id");
            Create.ForeignKey("fk_UserOpenVpnCerts_VpnServers_Id")
				.FromTable("UserOpenVpnCerts").ForeignColumn("VpnServersId")
				.ToTable("VpnServers").PrimaryColumn("Id");

            Create.Table("UserPptpInfo")
				.WithColumn("Id").AsInt32().PrimaryKey().Identity()
				.WithColumn("UserId").AsInt32().Unique()
				.WithColumn("Expired").AsBoolean().NotNullable()
				.WithColumn("CreateTime").AsDateTime().NotNullable()
				.WithColumn("VpnServersId").AsInt32().NotNullable();
            Create.ForeignKey("fk_UserPptpInfo_Users_Id")
				.FromTable("UserPptpInfo").ForeignColumn("UserId")
				.ToTable("Users").PrimaryColumn("Id");
            Create.ForeignKey("fk_UserPptpInfo_VpnServers_Id")
				.FromTable("UserPptpInfo").ForeignColumn("VpnServersId")
				.ToTable("VpnServers").PrimaryColumn("Id");

            Create.Table("LookupPaymentType")
				.WithColumn("Id").AsInt32().PrimaryKey().Identity()
				.WithColumn("Code").AsString(50).NotNullable().Unique()
				.WithColumn("Description").AsString(255).NotNullable();

            Create.Table("UserPayments")
				.WithColumn("Id").AsInt32().PrimaryKey().Identity()
				.WithColumn("UserId").AsInt32().NotNullable()
				.WithColumn("AmountPaid").AsDecimal().NotNullable()
				.WithColumn("CreateTime").AsDateTime().NotNullable()
				.WithColumn("LookupPaymentTypeId").AsInt32().NotNullable();
            Create.ForeignKey("fk_UserPayments_Users_Id")
				.FromTable("UserPayments").ForeignColumn("UserId")
				.ToTable("Users").PrimaryColumn("Id");
            Create.ForeignKey("fk_UserPayments_LookupPaymentType_Id")
				.FromTable("UserPayments").ForeignColumn("LookupPaymentTypeId")
				.ToTable("LookupPaymentType").PrimaryColumn("Id");

            Create.Table("Errors")
				.WithColumn("Id").AsInt32().PrimaryKey().Identity()
				.WithColumn("TimeCreated").AsDateTime().NotNullable()
				.WithColumn("Message").AsString(4000).NotNullable()
				.WithColumn("StackTrace").AsString(4000).NotNullable()
				.WithColumn("RecursiveStackTrace").AsString(8000).NotNullable();

            Create.Table("PaymentRates")
				.WithColumn("Id").AsInt32().PrimaryKey().Identity()
				.WithColumn("CurrentMonthlyRate").AsDecimal().NotNullable()
				.WithColumn("CurrentYearlyRate").AsDecimal().NotNullable();

            Create.Table("Counters")
				.WithColumn("Id").AsInt32().PrimaryKey().Identity()
				.WithColumn("Code").AsString(50).NotNullable()
				.WithColumn("Description").AsString(255).NotNullable()
				.WithColumn("Num").AsInt64().NotNullable();

            Create.Table("SiteInfo")
				.WithColumn("Id").AsInt32().PrimaryKey().Identity()
				.WithColumn("VpnSshUser").AsString(250).NotNullable()
				.WithColumn("VpnSshPassword").AsString(1000).NotNullable()
				.WithColumn("SshPort").AsInt32().NotNullable()
				.WithColumn("AdminEmail").AsString(250).NotNullable()
				.WithColumn("SiteName").AsString(1000).NotNullable()
				.WithColumn("SiteUrl").AsString(1000).NotNullable()
				.WithColumn("StripeAPISecretKey").AsString(250).NotNullable()
				.WithColumn("LiveSite").AsBoolean().NotNullable();

	
            Insert.IntoTable("DatabaseInfo").Row(new {VersionId = "1", CreateTime = DateTime.UtcNow, LastDailyProcess = DateTime.UtcNow});

            Insert.IntoTable("LookupPaymentType").Row(new {Code = "MONTHLY", Description = "Monthly subscription.  Payments made once a month."});
            Insert.IntoTable("LookupPaymentType").Row(new {Code = "YEARLY", Description = "Yearly subscription.  Payments made once a year."});

            // PaymentRates
            Insert.IntoTable("PaymentRates").Row(new {CurrentMonthlyRate = 9.97m, CurrentYearlyRate = 99.97m});

            // Counters
            Insert.IntoTable("Counters").Row(new {Code = "VPNCERT", Description = "Current vpn cert count.  Used to make unique vpn cert names",Num = 1});

            // Regions
            int usRegionId = 1;
            int canadaRegionId = 2;
            Insert.IntoTable("Regions").Row(new {Id = usRegionId, Description = "United States", Active = true});
            Insert.IntoTable("Regions").Row(new {Id = canadaRegionId, Description = "Canada", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "Netherlands", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "Singapore", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "Brazil", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "Ireland", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "Italy", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "Germany", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "France", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "United Kingdom", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "Sweden", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "Spain", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "Poland", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "Japan", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "India", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "South Korea", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "Hong Kong", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "Philippines", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "Taiwan", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "Australia", Active = true});
            Insert.IntoTable("Regions").Row(new {Description = "China", Active = true});
		
            // VpnServers
            Insert.IntoTable("VpnServers").Row(new {Address = "127.0.0.1", VpnPort = 1194, 
				Description = "default vagrant testing vpn authority", RegionId = canadaRegionId, Active = true});
            Insert.IntoTable("VpnServers").Row(new {Address = "10.0.2.2", VpnPort = 1194, 
				Description = "default vagrant testing vpn authority, access from vagrant web site", RegionId = canadaRegionId, Active = true});
			

            // SiteInfo
            int port = 22;
            bool liveSite = false;
            //string sshPassword = "NFklIXhfZMfHpEnfVPdheQUYiThaBtZEIgbSsqbtoTOEPVpShqqGdpiWGicLPgzBMwZjCrMENFPhcmejDkOldtxsTbhfomDhtiRsRfpMDMOxwbTueOhFooytVHpxjDqBcIJQuLoyRXEnAZSShNbvaUOfFyskMNDWvjvJndVoAJJrFlPWLtpsBRBdTEgSseHZqIRqDiEeOJQqlxztEsysxkuAIrMFScVwbJWTqysyOlEOakOweJnWBoBfnrElsViLNuoxwCdfZWAiCCAxzpBfLVRIiNFZQggPdOaCSrVyFdSEKoyGMgIcqdWODBoFSapSPHfJCUBhucvHcmYPoZiFudyVSiVqYmFFOZKZjKaOFAEnDpLqzzqPOFyMifRtVgfdtQOYxVFAUpaPvhsDwENySLNjBYNZTGPuvAPJwJPAPPRzBFqaJnTqpGhHcFxuhCDnvBuRnEgJLnFcQoESIMmFJzfKpiGNlusniDZakvIegjBbbeMKaALrqXESvFBfTotqQrPnkEDndhyvXZJkrRIGEaTlUdAFPDhOLhlFkzAwTyjeyeUvRnJcCJHLuEogdXuAaGFMOdZOrlJgNOIQjfJBPeicvvlCkutVdJeNveCNnRHqVBUCRuTUkOGVyCqtOxDWSNcdJUqayWoYtRcUwEGMwvHldfpONjUMnGtiuRSWdSwnADxCAsTgMNAxcRZz";
            string sshPassword = "aPassword";

            Insert.IntoTable("SiteInfo").Row(new {VpnSshUser = "avpnSshUser", VpnSshPassword = sshPassword, 
                SshPort = port, AdminEmail = "yourAdminEmail", SiteName = "Majorsilence VPN",
				SiteUrl = "https://majorsilencevpn.com",StripeAPISecretKey = "YourStripeSecretKey",
				LiveSite = liveSite});
				
        }

        public override void Down()
        {
            Delete.Table("DatabaseInfo");
            Delete.Table("Users");
            Delete.Table("Regions");
            Delete.Table("VpnServers");
            Delete.Table("UserOpenVpnCerts");
            Delete.Table("UserPptpInfo");
            Delete.Table("LookupPaymentType");
            Delete.Table("UserPayments");
            Delete.Table("Errors");
            Delete.Table("PaymentRates");
            Delete.Table("Counters");
            Delete.Table("SiteInfo");
        }
    }
}

