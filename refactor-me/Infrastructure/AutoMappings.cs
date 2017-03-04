using System;
using System.Collections.Generic;
using AutoMapper;

using Domain = ProductAPI.Models;
using ProductAPI.Data;

namespace ProductAPI.Infrastructure
{
	public static class AutoMappings
	{
		public static void Configure()
		{
			Mapper.Initialize(cfg =>
			{
				cfg.CreateMap<Product, Domain.Product>();
				cfg.CreateMap<Domain.Product, Product>();

				cfg.CreateMap<Domain.Products, List<Product>>()
					.ConstructUsing(x => Mapper.Map<List<Product>>(x.Items));
				cfg.CreateMap<List<Product>, Domain.Products>()
					.ConstructUsing(x => new Domain.Products() {
						Items = Mapper.Map<List<Domain.Product>>(x)
					});
				

				cfg.CreateMap<ProductOption, Domain.ProductOption>();
				cfg.CreateMap<Domain.ProductOption, ProductOption>();

				cfg.CreateMap<Domain.ProductOptions, List<ProductOption>>()
					.ConstructUsing(x => Mapper.Map<List<ProductOption>>(x.Items));
				cfg.CreateMap<List<ProductOption>, Domain.ProductOptions>()
					.ConstructUsing(x => new Domain.ProductOptions() {
						Items = Mapper.Map<List<Domain.ProductOption>>(x)
					});
			});
		}
	}
}