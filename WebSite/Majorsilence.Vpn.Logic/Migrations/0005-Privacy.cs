using System;
using FluentMigrator;
using FluentMigrator.Runner.Extensions;

namespace Majorsilence.Vpn.Logic.Migrations
{
    [Migration(5, TransactionBehavior.Default)]
    public class Privacy : Migration
    {
        public Privacy()
        {
        }

        public override void Up()
        {

            Create.Table("Privacy")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Policy").AsCustom("MEDIUMTEXT").NotNullable()
                .WithColumn("CreateTime").AsDateTime().NotNullable().Indexed("idx_privacy_createtime");


            string policy = @"<h1>Information Collection and Use</h1><p><strong>Information Collected Upon Registration:</strong> When you create a Majorsilence VPN account, you provide some personal information, such as your name, username, password, and email address. Some of this information, for example, your name is used for credit card charges. Nothing else is shared with other companies or organizations.</p><p>Log Data: Our servers automatically record information (""Log Data"") created by your use of the Services. Log Data may include information such as your IP address, browser type, operating system, the referring web page, pages visited, location, your mobile carrier, device and application IDs, search terms, and cookie information. We receive Log Data when you interact with our Services, for example, when you visit our websites, sign into our Services, interact with our email notifications. We delete Log Data containing this type of information routinely. Any data collected will not be shared with other companies or organizations.</p><p><strong>Law and Harm:</strong> Notwithstanding anything to the contrary in this Policy, we may preserve or disclose your information if we believe that it is reasonably necessary to comply with a law, regulation or legal request; to protect the safety of any person; to address fraud, security or technical issues; or to protect Majorsilence's rights or property. However, nothing in this Privacy Policy is intended to limit any legal defenses or objections that you may have to a third party’s, including a government’s, request to disclose your information.</p>Our Services are not directed to persons under 18. We do not knowingly collect personal information from persons under 18. If we become aware that a person under 18 has provided us with personal information, we take steps to remove such information and terminate the person's account. We may revise this Privacy Policy from time to time. The most current version of the policy will govern our use of your information and will always be at https://majorsilencevpn.com/privacy. If we make a change to this policy that, in our sole discretion, is material, we will notify you via email to the email address associated with your account. By continuing to access or use the Services after those changes become effective, you agree to be bound by the revised Privacy Policy.";

            Insert.IntoTable("Privacy").Row(new {Policy=policy, CreateTime=DateTime.UtcNow});
        }

        public override void Down()
        {
            Delete.Table("Privacy");
        }

    }
}

