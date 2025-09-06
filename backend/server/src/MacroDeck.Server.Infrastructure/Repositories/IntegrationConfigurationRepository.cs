using MacroDeck.Server.Application.Repositories;
using MacroDeck.Server.Domain.Entities;
using MacroDeck.Server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MacroDeck.Server.Infrastructure.Repositories;

public class IntegrationConfigurationRepository : IIntegrationConfigurationRepository
{
	private readonly DatabaseContext _context;

	public IntegrationConfigurationRepository(DatabaseContext context)
	{
		_context = context;
	}

	public Task<IntegrationConfigurationEntity?> GetConfigurationByIntegrationIdAndKey(string integrationId, string key)
	{
		return _context.IntegrationConfigurations.FirstOrDefaultAsync(x => x.IntegrationId == integrationId
																		   && x.ConfigurationKey == key);
	}

	public async Task CreateConfiguration(IntegrationConfigurationEntity integrationConfiguration)
	{
		await _context.IntegrationConfigurations.AddAsync(integrationConfiguration);
		await _context.SaveChangesAsync();
	}

	public Task Save()
	{
		return _context.SaveChangesAsync();
	}
}
