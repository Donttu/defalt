using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Defalt.Services;

namespace Defalt.Services;

public class BotHostedService : BackgroundService
{
    private readonly DiscordClientService _discordService;
    private readonly ILogger<BotHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public BotHostedService(DiscordClientService discordService, ILogger<BotHostedService> logger, IServiceProvider serviceProvider)
    {
        _discordService = discordService;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // Get the other services and set them in the discord client service
            var slashCommandService = _serviceProvider.GetRequiredService<SlashCommandService>();
            var reactionRoleService = _serviceProvider.GetRequiredService<ReactionRoleService>();
            _discordService.SetServices(slashCommandService, reactionRoleService);
            
            await _discordService.StartAsync();
            
            // Keep the service running until cancellation is requested
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // This is expected when the cancellation token is triggered
            _logger.LogInformation("Bot service is shutting down...");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while running the bot service");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Discord bot...");
        await _discordService.StopAsync();
        await base.StopAsync(cancellationToken);
    }
}
