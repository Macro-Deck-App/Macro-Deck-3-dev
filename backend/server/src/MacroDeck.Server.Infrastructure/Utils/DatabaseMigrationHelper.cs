using EvolveDb;
using Microsoft.Data.Sqlite;
using Serilog;

namespace MacroDeck.Server.Infrastructure.Utils;

public static class DatabaseMigrationHelper
{
	public static void MigrateDatabase()
	{
		try
		{
			const string databasePath = "database.db";
			var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = databasePath };
			var connection = new SqliteConnection(connectionStringBuilder.ToString());
			var evolve = new Evolve(connection, Log.Information)
						 {
							 Locations = ["DatabaseMigrations"],
							 IsEraseDisabled = true,
							 Schemas = ["database_migration"]
						 };

			evolve.Migrate();
		}
		catch (Exception ex)
		{
			Log.Fatal(ex, "Database migration failed");
			throw;
		}
	}
}
