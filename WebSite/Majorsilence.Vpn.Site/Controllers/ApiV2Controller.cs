using System;
using System.Linq;
using System.Net;
using System.Text;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.DTO;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Logic.OpenVpn;
using Majorsilence.Vpn.Site.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Majorsilence.Vpn.Site.Controllers;

public class ApiV2Controller : Controller
{
    private readonly DatabaseSettings _dbSettings;
    private readonly IEncryptionKeysSettings _keys;
    private readonly ILogger<ApiV2Controller> _logger;
    private readonly ISessionVariables sessionVars;

    public ApiV2Controller(ISessionVariables sessionInstance, ILogger<ApiV2Controller> logger,
        IEncryptionKeysSettings keys,
        DatabaseSettings dbSettings)
    {
        sessionVars = sessionInstance;
        _logger = logger;
        _keys = keys;
        _dbSettings = dbSettings;
    }

    public ActionResult Index()
    {
        return View();
    }

    private string[] ParseAuthHeader(string authHeader)
    {
        // Check this is a Basic Auth header
        if (authHeader == null || authHeader.Length == 0 || !authHeader.StartsWith("Basic"))
            return null;

        // Pull out the Credentials with are seperated by ':' and Base64 encoded
        var base64Credentials = authHeader.Substring(6);
        var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(base64Credentials))
            .Split(new[] { ':' });

        if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[0]))
            return null;

        // Okay this is the credentials
        return credentials;
    }

    private bool IsAuthenticateUserWithToken(HttpContext context, out int UserId)
    {
        UserId = -1;
        if (!context.Request.Headers.Keys.Contains("VpnAuthToken", StringComparer.OrdinalIgnoreCase)) return false;
        if (!context.Request.Headers.Keys.Contains("VpnUserId", StringComparer.OrdinalIgnoreCase)) return false;

        string token = context.Request.Headers["VpnAuthToken"];
        var uid = -1;
        int.TryParse(context.Request.Headers["VpnUserId"], out uid);
        var api = new UserApiTokens(_keys, _dbSettings);
        var data = api.Retrieve(uid);

        if (data.Token1 != token)
        {
            _logger.LogWarning("data.Token1 != token");
            return false;
        }

        if (data.Token1ExpireTime <= DateTime.UtcNow)
        {
            _logger.LogWarning("data.Token1ExpireTime <= DateTime.UtcNow");
            return false;
        }

        UserId = uid;
        return true;
    }

    [HttpPost]
    public ContentResult Auth()
    {
        // Majorsilence.Vpn.Logic.DTO.ApiAuthResponse results;

        try
        {
            if (!HttpContext.Request.Headers.Keys.Contains("Authorization", StringComparer.OrdinalIgnoreCase))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Content("Authorization not sent");
            }

            string authHeader = HttpContext.Request.Headers["Authorization"];
            var creds = ParseAuthHeader(authHeader);


            var login = new Login(creds[0], creds[1], _logger, _dbSettings);


            try
            {
                login.Execute();
            }
            catch (InvalidDataException ex)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                _logger.LogError(ex, "Auth error");
                return Content("InternalServerError");
            }


            if (!login.LoggedIn)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Content("Unauthorized");
            }

            sessionVars.LoggedIn = login.LoggedIn;
            sessionVars.IsAdmin = login.IsAdmin;
            sessionVars.UserId = login.UserId;
            sessionVars.Username = login.Username;


            var toks = new UserApiTokens(_keys, _dbSettings);
            var tokData = toks.Retrieve(login.UserId);

            var results = new ApiAuthResponse
            {
                Token1 = tokData.Token1,
                Token2 = tokData.Token2,
                Token1ExpireUtc = tokData.Token1ExpireTime,
                Token2ExpireUtc = tokData.Token2ExpireTime,
                UserId = sessionVars.UserId
            };

            var json = JsonConvert.SerializeObject(results);

            HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            return Content(json);
        }
        catch (Exception ex)
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            _logger.LogError(ex, "Auth error");
            return Content("InternalServerError");
        }
    }

    [HttpPost]
    public ContentResult Servers()
    {
        try
        {
            var userid = -1;
            if (!IsAuthenticateUserWithToken(HttpContext, out userid))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                _logger.LogInformation("IsAuthenticateUserWithToken is false");
                return Content("Unauthorized");
            }
        }
        catch (Exception ex)
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            _logger.LogError(ex, "Server lookup");
            return Content("InternalServerError " + ex.Message + ex.StackTrace);
        }

        try
        {
            var details = new ServerDetails(_dbSettings);

            var data = JsonConvert.SerializeObject(details.Info);
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Content(data);
        }
        catch (Exception ex)
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            _logger.LogError(ex, "Server lookup details");
            return Content("InternalServerError");
        }
    }


    [HttpPost]
    public ContentResult ChangeServer()
    {
        throw new NotImplementedException();
    }


    [HttpPost]
    public ContentResult DownloadOpenVpnCert()
    {
        var userid = -1;
        try
        {
            if (!IsAuthenticateUserWithToken(HttpContext, out userid))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                _logger.LogInformation("IsAuthenticateUserWithToken is false");
                return Content("Unauthorized");
            }
        }
        catch (Exception ex)
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            _logger.LogError(ex, "DownloadOpenVpnCert error");
            return Content("InternalServerError " + ex.Message + ex.StackTrace);
        }

        try
        {
            var data = WriteZippedCertsToString(userid);
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Content(data);
        }
        catch (Exception ex)
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            _logger.LogError(ex, "DownloadOpenVpnCert zipped certs error");
            return Content("InternalServerError");
        }
    }


    private string WriteZippedCertsToString(int userid)
    {
        var dl = new CertsOpenVpnDownload(_dbSettings);
        var fileBytes = dl.UploadToClient(userid);

        return Convert.ToBase64String(fileBytes);
    }
}