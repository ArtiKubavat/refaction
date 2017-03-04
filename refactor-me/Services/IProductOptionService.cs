using System;

using ProductAPI.Models;

namespace ProductAPI.Services
{
	public interface IProductOptionService
	{
		void Create(ProductOption option);

		void Update(ProductOption option);

		void DeleteById(Guid id);

		void DeleteByProductId(Guid id);

		ProductOptions GetOptionsByProductId(Guid productId);

		ProductOption GetSingleOptionByProductId(Guid productId, Guid id);
	}
}
