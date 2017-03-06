using System;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using ProductAPI.Data;
using ProductAPI.Services;
using ProductAPI.Infrastructure;
using ProductAPI.ExceptionHandler;

using ModelProduct = ProductAPI.Models.Product;

namespace ProductAPI.UnitTests.Services
{
	[TestClass]
	public class ProductServiceTests
	{
		private List<Product> productList;
		private Mock<ProductDataContext> _dbContextMock;

		private const string PRODUCTID = "0f8fad5b-d9cb-469f-a165-70867728950e";


		[TestInitialize]
		public void SetUp()
		{
			AutoMappings.Configure();

			productList = new List<Product>
			{
				new Product {
					Id = Guid.Parse(PRODUCTID),
					Name = "Product1",
					Description= "Newest Product1 from Test Project.",
					Price = decimal.Parse("104.99"),
					DeliveryPrice = decimal.Parse("1.99")
				},
				new Product {
					Id = Guid.NewGuid(),
					Name = "Product2",
					Description= "Newest Product1 from Test Project.",
					Price = decimal.Parse("1024.99"),
					DeliveryPrice = decimal.Parse("16.99")
				}
			};

			var productsDb = productList.AsQueryable();

			var mockSet = new Mock<DbSet<Product>>();
			mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(productsDb.Provider);
			mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(productsDb.Expression);
			mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(productsDb.ElementType);
			mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(productsDb.GetEnumerator());

			mockSet.Setup(x => x.Find(It.IsAny<object[]>())).Returns((object[] input) => productsDb.FirstOrDefault(d => d.Id == Guid.Parse(input[0].ToString())));
			mockSet.Setup(x => x.Add(It.IsAny<Product>())).Callback((Product product) => productList.Add(product));
			mockSet.Setup(x => x.Remove(It.IsAny<Product>())).Callback((Product product) => productList.Remove(product));

			_dbContextMock = new Mock<ProductDataContext>();
			_dbContextMock.Setup(c => c.Products).Returns(mockSet.Object);
		}

		[TestMethod]
		public void CreateProduct_should_execute_successfully()
		{
			var productService = new ProductService(_dbContextMock.Object);
			var product = new ModelProduct { Id = Guid.NewGuid() , Name = "Item" };

			productService.CreateProduct(product);
			Assert.AreEqual(productList.Count, 3);
		}

		[TestMethod]
		public void CreateProduct_should_throw_duplicate_product_found_exception()
		{
			var productService = new ProductService(_dbContextMock.Object);
			var product = new ModelProduct { Id = Guid.Parse(PRODUCTID), Name = "Product" };

			try
			{
				productService.CreateProduct(product);
			}
			catch (GlobalException ex)
			{
				Assert.AreEqual(ex.Message, "Duplicate Product Id found.");
				Assert.AreEqual(productList.Count, 2);
			}
		}

		[TestMethod]
		public void DeleteProduct_should_execute_successfully()
		{
			var productService = new ProductService(_dbContextMock.Object);

			productService.DeleteProduct(Guid.Parse(PRODUCTID));

			Assert.AreEqual(productList.Count, 1);
		}

		[TestMethod]
		public void DeleteProduct_should_throw_product_unavailable_exception()
		{
			var productService = new ProductService(_dbContextMock.Object);

			try
			{
				productService.DeleteProduct(Guid.NewGuid());
			}
			catch (GlobalException ex)
			{
				Assert.AreEqual(ex.Message, "Product not available.");
				Assert.AreEqual(productList.Count, 2);
			}
		}

		[TestMethod]
		public void UpdateProduct_should_execute_successfully()
		{
			var productService = new ProductService(_dbContextMock.Object);
			var product = new ModelProduct { Id = Guid.Parse(PRODUCTID), Name = "UpdatedProduct" };

			_dbContextMock.Setup(c => c.UpdateEntity(It.IsAny<Product>(), It.IsAny<Product>())).Callback((Product original, Product modified) =>
			{
				productList.Remove(original);
				productList.Add(modified);
			});

			productService.UpdateProduct(product.Id, product);
			Assert.AreEqual(productList.Find(x => x.Id.ToString() == PRODUCTID).Name, "UpdatedProduct");
		}

		[TestMethod]
		public void UpdateProduct_should_throw_product_unavailable_exception()
		{
			var productService = new ProductService(_dbContextMock.Object);
			var product = new ModelProduct { Id = Guid.NewGuid(), Name = "UpdatedProduct" };

			try
			{
				productService.UpdateProduct(product.Id, product);
			}
			catch (GlobalException ex)
			{
				Assert.AreEqual(ex.Message, "Product not available.");
			}
		}

		[TestMethod]
		public void GetProductById_should_return_product()
		{
			var productService = new ProductService(_dbContextMock.Object);
			var result = productService.GetProductById(Guid.Parse(PRODUCTID));

			Assert.AreEqual(result.Id.ToString(), PRODUCTID);
		}

		[TestMethod]
		public void GetProducts_should_return_ProductList()
		{
			var productService = new ProductService(_dbContextMock.Object);
			var result = productService.GetProducts();

			Assert.AreEqual(result.Items.Count, 2);
		}

		[TestMethod]
		public void GetProductByName_should_return_product()
		{
			var productService = new ProductService(_dbContextMock.Object);
			var result = productService.GetProductByName("Product2");
			
			Assert.AreEqual(result.Items[0].Name, "Product2");
		}
	}

}
