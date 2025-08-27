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
            // Create the /info command (global)
            var infoCommand = new SlashCommandBuilder()
                .WithName("info")
                .WithDescription("Display bot information");

            await _client.CreateGlobalApplicationCommandAsync(infoCommand.Build());

            // Create the /whitelist command (guild-specific for servers with WhitelistUrl configured)
            var whitelistCommand = new SlashCommandBuilder()
                .WithName("whitelist")
                .WithDescription("Add a Minecraft username to the server whitelist")
                .AddOption("username", ApplicationCommandOptionType.String, "The Minecraft username to whitelist", isRequired: true);

            // Register whitelist command only for servers that have WhitelistUrl configured
            foreach (var serverConfig in _config.Servers.Where(s => !string.IsNullOrEmpty(s.WhitelistUrl)))
            {
                var guild = _client.GetGuild(serverConfig.ServerId);
                if (guild != null)
                {
                    await guild.CreateApplicationCommandAsync(whitelistCommand.Build());
                    _logger.LogInformation("Registered whitelist command for server: {ServerName} ({ServerId})", 
                        serverConfig.ServerName, serverConfig.ServerId);
                }
                else
                {
                    _logger.LogWarning("Could not find guild with ID {ServerId} ({ServerName}) to register whitelist command", 
                        serverConfig.ServerId, serverConfig.ServerName);
                }
            }

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
            case "whitelist":
                await HandleWhitelistCommandAsync(command);
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

    private async Task HandleWhitelistCommandAsync(SocketSlashCommand command)
    {
        var guild = (command.Channel as SocketGuildChannel)?.Guild;
        var serverConfig = _config.Servers.FirstOrDefault(s => s.ServerId == guild?.Id);
        
        if (serverConfig?.WhitelistUrl == null)
        {
            await command.RespondAsync("❌ Whitelist functionality is not configured for this server.", ephemeral: true);
            return;
        }

        var username = command.Data.Options.FirstOrDefault(x => x.Name == "username")?.Value?.ToString();
        
        if (string.IsNullOrWhiteSpace(username))
        {
            await command.RespondAsync("❌ Please provide a valid Minecraft username.", ephemeral: true);
            return;
        }

        // Validate username format (Minecraft usernames are 3-16 characters, alphanumeric and underscores)
        if (!IsValidMinecraftUsername(username))
        {
            await command.RespondAsync("❌ Invalid Minecraft username format. Usernames must be 3-16 characters long and contain only letters, numbers, and underscores.", ephemeral: true);
            return;
        }

        await command.DeferAsync(ephemeral: true);

        try
        {
            using var httpClient = new HttpClient();
            var whitelistUrl = $"{serverConfig.WhitelistUrl.TrimEnd('/')}/{username}";
            
            _logger.LogInformation("Attempting to whitelist user {Username} via URL: {Url}", username, whitelistUrl);
            
            var response = await httpClient.GetAsync(whitelistUrl);
            
            if (response.IsSuccessStatusCode)
            {
                var embed = new EmbedBuilder()
                    .WithTitle("✅ Whitelist Success")
                    .WithDescription($"Successfully added **{username}** to the Minecraft server whitelist!")
                    .WithColor(Color.Green)
                    .WithFooter($"Requested by {command.User.Username}", command.User.GetAvatarUrl())
                    .WithTimestamp(DateTime.UtcNow);

                await command.FollowupAsync(embed: embed.Build(), ephemeral: true);
                _logger.LogInformation("Successfully whitelisted user {Username}", username);
            }
            else
            {
                _logger.LogWarning("Failed to whitelist user {Username}. HTTP Status: {StatusCode}", username, response.StatusCode);
                await command.FollowupAsync("❌ Failed to add user to whitelist. Please try again later or contact an administrator.", ephemeral: true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while attempting to whitelist user {Username}", username);
            await command.FollowupAsync("❌ An error occurred while processing the whitelist request. Please try again later.", ephemeral: true);
        }
    }

    private static bool IsValidMinecraftUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;
            
        if (username.Length < 3 || username.Length > 16)
            return false;
            
        return username.All(c => char.IsLetterOrDigit(c) || c == '_');
    }

    private string GetUptimeString()
    {
        var uptime = DateTime.UtcNow - _client.CurrentUser.CreatedAt;
        var years = uptime.Days / 365;
        var remainingDays = uptime.Days % 365;
        return $"{years} years, {remainingDays} days";
    }
}
