using EasyUiBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EasyUiBackend.Infrastructure;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
	public AppDbContext CreateDbContext(string[] args)
	{
		var config = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json")
			.Build();

		var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
		var connectionString = config.GetConnectionString("DefaultConnection");

		optionsBuilder.UseNpgsql(connectionString);

		return new AppDbContext(optionsBuilder.Options);
	}
}
