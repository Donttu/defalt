using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Defalt.Configuration;
using DiscordConfig = Defalt.Configuration.DiscordConfig;

namespace Defalt.Services;

public class ReactionRoleService
{
    private readonly DiscordSocketClient _client;
    private readonly ILogger<ReactionRoleService> _logger;
    private readonly DiscordConfig _config;

    public ReactionRoleService(DiscordSocketClient client, ILogger<ReactionRoleService> logger, IOptions<DiscordConfig> config)
    {
        _client = client;
        _logger = logger;
        _config = config.Value;
    }

    public async Task HandleReactionAddedAsync(Cacheable<IUserMessage, ulong> cachedMessage, 
        Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
    {
        try
        {
            // Don't process bot reactions
            if (reaction.User.Value?.IsBot == true)
                return;

            var channel = await cachedChannel.GetOrDownloadAsync();
            if (channel is not SocketGuildChannel guildChannel)
                return;

            var serverConfig = GetServerConfig(guildChannel.Guild.Id);
            if (serverConfig == null || !serverConfig.EnableReactionRole)
                return;

            // Check if this is the configured rules channel and message
            if (serverConfig.RulesChannelId != channel.Id || 
                serverConfig.RulesMessageId != cachedMessage.Id)
                return;

            // Check if this is the correct emoji
            if (!IsCorrectEmoji(reaction.Emote, serverConfig.ReactionEmoji))
                return;

            var user = reaction.User.Value as SocketGuildUser;
            if (user == null)
                return;

            await AssignRoleAsync(user, serverConfig);

            if (serverConfig.RemoveReactionAfterRole)
            {
                var message = await cachedMessage.GetOrDownloadAsync();
                await message.RemoveReactionAsync(reaction.Emote, user);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling reaction added event");
        }
    }

    public async Task HandleReactionRemovedAsync(Cacheable<IUserMessage, ulong> cachedMessage, 
        Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
    {
        try
        {
            // Don't process bot reactions
            if (reaction.User.Value?.IsBot == true)
                return;

            var channel = await cachedChannel.GetOrDownloadAsync();
            if (channel is not SocketGuildChannel guildChannel)
                return;

            var serverConfig = GetServerConfig(guildChannel.Guild.Id);
            if (serverConfig == null || !serverConfig.EnableReactionRole)
                return;

            // Check if this is the configured rules channel and message
            if (serverConfig.RulesChannelId != channel.Id || 
                serverConfig.RulesMessageId != cachedMessage.Id)
                return;

            // Check if this is the correct emoji
            if (!IsCorrectEmoji(reaction.Emote, serverConfig.ReactionEmoji))
                return;

            var user = reaction.User.Value as SocketGuildUser;
            if (user == null)
                return;

            await RemoveRoleAsync(user, serverConfig);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling reaction removed event");
        }
    }

    private async Task AssignRoleAsync(SocketGuildUser user, ServerConfig serverConfig)
    {
        try
        {
            var role = user.Guild.GetRole(serverConfig.AutoRoleId);
            if (role == null)
            {
                _logger.LogError("Role with ID {RoleId} not found in server {ServerName}", 
                    serverConfig.AutoRoleId, user.Guild.Name);
                return;
            }

            // Check if user already has the role
            if (user.Roles.Contains(role))
            {
                _logger.LogDebug("User {Username} already has role {RoleName} in server {ServerName}", 
                    user.Username, role.Name, user.Guild.Name);
                return;
            }

            await user.AddRoleAsync(role);
            _logger.LogInformation("✅ Assigned role {RoleName} to user {Username} via reaction in server {ServerName}", 
                role.Name, user.Username, user.Guild.Name);

            // Send welcome message if enabled
            if (serverConfig.EnableWelcomeMessage && serverConfig.WelcomeChannelId.HasValue)
            {
                await SendWelcomeMessageAsync(user, serverConfig);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign role to user {Username} in server {ServerName}", 
                user.Username, user.Guild.Name);
        }
    }

    private async Task RemoveRoleAsync(SocketGuildUser user, ServerConfig serverConfig)
    {
        try
        {
            var role = user.Guild.GetRole(serverConfig.AutoRoleId);
            if (role == null)
            {
                _logger.LogError("Role with ID {RoleId} not found in server {ServerName}", 
                    serverConfig.AutoRoleId, user.Guild.Name);
                return;
            }

            // Check if user has the role
            if (!user.Roles.Contains(role))
            {
                _logger.LogDebug("User {Username} doesn't have role {RoleName} in server {ServerName}", 
                    user.Username, role.Name, user.Guild.Name);
                return;
            }

            await user.RemoveRoleAsync(role);
            _logger.LogInformation("❌ Removed role {RoleName} from user {Username} via reaction removal in server {ServerName}", 
                role.Name, user.Username, user.Guild.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove role from user {Username} in server {ServerName}", 
                user.Username, user.Guild.Name);
        }
    }

    private async Task SendWelcomeMessageAsync(SocketGuildUser user, ServerConfig serverConfig)
    {
        try
        {
            if (!serverConfig.WelcomeChannelId.HasValue)
                return;

            var channel = user.Guild.GetTextChannel(serverConfig.WelcomeChannelId.Value);
            if (channel == null)
            {
                _logger.LogError("Welcome channel with ID {ChannelId} not found in server {ServerName}", 
                    serverConfig.WelcomeChannelId.Value, user.Guild.Name);
                return;
            }

            var welcomeMessage = serverConfig.WelcomeMessage.Replace("{user}", user.Mention);
            await channel.SendMessageAsync(welcomeMessage);
            
            _logger.LogInformation("📨 Sent welcome message to {Username} in server {ServerName}", 
                user.Username, user.Guild.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome message for user {Username} in server {ServerName}", 
                user.Username, user.Guild.Name);
        }
    }

    private bool IsCorrectEmoji(IEmote emote, string configuredEmoji)
    {
        return emote.Name == configuredEmoji || emote.ToString() == configuredEmoji;
    }

    private ServerConfig? GetServerConfig(ulong guildId)
    {
        return _config.Servers.FirstOrDefault(s => s.ServerId == guildId);
    }

    public async Task SetupReactionRoleAsync(ulong guildId, ulong channelId, ulong messageId)
    {
        try
        {
            var guild = _client.GetGuild(guildId);
            if (guild == null)
            {
                _logger.LogError("Guild with ID {GuildId} not found", guildId);
                return;
            }

            var channel = guild.GetTextChannel(channelId);
            if (channel == null)
            {
                _logger.LogError("Channel with ID {ChannelId} not found in guild {GuildName}", channelId, guild.Name);
                return;
            }

            var message = await channel.GetMessageAsync(messageId);
            if (message == null)
            {
                _logger.LogError("Message with ID {MessageId} not found in channel {ChannelName}", messageId, channel.Name);
                return;
            }

            var serverConfig = GetServerConfig(guildId);
            if (serverConfig == null)
            {
                _logger.LogError("No server configuration found for guild {GuildName}", guild.Name);
                return;
            }

            // Add the reaction emoji to the message if it doesn't already exist
            var emoji = new Emoji(serverConfig.ReactionEmoji);
            await message.AddReactionAsync(emoji);

            _logger.LogInformation("🎭 Set up reaction role on message {MessageId} in channel {ChannelName} for guild {GuildName}", 
                messageId, channel.Name, guild.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to setup reaction role for guild {GuildId}", guildId);
        }
    }

    public string GetServerStatus(ulong guildId)
    {
        var serverConfig = GetServerConfig(guildId);
        if (serverConfig == null)
            return "❌ Server not configured";

        var guild = _client.GetGuild(guildId);
        if (guild == null)
            return "❌ Server not found";

        var status = $"✅ **{serverConfig.ServerName}**\n";
        
        if (serverConfig.EnableReactionRole)
        {
            var role = guild.GetRole(serverConfig.AutoRoleId);
            var channel = serverConfig.RulesChannelId.HasValue ? guild.GetTextChannel(serverConfig.RulesChannelId.Value) : null;
            
            status += $"🎭 Reaction Role: {(role?.Name ?? "Role not found")}\n";
            status += $"📋 Rules Channel: {(channel?.Name ?? "Channel not configured")}\n";
            status += $"📝 Message ID: {(serverConfig.RulesMessageId?.ToString() ?? "Not configured")}\n";
            status += $"😀 Emoji: {serverConfig.ReactionEmoji}\n";
        }
        else
        {
            status += "🎭 Reaction Role: Disabled\n";
        }

        if (serverConfig.EnableWelcomeMessage)
        {
            var channel = serverConfig.WelcomeChannelId.HasValue 
                ? guild.GetTextChannel(serverConfig.WelcomeChannelId.Value)
                : null;
            status += $"💬 Welcome Message: {(channel?.Name ?? "Channel not found")}\n";
        }
        else
        {
            status += "💬 Welcome Message: Disabled\n";
        }

        return status;
    }
}
