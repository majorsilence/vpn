using System;
using System.Web;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace MvcFakes
{
    /// <summary>
    /// See http://pretzelsteelersfan.blogspot.ca/2009/07/fake-httpresponse-for-aspnet-mvc.html
    /// </summary>
    public class FakeHttpResponse
        : HttpResponseBase
    {
        private HttpCookieCollection _cookies;
        private NameValueCollection _headers;
        private bool isClientConnected;
        private bool isRequestBeingRedirected;
        private TextWriter output;
        private Stream outputStream;

        public FakeHttpResponse(
            NameValueCollection headers,
            HttpCookieCollection cookies
        )
        {
            this._headers = headers;
            this._cookies = cookies;
        }

        public FakeHttpResponse(
            NameValueCollection headers,
            HttpCookieCollection cookies,
            bool isClientConnected,
            bool isRequestBeingRedirected,
            TextWriter output,
            Stream outputStream
        )
        {
            this._headers = headers;
            this._cookies = cookies;
            this.isClientConnected = isClientConnected;
            this.isRequestBeingRedirected = isRequestBeingRedirected;
            this.output = output;
            this.outputStream = outputStream;
        }

        public override HttpCookieCollection Cookies
        {
            get
            {
                if (this._cookies == null)
                {
                    this._cookies = new HttpCookieCollection();
                }
                return this._cookies;
            }
        }

        public override NameValueCollection Headers
        {
            get
            {
                if (this._headers == null)
                {
                    this._headers = new NameValueCollection();
                }
                return this._headers;
            }
        }

        public override bool Buffer { get; set; }

        public override bool BufferOutput { get; set; }

        public override string Charset { get; set; }

        public override Encoding ContentEncoding { get; set; }

        public override string ContentType { get; set; }

        public override int Expires { get; set; }

        public override DateTime ExpiresAbsolute { get; set; }

        public override Stream Filter { get; set; }

        public override Encoding HeaderEncoding { get; set; }

        public override bool IsClientConnected { get { return isClientConnected; } }

        public override bool IsRequestBeingRedirected { get { return isRequestBeingRedirected; } }

        public override TextWriter Output { get { return output; } }

        public override Stream OutputStream { get { return outputStream; } }

        public override string RedirectLocation { get; set; }

        public override string Status { get; set; }

        public override int StatusCode { get; set; }

        public override string StatusDescription { get; set; }

        public override int SubStatusCode { get; set; }

        public override bool SuppressContent { get; set; }

        public override bool TrySkipIisCustomErrors { get; set; }
    }
}

