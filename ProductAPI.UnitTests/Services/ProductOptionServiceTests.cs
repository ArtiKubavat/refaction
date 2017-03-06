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

using ModelProductOption = ProductAPI.Models.ProductOption;



namespace ProductAPI.UnitTests.Services
{
	[TestClass]
	public class ProductOptionServiceTests
	{
		private Mock<ProductDataContext> _dbContextMock;
		private List<ProductOption> productOptionList;

		private const string PRODUCTID = "0f8fad5b-d9cb-469f-a165-70867728950e";
		private const string PRODUCTOPTIONID = "faf250b0-d0cf-4cd5-81db-34dc018c93d0";
		

		[TestInitialize]
		public void SetUp()
		{
			AutoMappings.Configure();

			productOptionList = new List<ProductOption>
			{
				new ProductOption {
					Id = Guid.NewGuid(),
					ProductId = Guid.NewGuid(),
					Name = "ProductOption1",
					Description = "Newest ProductOption1 from Test Project."
				},
				new ProductOption {
					Id = Guid.NewGuid(),
					ProductId = Guid.NewGuid(),
					Name = "ProductOption2"
				},
				new ProductOption {
					Id = Guid.Parse(PRODUCTOPTIONID),
					ProductId = Guid.Parse(PRODUCTID),
					Name = "ProductOption3"
				},
				
			};

			var productOptionsDb = productOptionList.AsQueryable();

			var mockSet = new Mock<DbSet<ProductOption>>();
			mockSet.As<IQueryable<ProductOption>>().Setup(m => m.Provider).Returns(productOptionsDb.Provider);
			mockSet.As<IQueryable<ProductOption>>().Setup(m => m.Expression).Returns(productOptionsDb.Expression);
			mockSet.As<IQueryable<ProductOption>>().Setup(m => m.ElementType).Returns(productOptionsDb.ElementType);
			mockSet.As<IQueryable<ProductOption>>().Setup(m => m.GetEnumerator()).Returns(productOptionsDb.GetEnumerator());

			mockSet.Setup(x => x.Find(It.IsAny<object[]>())).Returns((object[] input) => productOptionsDb.FirstOrDefault(d => d.Id == Guid.Parse(input[0].ToString())));
			mockSet.Setup(x => x.Add(It.IsAny<ProductOption>())).Callback((ProductOption product) => productOptionList.Add(product));
			mockSet.Setup(x => x.RemoveRange(It.IsAny<IEnumerable<ProductOption>>())).Callback((IEnumerable<ProductOption> range) => range.ToList().ForEach(x => productOptionList.Remove(x)));
			mockSet.Setup(x => x.Remove(It.IsAny<ProductOption>())).Callback((ProductOption product) => productOptionList.Remove(product));


			_dbContextMock = new Mock<ProductDataContext>();
			_dbContextMock.Setup(c => c.ProductOptions).Returns(mockSet.Object);
			_dbContextMock.Setup(c => c.UpdateEntity(It.IsAny<ProductOption>(), It.IsAny<ProductOption>())).Callback((ProductOption original, ProductOption modified) =>
			{
				productOptionList.Remove(original);
				productOptionList.Add(modified);
			});

		}

		[TestMethod]
		public void Create_should_execute_successfully()
		{
			var optionService = new ProductOptionService(_dbContextMock.Object);
			var productOption = new ModelProductOption { Id = Guid.NewGuid(), ProductId= Guid.Parse(PRODUCTID), Name = "ProductOptionNew" };

			optionService.Create(productOption);
			Assert.AreEqual(productOptionList.Count, 4);
		}

		[TestMethod]
		public void Create_should_throw_duplicate_id_found_exception()
		{
			var optionService = new ProductOptionService(_dbContextMock.Object);
			var productOption = new ModelProductOption { Id = Guid.Parse(PRODUCTOPTIONID), ProductId = Guid.Parse(PRODUCTID), Name = "ProductOption1" };

			try
			{
				optionService.Create(productOption);
			}
			catch (GlobalException ex)
			{
				Assert.AreEqual(ex.Message, "Duplicate Product Option Id found.");
			}
		}

		[TestMethod]
		public void Update_should_execute_successfully()
		{
			var optionService = new ProductOptionService(_dbContextMock.Object);
			var productOption = new ModelProductOption { Id = Guid.Parse(PRODUCTOPTIONID), ProductId = Guid.Parse(PRODUCTID), Name = "ProductOptionUpdated" };

			optionService.Update(productOption);
			Assert.AreEqual(productOptionList.Count, 3);
			Assert.AreEqual(productOptionList.Find(x => x.Id.ToString() == PRODUCTOPTIONID).Name, "ProductOptionUpdated");
		}

		[TestMethod]
		public void Update_should_throw_productOption_unavailable_exception()
		{
			var productOption = new ModelProductOption { Id = Guid.NewGuid(), ProductId = Guid.Parse(PRODUCTID), Name = "ProductOptionNew" };
			
			var optionService = new ProductOptionService(_dbContextMock.Object);

			try
			{
				optionService.Update(productOption);
			}
			catch (GlobalException ex)
			{
				Assert.AreEqual(ex.Message, "Product Option not available.");
				Assert.AreEqual(productOptionList.Count, 3);
			}
		}

		[TestMethod]
		public void DeleteById_should_execute_successfully()
		{
			var optionService = new ProductOptionService(_dbContextMock.Object);

			optionService.DeleteById(Guid.Parse(PRODUCTOPTIONID));

			Assert.AreEqual(productOptionList.Count, 2);
		}

		[TestMethod]
		public void DeleteById_should_throw_productOption_unavailble_exception()
		{
			var optionService = new ProductOptionService(_dbContextMock.Object);

			try
			{
				optionService.DeleteById(Guid.NewGuid());
			}
			catch (GlobalException ex)
			{
				Assert.AreEqual(ex.Message, "Product Option not available.");
				Assert.AreEqual(productOptionList.Count, 3);
			}
		}

		[TestMethod]
		public void DeleteByProductId_should_execute_successfully()
		{
			var optionService = new ProductOptionService(_dbContextMock.Object);

			optionService.DeleteByProductId(Guid.Parse(PRODUCTID));

			Assert.AreEqual(productOptionList.Count, 2);
		}

		[TestMethod]
		public void GetOptionsByProductId_should_return_productOption()
		{
			var optionService = new ProductOptionService(_dbContextMock.Object);
			var result = optionService.GetOptionsByProductId(Guid.Parse(PRODUCTID));

			Assert.AreEqual(result.Items.Count, 1);
		}

		[TestMethod]
		public void GetSingleOptionByProductId_should_return_productOption()
		{
			var optionService = new ProductOptionService(_dbContextMock.Object);
			var result = optionService.GetSingleOptionByProductId(Guid.Parse(PRODUCTID), Guid.Parse(PRODUCTOPTIONID));

			Assert.AreEqual(result.ProductId.ToString(), PRODUCTID);
			Assert.AreEqual(result.Id.ToString(), PRODUCTOPTIONID);
		}
	}
}
