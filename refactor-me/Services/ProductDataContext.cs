namespace ProductAPI.Services
{
	using System.Data.Entity;

	using Data;

	public partial class ProductDataContext : DbContext
	{
		public ProductDataContext()
			: base("name=ProductDataContext")
		{
		}

		public virtual DbSet<Product> Products { get; set; }
		public virtual DbSet<ProductOption> ProductOptions { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
		}
		public virtual void UpdateEntity<T>(T original, T modified) where T : class
		{
			Entry(original).CurrentValues.SetValues(modified);
		}
	}
}
