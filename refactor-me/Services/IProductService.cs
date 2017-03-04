using System;

using ProductAPI.Models;

namespace ProductAPI.Services
{
	public interface IProductService
	{
		void CreateProduct(Product product);

		void UpdateProduct(Guid id, Product product);

		void DeleteProduct(Guid id);

		Products GetProducts();

		Products GetProductByName(string name);

		Product GetProductById(Guid id);
	}
}
