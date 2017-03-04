namespace ProductAPI.Data
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("Product")]
	public partial class Product
	{
		public Guid Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; }

		[StringLength(500)]
		public string Description { get; set; }

		public decimal Price { get; set; }

		public decimal DeliveryPrice { get; set; }
	}
}
