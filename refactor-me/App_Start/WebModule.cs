using Ninject.Modules;
using Ninject.Web.Common;

using ProductAPI.Services;

namespace ProductAPI.App_Start
{
	public class WebModule : NinjectModule
	{
		public override void Load()
		{
			Bind<ProductDataContext>().ToSelf().InRequestScope();
			Bind<IProductOptionService>().To<ProductOptionService>();
			Bind<IProductService>().To<ProductService>();
		}
	}
}