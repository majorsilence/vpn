﻿using System;
using FluentMigrator;

namespace Majorsilence.Vpn.Logic.Migrations;

[Migration(4)]
public class TermsOfService : Migration
{
    public override void Up()
    {
        Create.Table("TermsOfService")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Terms").AsCustom("MEDIUMTEXT").NotNullable()
            .WithColumn("CreateTime").AsDateTime().NotNullable().Indexed("idx_termsofservice_createtime");


        var terms =
            @"<h1>Majorsilence Terms of Service</h1><p>Last modified: June 21, 2014 (<a href=""https://vpn.majorsilence.com/terms/archive"">view archived versions</a>)</p><h2>Welcome to Majorsilence VPN!</h2><p>Thanks for using our products and services (""Services""). The Services are provided by Majorsilence Solutions Inc. (""Majorsilence"").</p><p>By using our Services, you are agreeing to these terms. Please read them carefully.</p><p>Our Services are very diverse, so sometimes additional terms or product requirements (including age requirements) may apply. Additional terms will be available with the relevant Services, and those additional terms become part of your agreement with us if you use those Services.</p><h2 id=toc-services>Using our Services</h2><p>You must follow any policies made available to you within the Services.</p><p>Don't misuse our Services. For example, don't interfere with our Services or try to access them using a method other than the interface and the instructions that we provide. You may use our Services only as permitted by law, including applicable export and re-export control laws and regulations. We may suspend or stop providing our Services to you if you do not comply with our terms or policies or if we are investigating suspected misconduct.</p><p>Using our Services does not give you ownership of any intellectual property rights in our Services or the content you access. You may not use content from our Services unless you obtain permission from its owner or are otherwise permitted by law. These terms do not grant you the right to use any branding or logos used in our Services. Don't remove, obscure, or alter any legal notices displayed in or along with our Services.</p><p>Our Services display some content that is not Majorsilence's. This content is the sole responsibility of the entity that makes it available. We may review content to determine whether it is illegal or violates our policies, and we may remove or refuse to display content that we reasonably believe violates our policies or the law. But that does not necessarily mean that we review content, so please don't assume that we do.</p><p>In connection with your use of the Services, we may send you service announcements, administrative messages, and other information. You may opt out of some of those communications.</p><p>Some of our Services are available on mobile devices. Do not use such Services in a way that distracts you and prevents you from obeying traffic or safety laws.</p><h2 id=toc-account>Your Majorsilence VPN Account</h2><p>You may need a Majorsilence VPN Account in order to use some of our Services. You may create your own Majorsilence VPN Account, or your Majorsilence VPN Account may be assigned to you by an administrator, such as your employer or educational institution. If you are using a Majorsilence VPN Account assigned to you by an administrator, different or additional terms may apply and your administrator may be able to access or disable your account.</p><p>To protect your Majorsilence VPN Account, keep your password confidential. You are responsible for the activity that happens on or through your Majorsilence VPN Account. Try not to reuse your Majorsilence VPN Account password on third-party applications. If you learn of any unauthorized use of your password or Majorsilence VPN Account, <a href=https://vpn.majorsilence.com/knowledgebase?code=misuse>follow these instructions</a>.</p><h2 id=toc-protection>Privacy and Copyright Protection</h2><p>Majorsilence's <a href=https://vpn.majorsilence.com/privacy>privacy policies</a> explain how we treat your personal data and protect your privacy when you use our Services. By using our Services, you agree that Majorsilence can use such data in accordance with our privacy policies.</p><p>We respond to notices of alleged copyright infringement and terminate accounts of repeat infringers according to the process set out in the U.S. Digital Millennium Copyright Act.</p><p>We provide information to help copyright holders manage their intellectual property online. If you think somebody is violating your copyrights and want to notify us, you can find information about submitting notices and Majorsilence's policy about responding to notices <a href=http://vpn.majorsilence.com/knowledgebase?code=dmca>in our Help Center</a>.</p><h2 id=toc-content>Your Content in our Services</h2><p>Some of our Services allow you to upload, submit, store, send or receive content. You retain ownership of any intellectual property rights that you hold in that content. In short, what belongs to you stays yours.</p><p>When you upload, submit, store, send or receive content to or through our Services, you give Majorsilence (and those we work with) a worldwide license to use, host, store, reproduce, modify, create derivative works (such as those resulting from translations, adaptations or other changes we make so that your content works better with our Services), communicate, publish, publicly perform, publicly display and distribute such content. The rights you grant in this license are for the limited purpose of operating, promoting, and improving our Services, and to develop new ones. This license continues even if you stop using our Services. Some Services may offer you ways to access and remove content that has been provided to that Service. Also, in some of our Services, there are terms or settings that narrow the scope of our use of the content submitted in those Services. Make sure you have the necessary rights to grant us this license for any content that you submit to our Services.</p><p>You can find more information about how Majorsilence uses and stores content in the privacy policy or additional terms for particular Services. If you submit feedback or suggestions about our Services, we may use your feedback or suggestions without obligation to you.</p><h2 id=toc-software>About Software in our Services</h2><p>When a Service requires or includes downloadable software, this software may update automatically on your device once a new version or feature is available. Some Services may let you adjust your automatic update settings.</p><p>Majorsilence gives you a personal, worldwide, royalty-free, non-assignable and non-exclusive license to use the software provided to you by Majorsilence as part of the Services. This license is for the sole purpose of enabling you to use and enjoy the benefit of the Services as provided by Majorsilence, in the manner permitted by these terms. You may not copy, modify, distribute, sell, or lease any part of our Services or included software, nor may you reverse engineer or attempt to extract the source code of that software, unless laws prohibit those restrictions or you have our written permission.</p><p>Open source software is important to us. Some software used in our Services may be offered under an open source license that we will make available to you. There may be provisions in the open source license that expressly override some of these terms.</p><h2 id=toc-modification>Modifying and Terminating our Services</h2><p>We are constantly changing and improving our Services. We may add or remove functionalities or features, and we may suspend or stop a Service altogether.</p><p>You can stop using our Services at any time, although we'll be sorry to see you go. Majorsilence may also stop providing Services to you, or add or create new limits to our Services at any time.</p><p>We believe that you own your data and preserving your access to such data is important. If we discontinue a Service, where reasonably possible, we will give you reasonable advance notice and a chance to get information out of that Service.</p><h2 id=toc-warranties-disclaimers>Our Warranties and Disclaimers</h2><p>We provide our Services using a commercially reasonable level of skill and care and we hope that you will enjoy using them. But there are certain things that we don't promise about our Services.</p><p>Other than as expressly set out in these terms or additional terms, neither Majorsilence nor its suppliers or distributors make any specific promises about the Services. For example, we don't make any commitments about the content within the Services, the specific functions of the Services, or their reliability, availability, or ability to meet your needs. We provide the Services ""as is"".</p><p>Some jurisdictions provide for certain warranties, like the implied warranty of merchantability, fitness for a particular purpose and non-infringement. To the extent permitted by law, we exclude all warranties.</p><h2 id=toc-liability>Liability for our Services</h2><p>When permitted by law, Majorsilence, and Majorsilence's suppliers and distributors, will not be responsible for lost profits, revenues, or data, financial losses or indirect, special, consequential, exemplary, or punitive damages.</p><p>To the extent permitted by law, the total liability of Majorsilence, and its suppliers and distributors, for any claims under these terms, including for any implied warranties, is limited to the amount you paid us to use the Services (or, if we choose, to supplying you the Services again).</p><p>In all cases, Majorsilence, and its suppliers and distributors, will not be liable for any loss or damage that is not reasonably foreseeable.</p><p>We recognize that in some countries, you might have legal rights as a consumer. If you are using the Services for a personal purpose, then nothing in these terms or any additional terms limits any consumer legal rights which may not be waived by contract.</p><h2 id=toc-business-uses>Business uses of our Services</h2><p>If you are using our Services on behalf of a business, that business accepts these terms. It will hold harmless and indemnify Majorsilence and its affiliates, officers, agents, and employees from any claim, suit or action arising from or related to the use of the Services or violation of these terms, including any liability or expense arising from claims, losses, damages, suits, judgments, litigation costs and attorneys' fees.</p><h2 id=toc-about>About these Terms</h2><p>We may modify these terms or any additional terms that apply to a Service to, for example, reflect changes to the law or changes to our Services. You should look at the terms regularly. We'll post notice of modifications to these terms on this page. We'll post notice of modified additional terms in the applicable Service. Changes will not apply retroactively and will become effective no sooner than fourteen days after they are posted. However, changes addressing new functions for a Service or changes made for legal reasons will be effective immediately. If you do not agree to the modified terms for a Service, you should discontinue your use of that Service.</p><p>If there is a conflict between these terms and the additional terms, the additional terms will control for that conflict.</p><p>These terms control the relationship between Majorsilence and you. They do not create any third party beneficiary rights.</p><p>If you do not comply with these terms, and we don't take action right away, this doesn't mean that we are giving up any rights that we may have (such as taking action in the future).</p><p>If it turns out that a particular term is not enforceable, this will not affect any other terms.</p><p>The courts in some countries will not apply Newfoundland law to some types of disputes. If you reside in one of those countries, then where Newfoundland law is excluded from applying, your country's laws will apply to such disputes related to these terms. Otherwise, you agree that the laws of Newfoundland, Canada, excluding Newfoundland's choice of law rules, will apply to any disputes arising out of or relating to these terms or the Services. Similarly, if the courts in your country will not permit you to consent to the jurisdiction and venue of the courts in Gander, Newfoundland, Canda, then your local jurisdiction and venue will apply to such disputes related to these terms. Otherwise, all claims arising out of or relating to these terms or the services will be litigated exclusively in the federal or provincial courts of Gander, Newfoundland, Canada, and you and Majorsilence consent to personal jurisdiction in those courts.</p><p>For information about how to contact Majorsilence, please visit our <a href=http://www.vpn.majorsilence.com/contact>contact page</a>.</p>";

        Insert.IntoTable("TermsOfService").Row(new { Terms = terms, CreateTime = DateTime.UtcNow });
    }

    public override void Down()
    {
        Delete.Table("TermsOfService");
    }
}