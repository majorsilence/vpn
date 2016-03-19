using System;
using System.Collections.Specialized;
using System.Web;

namespace MvcFakes
{

    public class FakeHttpRequest : HttpRequestBase
    {
        private readonly NameValueCollection _formParams;
        private readonly NameValueCollection _queryStringParams;
        private NameValueCollection _headers;
        private readonly HttpCookieCollection _cookies;

        public FakeHttpRequest(NameValueCollection formParams, NameValueCollection queryStringParams, HttpCookieCollection cookies,
                               NameValueCollection headers)
        {
            _formParams = formParams;
            _queryStringParams = queryStringParams;
            _cookies = cookies;
            _headers = headers;
        }

        public override NameValueCollection Form
        {
            get
            {
                return _formParams;
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

        public override NameValueCollection QueryString
        {
            get
            {
                return _queryStringParams;
            }
        }

        public override HttpCookieCollection Cookies
        {
            get
            {
                return _cookies;
            }
        }

    }



}
