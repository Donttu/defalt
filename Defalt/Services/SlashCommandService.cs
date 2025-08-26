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
                .WithDescription("Display bot information and server statistics");

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
            .WithTitle("Defalt Bot Information")
            .WithDescription("A Discord bot for reaction-based role assignment and server management")
            .WithColor(Color.Blue)
            .WithThumbnailUrl(_client.CurrentUser.GetAvatarUrl())
            .AddField("Version", "2.0", true)
            .AddField("Uptime", GetUptimeString(), true)
            .AddField("Total Servers", _client.Guilds.Count, true);

        if (guild != null)
        {
            embed.AddField("Current Server", guild.Name, true)
                 .AddField("Server Members", guild.MemberCount, true);

            if (serverConfig != null)
            {
                var role = guild.GetRole(serverConfig.AutoRoleId);
                var rulesChannel = serverConfig.RulesChannelId.HasValue ? guild.GetTextChannel(serverConfig.RulesChannelId.Value) : null;
                
                embed.AddField("Reaction Role", 
                    serverConfig.EnableReactionRole 
                        ? (role?.Name ?? "Role not found") 
                        : "Disabled", true);
                
                if (serverConfig.EnableReactionRole)
                {
                    embed.AddField("Rules Channel", rulesChannel?.Name ?? "Not configured", true);
                    embed.AddField("Reaction Emoji", serverConfig.ReactionEmoji, true);
                }
            }
            else
            {
                embed.AddField("Server Configuration", "Not configured", true);
            }
        }

        embed.WithFooter($"Requested by {command.User.Username}", command.User.GetAvatarUrl())
             .WithTimestamp(DateTime.UtcNow);

        await command.RespondAsync(embed: embed.Build());
    }

    private string GetUptimeString()
    {
        var uptime = DateTime.UtcNow - _client.CurrentUser.CreatedAt;
        return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m";
    }
}
