using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;
using RestSharp;
using RestSharp.Authenticators;

namespace Majorsilence.Vpn.Logic.Payments;

public class PaypalPayment
{
    private string url;
    private static string accessToken = "";
    private static string paymentid = "";
    private int userId;
    private IPaypalSettings _paypalSettings;

    private PaypalPayment()
    {
    }

    public PaypalPayment(int userId, IPaypalSettings paypalSettings)
    {
        this.userId = userId;
        _paypalSettings = paypalSettings;
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
        if (string.IsNullOrEmpty(url)) throw new Exceptions.InvalidDataException("Invalid");

        if (amount == 0) throw new Exceptions.InvalidDataException("Invalid");

        this.url = url;
        // Get the access token to do payments 
        var token = GetAccessToken();
        // Use the access token and do the payment 
        var approveURL = DoPayment(token, amount);
        if (!string.IsNullOrEmpty(approveURL))
            return approveURL;
        else
            throw new Exceptions.InvalidDataException("Invalid");
    }

    private string GetAccessToken()
    {
        var options = new RestClientOptions(_paypalSettings.Url);
        using var client = new RestClient(options);
        client.Authenticator = new HttpBasicAuthenticator(
            _paypalSettings.Username,
            _paypalSettings.Password);
        var request = new RestRequest("/v1/oauth2/token", Method.Post);
        request.AddHeader("content-type", "application/x-www-form-urlencoded");
        request.AddParameter("grant_type", "client_credentials");
        try
        {
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = response.Content;
                var token = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.AccessTokenResponse>(content);
                var atoken = "Bearer " + token.access_token.Trim();
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
        var options = new RestClientOptions(_paypalSettings.Url);
        using var client = new RestClient(options);
        var request = new RestRequest("/v1/payments/payment", Method.Post);
        request.RequestFormat = DataFormat.Json;
        request.AddHeader("Authorization", token);

        var preq = new DTO.PaypalPaymentRequest();
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
            new()
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

                var payPalResponse =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.PaypalPaymentResponse>(content);

                // Send back the approval URL
                foreach (var link in payPalResponse.links)
                    if (link.rel == "approval_url")
                    {
                        paymentid = payPalResponse.id;
                        return link.href;
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
        if (string.IsNullOrEmpty(payid)) throw new Exceptions.InvalidDataException("Invalid Request");

        if (string.IsNullOrEmpty(paymentid)) throw new Exceptions.InvalidDataException("Invalid Request");

        if (string.IsNullOrEmpty(accessToken)) throw new Exceptions.InvalidDataException("Invalid Request");

        var options = new RestClientOptions(_paypalSettings.Url);
        using var client = new RestClient(options);
        var request = new RestRequest("/v1/payments/payment/" + paymentid + "/execute/", Method.Post);
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
                var payPalResponse =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.PaypalPayExecuteResponse>(content);

                var pay = new Payment(userId);
                foreach (var transaction in payPalResponse.transactions)
                {
                    var amount = transaction.amount.total;
                    var createtime = payPalResponse.update_time;
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