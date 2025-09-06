using System.Reflection;
using MacroDeck.Server.Domain.Entities;
using MacroDeck.Server.Infrastructure.Interceptors;
using MacroDeck.Server.Infrastructure.Persistence.Configurations.Base;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Serilog.ILogger;

namespace MacroDeck.Server.Infrastructure.Persistence;

public class DatabaseContext : DbContext
{
	private readonly ILogger _logger;

	public DatabaseContext(ILogger logger)
	{
		_logger = logger;
	}

	public DbSet<FolderEntity> Folders => Set<FolderEntity>();

	public DbSet<WidgetEntity> Widgets => Set<WidgetEntity>();

	public DbSet<IntegrationConfigurationEntity> IntegrationConfigurations => Set<IntegrationConfigurationEntity>();

	protected override void OnConfiguring(DbContextOptionsBuilder options)
	{
		const string databasePath = "database.db";
		_logger.Information("Database path: {DbPath}", databasePath);
		var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = databasePath };
		var connection = new SqliteConnection(connectionStringBuilder.ToString());
		var loggerFactory = new LoggerFactory()
			.AddSerilog();
		options.UseSqlite(connection,
						  b => b.MigrationsAssembly(Assembly.GetExecutingAssembly()
															.GetName()
															.Name));
		options.UseLoggerFactory(loggerFactory);
		options.AddInterceptors(new SaveChangesSetTimestampsInterceptor());
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseCreatedEntityConfig<>).Assembly);
	}
}
