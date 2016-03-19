using System.Web;
using System.Web.Routing;

namespace VpnSite.Helpers
{

	public interface IHttpHandlerBase : IHttpHandler
	{
		RequestContext RequestContext {get; set; }
	}

	/// <summary>
	/// Generic route handler.
	/// </summary>
	/// <remarks>>See http://petesdotnet.blogspot.ca/2009/09/generic-handlers-and-aspnet-routing.html for details</remarks>
	public class GenericRouteHandler<T> : IRouteHandler
		where T : IHttpHandlerBase, new()
	{
		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			var retVal = new T();
			retVal.RequestContext = requestContext;
			return retVal;
		}
	} 


}

