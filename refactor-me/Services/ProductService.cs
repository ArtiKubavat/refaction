using System;
using System.Linq;
using System.Data.Entity;

using Domain = ProductAPI.Models;
using ProductAPI.Data;
using ProductAPI.ExceptionHandler;
using AutoMapper;
using System.Web.Http;
using System.Net;

namespace ProductAPI.Services
{
	public class ProductService : IProductService
	{
		private readonly ProductDataContext _dbContext;

		public ProductService(ProductDataContext dbContext)
		{
			_dbContext = dbContext;
		}

		public void CreateProduct(Domain.Product product)
		{
			var domainProduct = Mapper.Map<Product>(product);
			var duplicateProduct = _dbContext.Products.Find(product.Id);
			if (duplicateProduct != null)
			{
				throw new GlobalException("Duplicate Product Id found.");
			}

			if(domainProduct.Id == Guid.Empty)
			{
				domainProduct.Id =  Guid.NewGuid();
			}

			_dbContext.Products.Add(domainProduct);
			_dbContext.SaveChanges();
		}

		public void DeleteProduct(Guid id)
		{
			var product = _dbContext.Products.Find(id);

			if (product == null)
			{
				throw new GlobalException("Product not available.");
			}
			_dbContext.Products.Remove(product);
			_dbContext.SaveChanges();
		}

		public void UpdateProduct(Guid productId, Domain.Product updatedProduct)
		{
			var product = _dbContext.Products.SingleOrDefault(x => x.Id == productId);

			if (product == null)
			{
				throw new GlobalException("Product not available.");
			}

			_dbContext.UpdateEntity(product, Mapper.Map<Product>(updatedProduct));
			_dbContext.SaveChanges();
		}

		public Domain.Product GetProductById(Guid id)
		{
			var product = _dbContext.Products.Find(id);

			return Mapper.Map<Domain.Product>(product);
		}

		public Domain.Products GetProductByName(string name)
		{
			var products = _dbContext.Products.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();

			return Mapper.Map<Domain.Products>(products);
		 
		}

		public Domain.Products GetProducts()
		{
			var products = _dbContext.Products.ToList();

			return Mapper.Map<Domain.Products>(products);
		}

	}
}
