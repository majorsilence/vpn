using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;
using RestSharp;
using RestSharp.Authenticators;

namespace Majorsilence.Vpn.Logic.Payments
{
    public class PaypalPayment
    {

        private string url;
        private static string accessToken = "";
        private static string paymentid = "";
        private int userId;

        private PaypalPayment()
        {
        }

        public PaypalPayment(int userId)
        {
            this.userId = userId;
        }

        /// <summary>
        /// Initialize process with paypal.  Will direct users to paypal site.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="amount"></param>
        /// <param name="isRecurring">Is a recurring payment subscription.  
        /// Classic API must be used until rest api supports this</param>
        /// <returns></returns>
        /// <remarks>Do finalize a payment ExecutePayment is called.</remarks>
        public string InitializePayment(string url, decimal amount, bool isRecurring)
        {
            //Validate
            if (string.IsNullOrEmpty(url))
            {
                throw new Exceptions.InvalidDataException("Invalid");
            }
            if (amount == 0)
            {
                throw new Exceptions.InvalidDataException("Invalid");
            }

            this.url = url;
            // Get the access token to do payments 
            string token = GetAccessToken();
            // Use the access token and do the payment 
            string approveURL = DoPayment(token, amount);
            if (!string.IsNullOrEmpty(approveURL))
            {
                return approveURL;
            }
            else
            {
                throw new Exceptions.InvalidDataException("Invalid");
            }
        }

        private string GetAccessToken()
        {

            var client = new RestClient("https://api.sandbox.paypal.com/v1/oauth2/token");
            client.Authenticator = new HttpBasicAuthenticator(
                "AfhMaBAlk7Psj-PumK7fhiO2Ata3Pq8EBUyfbWyfBX3hbhCaL3OSyPPJure_",
                "ELgWJxB-qFjyorRj75ZM8tb0y_7_Cqzt_U02hhGFPy5Nw1QU4zFh7Ii5LMN6");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "client_credentials");
            try
            {
                var response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = response.Content;
                    var token = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.AccessTokenResponse>(content);
                    string atoken = "Bearer " + token.access_token.Trim();
                    accessToken = atoken;
                    return atoken;
                }
                else
                {
                    throw new Exceptions.InvalidDataException(response.StatusDescription);
                }

            }
            catch (Exception ex)
            {

                throw new Exceptions.InvalidDataException(ex.Message);

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private string DoPayment(string token, decimal amount)
        {

            var client = new RestClient("https://api.sandbox.paypal.com/v1/payments/payment");
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", token);

            DTO.PaypalPaymentRequest preq = new DTO.PaypalPaymentRequest();
            preq.intent = "sale";
            //To Do:Change Reurn Url and Cancel URL
            preq.redirect_urls = new DTO.PayPalRedirect_urls
            {
                return_url = url + "/setup/paypal/paymentsmade.aspx",
                cancel_url = url + "/setup/paypal/paymentscancel.aspx"
            };
            preq.payer = new DTO.PayPalPayer { payment_method = "paypal" };
            preq.transactions = new List<DTO.PayPalTransactions>
            {
                new DTO.PayPalTransactions
                { 
                    amount = new DTO.PayPalAmount { total = amount.ToString("0.00"), currency = "USD" },
                    description = "test" 
                }
            };
            request.AddBody(preq);

            try
            {
                var response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var content = response.Content;
                   
                    var payPalResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.PaypalPaymentResponse>(content);

                    // Send back the approval URL
                    foreach (DTO.PayPallinks link in payPalResponse.links)
                    {
                        if (link.rel == "approval_url")
                        {
                            paymentid = payPalResponse.id;
                            return link.href;
                        }
                    }

                    return "";
                }
                else
                {
                    throw new Exceptions.InvalidDataException(response.StatusDescription);
                }

            }
            catch (Exception ex)
            {

                throw new Exceptions.InvalidDataException(ex.Message);

            }
        }

        /// <summary>
        /// Finialize payments
        /// </summary>
        /// <param name="payid"></param>
        /// <param name="tokenn"></param>
        public void ExecutePayment(string payid, string tokenn)
        {

            //validate
            if (String.IsNullOrEmpty(payid))
            {
                throw new Exceptions.InvalidDataException("Invalid Request");
            }
            if (String.IsNullOrEmpty(paymentid))
            {
                throw new Exceptions.InvalidDataException("Invalid Request");
            }
            if (String.IsNullOrEmpty(accessToken))
            {
                throw new Exceptions.InvalidDataException("Invalid Request");
            }

            var client = new RestClient("https://api.sandbox.paypal.com/v1/payments/payment/" + paymentid + "/execute/");
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            //string token=GetAccessToken();
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", accessToken);
            request.AddBody(new { payer_id = payid.Trim() });
            try
            {
                var response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = response.Content;
                    accessToken = null;
                    paymentid = null;
                    var payPalResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.PaypalPayExecuteResponse>(content);

                    var pay = new Payment(this.userId);
                    foreach (DTO.PayPalTransactions transaction in payPalResponse.transactions)
                    {
                        string amount = transaction.amount.total;
                        DateTime createtime = payPalResponse.update_time;
                        pay.SaveUserPayment(Convert.ToDecimal(amount), createtime, Helpers.SiteInfo.MonthlyPaymentId);
                    }
                }
                else
                {
                    throw new Exceptions.InvalidDataException(response.StatusDescription);
                }

            }
            catch (Exception ex)
            {

                throw new Exceptions.InvalidDataException(ex.Message);

            }

        }

    }
}