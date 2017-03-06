using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using ProductAPI.Services;
using ProductAPI.Infrastructure;
using ProductAPI.ExceptionHandler;
using ProductAPI.Controllers;

using ModelProduct = ProductAPI.Models.Product;
using ModelProducts = ProductAPI.Models.Products;
using ModelProductOption = ProductAPI.Models.ProductOption;
using ModelProductOptions = ProductAPI.Models.ProductOptions;

namespace ProductAPI.UnitTests.controllers
{
	[TestClass]
	public class ProductControllerTests
	{
		private Mock<IProductService> _productService;
		private Mock<IProductOptionService> _productOptionService;

		private const string PRODUCTID = "0f8fad5b-d9cb-469f-a165-70867728950e";
		private const string PRODUCTOPTIONID = "faf250b0-d0cf-4cd5-81db-34dc018c93d0";

		[TestInitialize]
		public void Setup()
		{
			AutoMappings.Configure();
		}

		#region "Product Tests"

		[TestMethod]
		public void CreateProduct_should_return_HttpRequestMessage_with_StatusCode_204()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();
			var product = new ModelProduct() { Id = Guid.Parse(PRODUCTID), Name = "My Product" };

			_productService.Setup(x => x.CreateProduct(It.IsAny<ModelProduct>())).Verifiable();

			var controller = new ProductsController(_productService.Object, _productOptionService.Object);
			controller.Request = new HttpRequestMessage();

			var response = controller.CreateProduct(product);

			Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
			_productService.Verify(x => x.CreateProduct(It.Is<ModelProduct>(p => p.Id.ToString() == PRODUCTID)));
		}

		[TestMethod]
		public void CreateProduct_should_return_an_exception_with_StatusCode_400()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();
			var product = new ModelProduct() { Id = Guid.Parse(PRODUCTID), Name = "My Product" };

			_productService.Setup(x => x.CreateProduct(It.IsAny<ModelProduct>())).Throws<GlobalException>();


			var controller = new ProductsController(_productService.Object, _productOptionService.Object);
			controller.Request = new HttpRequestMessage();

