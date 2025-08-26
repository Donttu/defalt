using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Defalt.Configuration;
using Defalt.Services;

// Create the host builder
var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        // Add configuration sources
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables(prefix: "DEFALT_");
    })
    .ConfigureServices((context, services) =>
    {
        // Configure Discord settings
        services.Configure<DiscordConfig>(context.Configuration.GetSection("Discord"));
        
        // Register services
        services.AddSingleton<DiscordClientService>();
        services.AddSingleton<SlashCommandService>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<SlashCommandService>>();
            var config = provider.GetRequiredService<IOptions<DiscordConfig>>();
            var clientService = provider.GetRequiredService<DiscordClientService>();
            return new SlashCommandService(clientService.Client, logger, config);
        });
        services.AddSingleton<ReactionRoleService>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<ReactionRoleService>>();
            var config = provider.GetRequiredService<IOptions<DiscordConfig>>();
            var clientService = provider.GetRequiredService<DiscordClientService>();
            return new ReactionRoleService(clientService.Client, logger, config);
        });
        services.AddHostedService<BotHostedService>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Information);
    })
    .Build();

Console.WriteLine("Starting Defalt Discord Bot...");

try
{
    await host.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}
