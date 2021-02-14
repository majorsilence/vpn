﻿using System;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;

namespace LibLogic.Accounts
{
    public class BetaKeys
    {
        private Email.IEmail email;

        public BetaKeys(Email.IEmail email)
        {
            this.email = email;
        }

        public int UnsuedKeyCount()
        {

            using (var db = Setup.DbFactory)
            {
                db.Open();

                var data = db.Query<int>("SELECT count(id) FROM BetaKeys WHERE IsUsed=0");
                return data.First();

            }

        }

        private string RetrieveAndMarkSendKey()
        {
            using (var db = Setup.DbFactory)
            {
                db.Open();
                using (var txn = db.BeginTransaction())
                {
                    var data = db.Query<LibPoco.BetaKeys>("SELECT * FROM BetaKeys WHERE IsUsed=0 AND IsSent=0 LIMIT 1", null, txn);
                    data.First().IsSent = true;
                    db.Update(data.First(), txn, null);

                    txn.Commit();

                    return data.First().Code;
                }
            }
        }

        /// <summary>
        /// Mails the invite.
        /// </summary>
        /// <returns>The betakey code</returns>
        /// <param name="emailAddress">Email address.</param>
        public string MailInvite(string emailAddress)
        {

            var betakey = RetrieveAndMarkSendKey();
            var subject = LibLogic.Helpers.SiteInfo.SiteName + " Invite";

            string signupLink = string.Format("{0}/?betaemail={1}&betacode={2}", 
                                    LibLogic.Helpers.SiteInfo.SiteUrl, 
                                    System.Web.HttpUtility.HtmlEncode(emailAddress), 
                                    System.Web.HttpUtility.HtmlEncode(betakey));

            var message = string.Format("You have been invited to signup at <a href=\"{0}\">{1}</a>.", 
                              signupLink, LibLogic.Helpers.SiteInfo.SiteName);
            message += " Your beta key is below. <br><br>";
            message += string.Format("<strong>{0}</strong>", betakey);

            email.SendMail_BackgroundThread(message, subject, emailAddress, true, null, Email.EmailTemplates.BetaKey);

            return betakey;
        }

        private string GenerateKeyAndMarkSent()
        {
            string betaKey = System.Guid.NewGuid().ToString();
            using (var db = Setup.DbFactory)
            {
                db.Open();
               
                var data = new LibPoco.BetaKeys(betaKey, false, true);
                db.Insert(data);
            }

            return betaKey;
        }

        public string MailInvite(string emailAddressSendTo, int sentFromUserId)
        {
            string sentFromFirstName;
            string sentFromLastName;
            string sentFromEmailAddress;
            using (var db = Setup.DbFactory)
            {
                db.Open();
      
                var data = db.Get<LibPoco.Users>(sentFromUserId);

                sentFromFirstName = data.FirstName;
                sentFromLastName = data.LastName;
                sentFromEmailAddress = data.Email;
              
            }


            var betakey = GenerateKeyAndMarkSent();
            var subject = LibLogic.Helpers.SiteInfo.SiteName + " Invite";

            string signupLink = string.Format("{0}/?betaemail={1}&betacode={2}", 
                                    LibLogic.Helpers.SiteInfo.SiteUrl, 
                                    System.Web.HttpUtility.HtmlEncode(emailAddressSendTo), 
                                    System.Web.HttpUtility.HtmlEncode(betakey));

            var message = string.Format("You have been invited to signup at <a href=\"{0}\">{1}</a> by {2} {3} ({4}).<br><br>", 
                              signupLink, LibLogic.Helpers.SiteInfo.SiteName, sentFromFirstName, sentFromLastName, 
                              System.Web.HttpUtility.HtmlEncode(sentFromEmailAddress));

            message += " Your beta key is below. <br><br>";
            message += string.Format("<strong>{0}</strong>", betakey);

            email.SendMail_BackgroundThread(message, subject, emailAddressSendTo, true, null, Email.EmailTemplates.BetaKey);

            return betakey;
        }

    }
}