			try
			{
				controller.CreateProduct(product);
			}
			catch (HttpResponseException ex)
			{
				Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.BadRequest);
			}
		}

		[TestMethod]
		public void UpdateProduct_should_return_HttpRequestMessage_with_StatusCode_204()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();
			var product = new ModelProduct() { Id = Guid.Parse(PRODUCTID), Name = "My Product", Description = "Test Description" };
			var controller = new ProductsController(_productService.Object, _productOptionService.Object);
			controller.Request = new HttpRequestMessage();

			var response = controller.UpdateProduct(Guid.Parse(PRODUCTID), product);

			Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
		}

		[TestMethod]
		public void UpdateProduct_should_return_an_exception_with_StatusCode_400()
		{
			{
				_productService = new Mock<IProductService>();
				_productOptionService = new Mock<IProductOptionService>();
				var product = new ModelProduct() { Id = Guid.Parse(PRODUCTID), Name = "My Product", Description = "Test Description" };

				var controller = new ProductsController(_productService.Object, _productOptionService.Object);

				try
				{
					controller.UpdateProduct(Guid.NewGuid(), product);
				}
				catch (HttpResponseException ex)
				{
					Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.BadRequest);
				}
			}
		}

		[TestMethod]
		public void DeleteProduct_should_return_HttpRequestMessage_with_StatusCode_204()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();
			_productService.Setup(x => x.DeleteProduct(It.IsAny<Guid>())).Verifiable();
			_productOptionService.Setup(x => x.DeleteByProductId(It.IsAny<Guid>())).Verifiable();

			var controller = new ProductsController(_productService.Object, _productOptionService.Object);
			controller.Request = new HttpRequestMessage();

			var response = controller.DeleteProduct(Guid.Parse(PRODUCTID));

			Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
			_productService.Verify(x => x.DeleteProduct(It.Is<Guid>(id => id.ToString() == PRODUCTID)));
			_productOptionService.Verify(x => x.DeleteByProductId(It.Is<Guid>(id => id.ToString() == PRODUCTID)));
		}

		[TestMethod]
		public void DeleteProduct_should_return_an_exception_with_StatusCode_400()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();

			_productService.Setup(x => x.DeleteProduct(It.IsAny<Guid>())).Throws<GlobalException>();
			_productOptionService = new Mock<IProductOptionService>();

			var controller = new ProductsController(_productService.Object, _productOptionService.Object);

			try
			{
				controller.DeleteProduct(Guid.Parse(PRODUCTID));
			}
			catch (HttpResponseException ex)
			{
				Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.BadRequest);
			}
		}

		[TestMethod]
		public void GetAllProducts_should_return_ListOfProducts()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();

			var productsList = new List<ModelProduct>();
			productsList.Add(new ModelProduct() { Id = Guid.NewGuid(), Name = "My product" });
			var products = new ModelProducts { Items = productsList };

			_productService.Setup(x => x.GetProducts()).Returns(products);

			var controller = new ProductsController(_productService.Object, _productOptionService.Object);
			var response = controller.GetAllProducts();

			Assert.AreEqual(response.Items.Count, 1);
		}

		[TestMethod]
		public void SearchByName_should_return_a_list_of_expected_products()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();

			var productsList = new List<ModelProduct>();
			productsList.Add(new ModelProduct() { Id = Guid.Parse(PRODUCTID), Name = "My product" });
			var products = new ModelProducts { Items = productsList };

			_productService.Setup(x => x.GetProductByName(It.IsAny<string>())).Returns(products);
			var controller = new ProductsController(_productService.Object, _productOptionService.Object);

			var response = controller.SearchByName("My product");

			Assert.AreEqual(response.Items.Count, 1);
			Assert.AreEqual(response.Items[0].Id.ToString(), PRODUCTID);
		}

		[TestMethod]
		public void GetProduct_Should_return_expected_product()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();

			var product = new ModelProduct() { Id = Guid.Parse(PRODUCTID), Name = "My product" };
			_productService.Setup(x => x.GetProductById(It.IsAny<Guid>())).Returns(product);
			
			var controller = new ProductsController(_productService.Object, _productOptionService.Object);
			var response = controller.GetProduct(Guid.Parse(PRODUCTID));

			Assert.AreEqual(response.Id.ToString(), PRODUCTID);
		}

		[TestMethod]
		public void GetProduct_Should_return_an_exception_if_product_not_found()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();
			_productService.Setup(x => x.GetProductById(It.IsAny<Guid>())).Returns((ModelProduct) null);
			
			var controller = new ProductsController(_productService.Object, _productOptionService.Object);

			try
			{
				controller.GetProduct(Guid.Parse(PRODUCTID));
			}
			catch (HttpResponseException ex)
			{
				Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.NotFound);
			}
		}

		#endregion


		#region "Product Option Tests"

		[TestMethod]
		public void CreateOption_should_return_HttpRequestMessage_with_StatusCode_204()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();
			var option = new ModelProductOption()
			{
				Id = Guid.Parse(PRODUCTOPTIONID),
				Name = "My Option",
				ProductId = Guid.Parse(PRODUCTID)
			};

			_productOptionService.Setup(x => x.Create(It.IsAny<ModelProductOption>())).Verifiable();
			var controller = new ProductsController(_productService.Object, _productOptionService.Object);
			controller.Request = new HttpRequestMessage();

			var response = controller.CreateOption(Guid.Parse(PRODUCTID), option);

			Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
			_productOptionService.Verify(x => x.Create(It.Is<ModelProductOption>(p => p.Id.ToString() == PRODUCTOPTIONID)));
		}

		[TestMethod]
		public void CreateOption_should_throw_an_exception_if_product_id_is_mismatched()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();
			var option = new ModelProductOption()
			{
				Id = Guid.Parse(PRODUCTOPTIONID),
				Name = "My Option",
				ProductId = Guid.Parse(PRODUCTID)
			};

			var controller = new ProductsController(_productService.Object, _productOptionService.Object);

			try
			{
				controller.CreateOption(Guid.NewGuid(), option);
			}
			catch (HttpResponseException ex)
			{
				Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.BadRequest);
			}
		}

		[TestMethod]
		public void UpdateOption_should_return_HttpRequestMessage_with_StatusCode_204()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();
			var option = new ModelProductOption()
			{
				Id = Guid.Parse(PRODUCTOPTIONID),
				Name = "My Option",
				ProductId = Guid.Parse(PRODUCTID)
			};

			_productOptionService.Setup(x => x.Update(It.IsAny<ModelProductOption>())).Verifiable();

			var controller = new ProductsController(_productService.Object, _productOptionService.Object);
			controller.Request = new HttpRequestMessage();
			var response = controller.UpdateOption(Guid.Parse(PRODUCTID), Guid.Parse(PRODUCTOPTIONID), option);

			Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
			_productOptionService.Verify(x => x.Update(It.Is<ModelProductOption>(p => p.Id.ToString() == PRODUCTOPTIONID)));
		}

		[TestMethod]
		public void UpdateOption_should_return_an_exception_if_mismatched_productId_provided()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();
			var option = new ModelProductOption()
			{
				Id = Guid.Parse(PRODUCTOPTIONID),
				Name = "My Option",
				ProductId = Guid.Parse(PRODUCTID)
			};

			var controller = new ProductsController(_productService.Object, _productOptionService.Object);

			try
			{
				controller.UpdateOption(Guid.NewGuid(), Guid.Parse(PRODUCTOPTIONID), option);
			}
			catch (HttpResponseException ex)
			{
				Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.BadRequest);
			}
		}

		public void UpdateOption_should_return_an_exception_if_mismatched_OptionId_provided()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();
			var option = new ModelProductOption()
			{
				Id = Guid.Parse(PRODUCTOPTIONID),
				Name = "My Option",
				ProductId = Guid.Parse(PRODUCTID)
			};

			var controller = new ProductsController(_productService.Object, _productOptionService.Object);

			try
			{
				controller.UpdateOption(Guid.Parse(PRODUCTID), Guid.NewGuid(), option);
			}
			catch (HttpResponseException ex)
			{
				Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.BadRequest);
			}
		}

		public void UpdateOption_should_return_an_exception_if_product_option_not_found()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();
			var option = new ModelProductOption()
			{
				Id = Guid.Parse(PRODUCTOPTIONID),
				Name = "My Option",
				ProductId = Guid.Parse(PRODUCTID)
			};

			_productOptionService.Setup(x => x.Update(It.IsAny<ModelProductOption>())).Throws<GlobalException>();

			var controller = new ProductsController(_productService.Object, _productOptionService.Object);

			try
			{
				controller.UpdateOption(Guid.Parse(PRODUCTID), Guid.Parse(PRODUCTOPTIONID), option);
			}
			catch (HttpResponseException ex)
			{
				Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.NotFound);
			}
		}

		[TestMethod]
		public void DeleteOptionById_should_return_HttpRequestMessage_with_StatusCode_204()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();
			_productOptionService.Setup(x => x.DeleteById(It.IsAny<Guid>())).Verifiable();

			var controller = new ProductsController(_productService.Object, _productOptionService.Object);
			controller.Request = new HttpRequestMessage();

			controller.DeleteOptionById(Guid.Parse(PRODUCTOPTIONID));

			_productOptionService.Verify(x => x.DeleteById(It.Is<Guid>(id => id.ToString() == PRODUCTOPTIONID)));
		}

		[TestMethod]
		public void DeleteOptionById_should_return_an_exception_if_option_is_not_available()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();

			_productOptionService.Setup(x => x.DeleteById(It.IsAny<Guid>())).Throws<GlobalException>();

			var controller = new ProductsController(_productService.Object, _productOptionService.Object);

			try
			{
				controller.DeleteOptionById(Guid.Parse(PRODUCTOPTIONID));
			}
			catch (HttpResponseException ex)
			{
				Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.NotFound);
			}
		}

		[TestMethod]
		public void GetOptions_should_return_HttpRequestMessage_with_StatusCode_204()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();

			var optionsList = new List<ModelProductOption>();
				optionsList.Add(
					new ModelProductOption()
					{
						Id = Guid.Parse(PRODUCTOPTIONID),
						Name = "My Option1",
						ProductId = Guid.Parse(PRODUCTID)
					});
				optionsList.Add(
					new ModelProductOption()
					{
						Id = Guid.NewGuid(),
						Name = "My Option2",
						ProductId = Guid.Parse(PRODUCTID)
					});
			var options = new ModelProductOptions { Items = optionsList };

			_productOptionService.Setup(x => x.GetOptionsByProductId(It.IsAny<Guid>())).Returns(options);

			var controller = new ProductsController(_productService.Object, _productOptionService.Object);
			var response = controller.GetOptions(Guid.Parse(PRODUCTID));

			Assert.AreEqual(response.Items.Count, 2);
			Assert.AreEqual(response.Items[0].Id.ToString(), PRODUCTOPTIONID);
			Assert.AreEqual(response.Items[0].ProductId.ToString(), PRODUCTID);
			Assert.AreEqual(response.Items[1].ProductId.ToString(), PRODUCTID);
		}

		[TestMethod]
		public void GetOptionById_should_return_a_single_option_based_on_productId_and_optionId()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();
			var option = new ModelProductOption()
			{
				Id = Guid.Parse(PRODUCTOPTIONID),
				Name = "My Option1",
				ProductId = Guid.Parse(PRODUCTID)
			};

			_productOptionService.Setup(x => x.GetSingleOptionByProductId(Guid.Parse(PRODUCTID), Guid.Parse(PRODUCTOPTIONID))).Returns(option);

			var controller = new ProductsController(_productService.Object, _productOptionService.Object);
			var response = controller.GetOptionById(Guid.Parse(PRODUCTID), Guid.Parse(PRODUCTOPTIONID));

			Assert.AreEqual(response.Id.ToString(), PRODUCTOPTIONID);
			Assert.AreEqual(response.ProductId.ToString(), PRODUCTID);
		}

		[TestMethod]
		public void GetOptionById_should_return_an_exception_if_option_is_null()
		{
			_productService = new Mock<IProductService>();
			_productOptionService = new Mock<IProductOptionService>();
			_productOptionService.Setup(x => x.GetSingleOptionByProductId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns((ModelProductOption) null);

			var controller = new ProductsController(_productService.Object, _productOptionService.Object);

			try
			{
				controller.GetOptionById(Guid.Parse(PRODUCTID), Guid.Parse(PRODUCTOPTIONID));
			}
			catch (HttpResponseException ex)
			{
				Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.NotFound);
			}
		}

		#endregion
	}
}
