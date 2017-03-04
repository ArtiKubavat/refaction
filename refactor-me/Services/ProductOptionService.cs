using System;
using System.Linq;
using System.Data.Entity;

using Domain = ProductAPI.Models;
using AutoMapper;
using ProductAPI.Data;

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
				throw new Exception("Duplicate Product Option Id found.");
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
			var domainOption = Mapper.Map<Domain.ProductOption>(option);
			var productOption = _dbContext.ProductOptions.Find(domainOption.Id);

			if (productOption == null)
			{
				throw new Exception("Product Option not available.");
			}

			_dbContext.Entry(productOption).State = EntityState.Modified;
			_dbContext.SaveChanges();
		}

		public void DeleteById(Guid id)
		{
			var productOption = _dbContext.ProductOptions.Find(id);
			if (productOption == null)
			{
				throw new Exception("Product Option not available.");
			}
			_dbContext.ProductOptions.Remove(productOption);
			_dbContext.SaveChanges();
		}

		public void DeleteByProductId(Guid id)
		{
			_dbContext.ProductOptions.RemoveRange(_dbContext.ProductOptions.Where(x => x.ProductId == id).ToList());
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