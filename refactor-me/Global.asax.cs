using System.Web.Http;

using ProductAPI.Infrastructure;

namespace ProductAPI
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			GlobalConfiguration.Configure(WebApiConfig.Register);
			AutoMappings.Configure();
		}
	}
}
