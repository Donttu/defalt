namespace Defalt.Configuration;

public class DiscordConfig
{
    public string Token { get; set; } = string.Empty;
    public List<ServerConfig> Servers { get; set; } = new();
}

public class ServerConfig
{
    public ulong ServerId { get; set; }
    public string ServerName { get; set; } = string.Empty;
    public ulong AutoRoleId { get; set; }
    public ulong? RulesChannelId { get; set; }
    public ulong? RulesMessageId { get; set; }
    public string ReactionEmoji { get; set; } = "✅";
    public ulong? WelcomeChannelId { get; set; }
    public string WelcomeMessage { get; set; } = "Welcome to the server, {user}!";
    public bool EnableReactionRole { get; set; } = true;
    public bool EnableWelcomeMessage { get; set; } = false;
    public bool RemoveReactionAfterRole { get; set; } = false;
}
