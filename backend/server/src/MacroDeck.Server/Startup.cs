using System.Text.Json.Serialization;
using MacroDeck.Server.Application;
using MacroDeck.Server.Application.Extensions;
using MacroDeck.Server.Application.Plugins.Services;
using MacroDeck.Server.Extensions;
using MacroDeck.Server.Hubs;
using MacroDeck.Server.Infrastructure;
using MacroDeck.Server.Mappings;
using MacroDeck.Server.Middlewares;
using MacroDeck.Server.Services;
using MacroDeck.Server.UI.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi.Models;

namespace MacroDeck.Server;

public class Startup
{
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddHttpClient();
		services.AddApplicationModule()
			.AddInfrastructureModule();
		services.AddNotificationHandlersFromAllAssemblies();

		services.Configure<JsonOptions>(options =>
		{
			options.SerializerOptions.DefaultIgnoreCondition
				= JsonIgnoreCondition.WhenWritingNull;
		});
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "Macro Deck", Version = "v1" });
			c.UseInlineDefinitionsForEnums();
		});
		services.AddCors(options =>
		{
			options.AddPolicy("AllowAny",
				builder =>
				{
					builder.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader();
				});
		});
		services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddCookie(options =>
			{
				options.SlidingExpiration = true;
				options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
				options.Cookie.SameSite = SameSiteMode.Strict;

				options.Events.OnRedirectToAccessDenied = options.Events.OnRedirectToLogin = context =>
				{
					context.Response.StatusCode = StatusCodes.Status401Unauthorized;
					return Task.CompletedTask;
				};
			});
		services.AddAuthorizationBuilder()
			.AddDefaultPolicy("default",
				policy =>
				{
					policy.RequireAuthenticatedUser();
				});
		services.AddControllers();
		services.AddSignalR();

		// Register Mapperly mappers
		services.AddSingleton<FolderMapper>();
		services.AddSingleton<WidgetMapper>();

		// Register plugin services
		services.AddSingleton<IPluginRegistry, PluginRegistry>();
		services.AddSingleton<IPluginActionInvoker, PluginActionInvoker>();
		services.AddSingleton<IPluginCommunicationService, PluginCommunicationService>();

		// Register UI Framework services
		services.AddMacroDeckUi();

		services.AddDataProtection()
			.PersistKeysToFileSystem(new DirectoryInfo("keys"))
			.SetApplicationName("MacroDeck");
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		app.UseCors("AllowAny");
		app.UseDefaultFiles();
		app.UseStaticFiles();
		app.UseRouting();
		app.UseAuthentication();
		app.UseAuthorization();
		app.UseMiddleware<ErrorHandlingMiddleware>();
		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
			endpoints.MapHub<SystemNotificationHub>("/api/hubs/system-notifications");
			endpoints.MapHub<PluginCommunicationHub>("/api/hubs/plugin-communication");
			endpoints.MapHub<MdUiHub>("/api/hubs/ui");
		});
		app.UseSwagger();
		app.UseSwaggerUI();
		app.UseSpa(spa =>
		{
			spa.Options.SourcePath = "wwwroot";
		});
	}
}
