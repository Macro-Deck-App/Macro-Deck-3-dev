using MacroDeck.Server.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MacroDeck.Server.Application.HostedServices;

public class CreateDefaultFolderHostedService : IHostedService
{
	private readonly IServiceScopeFactory _serviceScopeFactory;

	public CreateDefaultFolderHostedService(IServiceScopeFactory serviceScopeFactory)
	{
		_serviceScopeFactory = serviceScopeFactory;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		using var scope = _serviceScopeFactory.CreateScope();
		var folderService = scope.ServiceProvider.GetRequiredService<IFolderService>();
		await folderService.EnsureDefaultFolderExists();
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}
