using System;
using System.Linq;
using AutoMapper;

using Domain = ProductAPI.Models;

using ProductAPI.Data;
using ProductAPI.ExceptionHandler;

namespace ProductAPI.Services
{
	public class ProductOptionService : IProductOptionService
	{
		private readonly ProductDataContext _dbContext;

		public ProductOptionService(ProductDataContext dbContext)
		{
			_dbContext = dbContext;
		}

		public void Create(Domain.ProductOption option)
		{
			var productOption = _dbContext.ProductOptions.Find(option.Id);
			var domainOption = Mapper.Map<ProductOption>(option);

			if (productOption != null)
			{
				throw new GlobalException("Duplicate Product Option Id found.");
			}

			if (domainOption.Id == Guid.Empty)
			{
				domainOption.Id = Guid.NewGuid();
			}

			_dbContext.ProductOptions.Add(domainOption);
			_dbContext.SaveChanges();
		}

		public void Update(Domain.ProductOption option)
		{
			var domainOption = Mapper.Map<ProductOption>(option);
			var productOption = _dbContext.ProductOptions.Find(domainOption.Id);

			if (productOption == null)
			{
				throw new GlobalException("Product Option not available.");
			}

			_dbContext.UpdateEntity(productOption, domainOption);
			_dbContext.SaveChanges();
		}

		public void DeleteById(Guid id)
		{
			var productOption = _dbContext.ProductOptions.Find(id);
			if (productOption == null)
			{
				throw new GlobalException("Product Option not available.");
			}
			_dbContext.ProductOptions.Remove(productOption);
			_dbContext.SaveChanges();
		}

		public void DeleteByProductId(Guid id)
		{
			var options =_dbContext.ProductOptions.Where(x => x.ProductId == id).ToList();

			_dbContext.ProductOptions.RemoveRange(options);
			_dbContext.SaveChanges();
		}

		public Domain.ProductOptions GetOptionsByProductId(Guid productId)
		{
			var options = _dbContext.ProductOptions.Where(x => x.ProductId == productId).ToList();
			return Mapper.Map<Domain.ProductOptions>(options);
		}

		public Domain.ProductOption GetSingleOptionByProductId(Guid productId, Guid id)
		{
			var option = _dbContext.ProductOptions.FirstOrDefault(x => x.Id == id && x.ProductId == productId);
			return Mapper.Map<Domain.ProductOption>(option);
		}
		
	}
}