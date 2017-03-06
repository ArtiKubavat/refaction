using System;
using System.Net;
using System.Web.Http;

using ProductAPI.Models;
using ProductAPI.Services;
using System.Net.Http;
using ProductAPI.ExceptionHandler;

namespace ProductAPI.Controllers
{
	[RoutePrefix("products")]
	public class ProductsController : ApiController
	{

		private readonly IProductService _productService;
		private readonly IProductOptionService _productOptionService;

		public ProductsController(IProductService productService, IProductOptionService productOptionService)
		{
			_productService = productService;
			_productOptionService = productOptionService;
		}


		#region "Products"
		[Route]
		[HttpPost]
		public HttpResponseMessage CreateProduct(Product product)
		{
			try
			{
				_productService.CreateProduct(product);
			}
			catch (GlobalException)
			{
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}

			return Request.CreateResponse(HttpStatusCode.NoContent);
		}

		[Route("{id}")]
		[HttpPut]
		public HttpResponseMessage UpdateProduct(Guid id, Product product)
		{
			if (product.Id != id)
			{
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}
			_productService.UpdateProduct(id, product);
			return Request.CreateResponse(HttpStatusCode.NoContent);
		}

		[Route("{id}")]
		[HttpDelete]
		public HttpResponseMessage DeleteProduct(Guid id)
		{try
			{
				_productService.DeleteProduct(id);
				_productOptionService.DeleteByProductId(id);
			}
			catch (GlobalException)
			{
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}

			return Request.CreateResponse(HttpStatusCode.NoContent);
		}

		[Route]
		[HttpGet]
		public Products GetAllProducts()
		{
			return _productService.GetProducts();
		}

		[Route]
		[HttpGet]
		public Products SearchByName(string name)
		{
			return _productService.GetProductByName(name);
		}

		[Route("{id}")]
		[HttpGet]
		public Product GetProduct(Guid id)
		{
			var product = _productService.GetProductById(id);
			if (product == null)
			{
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}

			return product;
		}

		#endregion

		#region "Product Options"

		[Route("{productId}/options")]
		[HttpPost]
		public HttpResponseMessage CreateOption(Guid productId, ProductOption option)
		{
			if (option.ProductId != productId)
			{
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}
			_productOptionService.Create(option);

			return Request.CreateResponse(HttpStatusCode.NoContent);

		}

		[Route("{productId}/options/{id}")]
		[HttpPut]
		public HttpResponseMessage UpdateOption(Guid productId, Guid id, ProductOption option)
		{
			if (option.ProductId != productId || option.Id != id)
			{
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}

			try
			{
				_productOptionService.Update(option);
			}
			catch(GlobalException)
			{
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}

			return Request.CreateResponse(HttpStatusCode.NoContent);
		}

		[Route("{productId}/options/{id}")]
		[HttpDelete]
		public HttpResponseMessage DeleteOptionById(Guid id)
		{
			try
			{
				_productOptionService.DeleteById(id);
			}
			catch (GlobalException)
			{
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}
			return Request.CreateResponse(HttpStatusCode.NoContent);
		}

		[Route("{productId}/options")]
		[HttpGet]
		public ProductOptions GetOptions(Guid productId)
		{
			return _productOptionService.GetOptionsByProductId(productId);
		}

		[Route("{productId}/options/{id}")]
		[HttpGet]
		public ProductOption GetOptionById(Guid productId, Guid id)
		{
			var option = _productOptionService.GetSingleOptionByProductId(productId, id);
			if (option == null)
			{
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}

			return option;
		}

		
		#endregion

	}
}
