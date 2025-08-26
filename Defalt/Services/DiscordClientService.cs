using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DiscordConfig = Defalt.Configuration.DiscordConfig;

namespace Defalt.Services;

public class DiscordClientService
{
    private readonly DiscordSocketClient _client;
    private readonly ILogger<DiscordClientService> _logger;
    private readonly DiscordConfig _config;
    private SlashCommandService? _slashCommandService;
    private ReactionRoleService? _reactionRoleService;

    public DiscordClientService(
        ILogger<DiscordClientService> logger, 
        IOptions<DiscordConfig> config)
    {
        _logger = logger;
        _config = config.Value;
        
        var clientConfig = new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Info,
            MessageCacheSize = 100,
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers
        };
        
        _client = new DiscordSocketClient(clientConfig);
        
        // Subscribe to client events
        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.SlashCommandExecuted += SlashCommandExecutedAsync;
        _client.ReactionAdded += ReactionAddedAsync;
        _client.ReactionRemoved += ReactionRemovedAsync;
    }

    public void SetServices(SlashCommandService slashCommandService, ReactionRoleService reactionRoleService)
    {
        _slashCommandService = slashCommandService;
        _reactionRoleService = reactionRoleService;
    }

    public async Task StartAsync()
    {
        if (string.IsNullOrEmpty(_config.Token))
        {
            _logger.LogError("Discord token is not configured. Please set the token in appsettings.json or environment variables.");
            throw new InvalidOperationException("Discord token is required.");
        }

        await _client.LoginAsync(TokenType.Bot, _config.Token);
        await _client.StartAsync();
        
        _logger.LogInformation("Discord bot is starting...");
    }

    public async Task StopAsync()
    {
        await _client.LogoutAsync();
        await _client.StopAsync();
        _logger.LogInformation("Discord bot has stopped.");
    }

    private Task LogAsync(LogMessage log)
    {
        var logLevel = log.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Trace,
            _ => LogLevel.Information
        };

        _logger.Log(logLevel, log.Exception, "[{Source}] {Message}", log.Source, log.Message);
        return Task.CompletedTask;
    }

    private async Task ReadyAsync()
    {
        _logger.LogInformation("Bot {Username} is connected and ready!", _client.CurrentUser.Username);
        _logger.LogInformation("Bot is active in {ServerCount} servers", _client.Guilds.Count);
        
        // Log configured servers
        foreach (var guild in _client.Guilds)
        {
            var serverConfig = _config.Servers.FirstOrDefault(s => s.ServerId == guild.Id);
            if (serverConfig != null)
            {
                _logger.LogInformation("✅ Configured server: {ServerName} ({ServerId})", guild.Name, guild.Id);
            }
            else
            {
                _logger.LogWarning("⚠️ Unconfigured server: {ServerName} ({ServerId})", guild.Name, guild.Id);
            }
        }
        
        // Register slash commands
        if (_slashCommandService != null)
            await _slashCommandService.RegisterCommandsAsync();
    }

    private async Task SlashCommandExecutedAsync(SocketSlashCommand command)
    {
        _logger.LogInformation("Slash command {CommandName} executed by {Username} in {ServerName}", 
            command.Data.Name, command.User.Username, 
            (command.Channel as SocketGuildChannel)?.Guild?.Name ?? "DM");
            
        if (_slashCommandService != null)
            await _slashCommandService.HandleSlashCommandAsync(command);
    }

    private async Task ReactionAddedAsync(Cacheable<IUserMessage, ulong> cachedMessage, 
        Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
    {
        if (_reactionRoleService != null)
            await _reactionRoleService.HandleReactionAddedAsync(cachedMessage, cachedChannel, reaction);
    }

    private async Task ReactionRemovedAsync(Cacheable<IUserMessage, ulong> cachedMessage, 
        Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
    {
        if (_reactionRoleService != null)
            await _reactionRoleService.HandleReactionRemovedAsync(cachedMessage, cachedChannel, reaction);
    }

    public DiscordSocketClient Client => _client;
}
