using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using DiscordConfig = Defalt.Configuration.DiscordConfig;

namespace Defalt.Services;

public class SlashCommandService
{
    private readonly DiscordSocketClient _client;
    private readonly ILogger<SlashCommandService> _logger;
    private readonly DiscordConfig _config;

    public SlashCommandService(DiscordSocketClient client, ILogger<SlashCommandService> logger, IOptions<DiscordConfig> config)
    {
        _client = client;
        _logger = logger;
        _config = config.Value;
    }

    public async Task RegisterCommandsAsync()
    {
        try
        {
            // Create the /info command
            var infoCommand = new SlashCommandBuilder()
                .WithName("info")
                .WithDescription("Display bot information");

            await _client.CreateGlobalApplicationCommandAsync(infoCommand.Build());
            _logger.LogInformation("Successfully registered slash commands");
        }
        catch (HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
            _logger.LogError("Failed to register slash commands: {Error}", json);
        }
    }

    public async Task HandleSlashCommandAsync(SocketSlashCommand command)
    {
        switch (command.Data.Name)
        {
            case "info":
                await HandleInfoCommandAsync(command);
                break;
            default:
                await command.RespondAsync("Unknown command.");
                break;
        }
    }

    private async Task HandleInfoCommandAsync(SocketSlashCommand command)
    {
        var guild = (command.Channel as SocketGuildChannel)?.Guild;
        var serverConfig = _config.Servers.FirstOrDefault(s => s.ServerId == guild?.Id);
        
        var embed = new EmbedBuilder()
            .WithTitle("whoami")
            .WithDescription("A Discord bot for miscellanous tasks")
            .WithColor(Color.Blue)
            .WithThumbnailUrl(_client.CurrentUser.GetAvatarUrl())
            .AddField("Version", "1.0", true)
            .AddField("Age", GetUptimeString(), true)
            .AddField("Owner", "https://github.com/Donttu", true);

        embed.WithFooter($"Requested by {command.User.Username}", command.User.GetAvatarUrl())
             .WithTimestamp(DateTime.UtcNow);

        await command.RespondAsync(embed: embed.Build());
    }

    private string GetUptimeString()
    {
        var uptime = DateTime.UtcNow - _client.CurrentUser.CreatedAt;
        var years = uptime.Days / 365;
        var remainingDays = uptime.Days % 365;
        return $"{years} years, {remainingDays} days";
    }
}
