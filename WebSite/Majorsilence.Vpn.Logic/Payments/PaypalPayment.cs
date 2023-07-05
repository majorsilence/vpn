using System;
using System.Collections.Generic;
using System.Net;
using Majorsilence.Vpn.Logic.DTO;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Logic.Helpers;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace Majorsilence.Vpn.Logic.Payments;

public class PaypalPayment
{
    private static string accessToken = "";
    private static string paymentid = "";
    private readonly DatabaseSettings _dbSettings;
    private readonly IPaypalSettings _paypalSettings;
    private readonly int userId;
    private string url;

    private PaypalPayment()
    {
    }

    public PaypalPayment(int userId, IPaypalSettings paypalSettings, DatabaseSettings dbSettings)
    {
        this.userId = userId;
        _paypalSettings = paypalSettings;
        _dbSettings = dbSettings;
    }

    /// <summary>
    ///     Initialize process with paypal.  Will direct users to paypal site.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="amount"></param>
    /// <param name="isRecurring">
    ///     Is a recurring payment subscription.
    ///     Classic API must be used until rest api supports this
    /// </param>
    /// <returns></returns>
    /// <remarks>Do finalize a payment ExecutePayment is called.</remarks>
    public string InitializePayment(string url, decimal amount, bool isRecurring)
    {
        //Validate
        if (string.IsNullOrEmpty(url)) throw new InvalidDataException("Invalid");

        if (amount == 0) throw new InvalidDataException("Invalid");

        this.url = url;
        // Get the access token to do payments 
        var token = GetAccessToken();
        // Use the access token and do the payment 
        var approveURL = DoPayment(token, amount);
        if (!string.IsNullOrEmpty(approveURL))
            return approveURL;
        throw new InvalidDataException("Invalid");
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
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = response.Content;
                var token = JsonConvert.DeserializeObject<AccessTokenResponse>(content);
                var atoken = "Bearer " + token.access_token.Trim();
                accessToken = atoken;
                return atoken;
            }

            throw new InvalidDataException(response.StatusDescription);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException(ex.Message);
        }
    }

    /// <summary>
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

        var preq = new PaypalPaymentRequest();
        preq.intent = "sale";
        //To Do:Change Reurn Url and Cancel URL
        preq.redirect_urls = new PayPalRedirect_urls
        {
            return_url = url + "/setup/paypal/paymentsmade.aspx",
            cancel_url = url + "/setup/paypal/paymentscancel.aspx"
        };
        preq.payer = new PayPalPayer { payment_method = "paypal" };
        preq.transactions = new List<PayPalTransactions>
        {
            new()
            {
                amount = new PayPalAmount { total = amount.ToString("0.00"), currency = "USD" },
                description = "test"
            }
        };
        request.AddBody(preq);

        try
        {
            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var content = response.Content;

                var payPalResponse =
                    JsonConvert.DeserializeObject<PaypalPaymentResponse>(content);

                // Send back the approval URL
                foreach (var link in payPalResponse.links)
                    if (link.rel == "approval_url")
                    {
                        paymentid = payPalResponse.id;
                        return link.href;
                    }

                return "";
            }

            throw new InvalidDataException(response.StatusDescription);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException(ex.Message);
        }
    }

    /// <summary>
    ///     Finialize payments
    /// </summary>
    /// <param name="payid"></param>
    /// <param name="tokenn"></param>
    public void ExecutePayment(string payid, string tokenn)
    {
        //validate
        if (string.IsNullOrEmpty(payid)) throw new InvalidDataException("Invalid Request");

        if (string.IsNullOrEmpty(paymentid)) throw new InvalidDataException("Invalid Request");

        if (string.IsNullOrEmpty(accessToken)) throw new InvalidDataException("Invalid Request");

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
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = response.Content;
                accessToken = null;
                paymentid = null;
                var payPalResponse =
                    JsonConvert.DeserializeObject<PaypalPayExecuteResponse>(content);

                var pay = new Payment(userId, _dbSettings);
                foreach (var transaction in payPalResponse.transactions)
                {
                    var amount = transaction.amount.total;
                    var createtime = payPalResponse.update_time;
                    pay.SaveUserPayment(Convert.ToDecimal(amount), createtime, SiteInfo.MonthlyPaymentId);
                }
            }
            else
            {
                throw new InvalidDataException(response.StatusDescription);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidDataException(ex.Message);
        }
    }
}