# Defalt Discord Bot v2.0

[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/)
[![Discord.Net](https://img.shields.io/badge/Discord.Net-3.18.0-7289da)](https://github.com/discord-net/Discord.Net)
[![Build Status](https://github.com/Donttu/defalt/workflows/Build%20and%20Test/badge.svg)](https://github.com/Donttu/defalt/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![GitHub repo](https://img.shields.io/badge/GitHub-defalt-181717?logo=github)](https://github.com/Donttu/defalt)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)

A modern Discord bot built with Discord.Net and .NET 9.0, designed for reaction-based role assignment across multiple servers.

## Features

- **Reaction Role Assignment**: Assigns specified roles when users react to specific messages
- **Multi-Server Support**: Configure different settings for each server the bot is in
- **Slash Commands**: Modern Discord slash command interface
- **Flexible Rule Acknowledgment**: Users must react to rules messages to get server access
- **Secure Configuration**: Multiple token management options for safe deployment
- **Comprehensive Logging**: Detailed logging for monitoring and debugging
- **Container Ready**: Designed for containerized deployment

## Available Commands

- `/info` - Display bot information and server-specific configuration status

## How It Works

### Reaction Role Flow
1. Create a rules message in your #säännöt (rules) channel
2. Configure the bot with the channel ID, message ID, and reaction emoji
3. Bot automatically adds the configured emoji to the message
4. When users react with the specified emoji, they get the configured role
5. When users remove their reaction, the role is removed (optional)
6. Welcome messages can be sent when users get the role (optional)

## Setup Instructions

### 1. Discord Bot Application Setup

1. Go to the [Discord Developer Portal](https://discord.com/developers/applications)
2. Click "New Application" and give it a name
3. Go to the "Bot" section
4. Click "Add Bot"
5. Copy the bot token (you'll need this later)
6. Under "Privileged Gateway Intents", enable:
   - **Server Members Intent** (required for role management)

### 2. Bot Configuration

#### Environment Variables (Recommended for Production)
```bash
# Windows (PowerShell)
$env:DEFALT_Discord__Token="your_bot_token_here"

# Linux/Docker
export DEFALT_Discord__Token="your_bot_token_here"
```

#### Configuration File (For Development)
Edit `appsettings.json`:

```json
{
  "Discord": {
    "Token": "your_bot_token_here",
    "Servers": [
      {
        "ServerId": 123456789012345678,
        "ServerName": "My Discord Server",
        "AutoRoleId": 987654321098765432,
        "RulesChannelId": 111222333444555666,
        "RulesMessageId": 777888999000111222,
        "ReactionEmoji": "✅",
        "WelcomeChannelId": 555666777888999000,
        "WelcomeMessage": "Welcome to the server, {user}! 🎉",
        "EnableReactionRole": true,
        "EnableWelcomeMessage": true,
        "RemoveReactionAfterRole": false
      }
    ]
  }
}
```

#### Configuration Parameters

- **ServerId**: The Discord server ID
- **ServerName**: Human-readable name for logging
- **AutoRoleId**: The role ID to assign when users react
- **RulesChannelId**: The channel containing the rules message
- **RulesMessageId**: The specific message users must react to
- **ReactionEmoji**: The emoji users must use (e.g., "✅", "👍", "🎉")
- **WelcomeChannelId**: Channel for welcome messages (optional)
- **WelcomeMessage**: Message to send when users get the role
- **EnableReactionRole**: Whether reaction roles are active
- **EnableWelcomeMessage**: Whether to send welcome messages
- **RemoveReactionAfterRole**: Remove user's reaction after giving role

### 3. Setting Up Reaction Roles

#### Step 1: Create Rules Message
1. Go to your #säännöt (rules) channel
2. Create a message explaining your server rules
3. Add instructions like "React with ✅ to accept the rules and gain access"

#### Step 2: Get Message and Channel IDs
1. Enable "Developer Mode" in Discord Settings → Advanced
2. Right-click the rules message → "Copy Message ID"
3. Right-click the #säännöt channel → "Copy Channel ID"
4. Right-click your server name → "Copy Server ID"
5. Go to Server Settings → Roles → Right-click the role → "Copy Role ID"

#### Step 3: Configure the Bot
Update your `appsettings.json`:
```json
{
  "ServerId": 1409547161213079584,
  "RulesChannelId": YOUR_SAANNOT_CHANNEL_ID,
  "RulesMessageId": YOUR_RULES_MESSAGE_ID,
  "AutoRoleId": 1409576738664484864,
  "ReactionEmoji": "✅"
}
```

#### Step 4: Test the Setup
1. Start the bot
2. Check logs for successful configuration
3. The bot will automatically add the reaction emoji to your rules message
4. Test by reacting to the message with an alt account

### 4. Invite Bot to Your Server

1. In Discord Developer Portal, go to OAuth2 > URL Generator
2. Select "bot" and "applications.commands" scopes
3. Select these permissions:
   - **Manage Roles** (to assign roles)
   - **Add Reactions** (to add initial reaction)
   - **Read Message History** (to access reaction events)
   - **Send Messages** (for welcome messages)
   - **Use Slash Commands**
4. **Important**: Make sure the bot's role is higher than the roles it needs to assign

### 5. Running the Bot

```bash
# Navigate to the project directory
cd Defalt

# Run the bot
dotnet run
```

## Project Structure

```
Defalt/
├── Configuration/
│   └── DiscordConfig.cs              # Configuration models
├── Services/
│   ├── DiscordClientService.cs       # Main bot coordination
│   ├── SlashCommandService.cs        # Slash command handling
│   ├── ReactionRoleService.cs        # Reaction role assignment logic
│   └── BotHostedService.cs          # Bot lifecycle management
├── Program.cs                        # Application entry point
├── appsettings.template.json         # Template configuration
└── appsettings.json                 # Your configuration (git-ignored)
```

## Multi-Server Support

The bot can operate in multiple Discord servers simultaneously, each with independent reaction role configuration:

```json
{
  "Discord": {
    "Token": "your_token",
    "Servers": [
      {
        "ServerId": 111111111111111111,
        "ServerName": "Gaming Server",
        "AutoRoleId": 222222222222222222,
        "RulesChannelId": 333333333333333333,
        "RulesMessageId": 444444444444444444,
        "ReactionEmoji": "🎮",
        "EnableReactionRole": true,
        "EnableWelcomeMessage": false
      },
      {
        "ServerId": 555555555555555555,
        "ServerName": "Study Server",
        "AutoRoleId": 666666666666666666,
        "RulesChannelId": 777777777777777777,
        "RulesMessageId": 888888888888888888,
        "ReactionEmoji": "✅",
        "WelcomeChannelId": 999999999999999999,
        "WelcomeMessage": "Welcome to our study group, {user}!",
        "EnableReactionRole": true,
        "EnableWelcomeMessage": true
      }
    ]
  }
}
```

## Development & Extension

### Adding New Slash Commands

1. **Register the command** in `SlashCommandService.RegisterCommandsAsync()`:
```csharp
var setupCommand = new SlashCommandBuilder()
    .WithName("setup")
    .WithDescription("Setup reaction roles for this server");
await _client.CreateGlobalApplicationCommandAsync(setupCommand.Build());
```

2. **Handle the command** in `SlashCommandService.HandleSlashCommandAsync()`:
```csharp
case "setup":
    await HandleSetupCommandAsync(command);
    break;
```

### Adding New Configuration Options

1. Add properties to `ServerConfig` class in `Configuration/DiscordConfig.cs`
2. Update `appsettings.template.json` with new configuration options
3. Use the new configuration in `ReactionRoleService`

### Customizing Reaction Behavior

The `ReactionRoleService` provides several customization options:

- **Multiple Emojis**: Modify `IsCorrectEmoji()` to accept multiple emojis
- **Role Stacking**: Add logic to assign multiple roles based on different reactions
- **Temporary Roles**: Add time-based role removal
- **Role Requirements**: Add checks for existing roles before assignment

## Container Deployment

The bot is designed for containerized deployment:

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:9.0
COPY . /app
WORKDIR /app
ENV DEFALT_Discord__Token=""
ENTRYPOINT ["dotnet", "Defalt.dll"]
```

Set configuration via environment variables:
```bash
docker run \
  -e DEFALT_Discord__Token="your_token" \
  -e DEFALT_Discord__Servers__0__ServerId="123456789" \
  -e DEFALT_Discord__Servers__0__AutoRoleId="987654321" \
  your-bot-image
```

## Security Notes

- Never commit your bot token to version control
- Use environment variables for production deployments
- The `.gitignore` excludes `appsettings.json` and other sensitive files
- Regenerate your bot token if accidentally exposed
- Ensure bot permissions are minimal but sufficient

## Troubleshooting

### Common Issues

1. **"No server configuration found"**
   - Add your server configuration to `appsettings.json`
   - Verify the ServerId matches your Discord server ID
   - Restart the bot after configuration changes

2. **"Role not found" or role assignment fails**
   - Check that the AutoRoleId exists in your server
   - Ensure the bot has "Manage Roles" permission
   - Make sure the bot's role is higher than the role it's trying to assign

3. **Reactions not triggering role assignment**
   - Verify RulesChannelId and RulesMessageId are correct
   - Check that the ReactionEmoji matches exactly
   - Ensure the bot has "Read Message History" permission
   - Enable "Server Members Intent" in Discord Developer Portal

4. **Bot doesn't add initial reaction**
   - Check that the bot has "Add Reactions" permission
   - Verify the message exists and is accessible
   - Check logs for setup errors

5. **Slash commands not appearing**
   - Commands may take up to 1 hour to sync globally
   - Check bot permissions include "Use Slash Commands"
   - Restart Discord client if commands don't appear

### Example Workflow for #säännöt Channel

1. **Create Rules Message**:
   ```
   📋 **SERVER RULES**
   
   1. Be respectful to all members
   2. No spam or excessive caps
   3. Keep discussions in appropriate channels
   4. Follow Discord ToS
   
   React with ✅ below to accept these rules and gain access to the server!
   ```

2. **Configure Bot**:
   - Copy the message ID
   - Add it to your configuration
   - Set the channel ID and emoji

3. **Test**:
   - Bot adds ✅ reaction automatically
   - Users react to get roles
   - Check logs for successful assignments

## Monitoring

The bot provides comprehensive logging:
- Server configuration status on startup
- Reaction events and role assignments
- Command executions
- Errors and warnings

Example log output:
```
[INFO] Bot Defalt is connected and ready!
[INFO] ✅ Configured server: Linkin fuksit S2025 (1409547161213079584)
[INFO] ✅ Assigned role Member to user newuser123 via reaction in server Linkin fuksit S2025
[INFO] Slash command info executed by username in Linkin fuksit S2025
```

## ⚡ Built With

- **[.NET 9.0](https://dotnet.microsoft.com/)** - Runtime and framework
- **[Discord.Net](https://github.com/discord-net/Discord.Net)** - Discord API wrapper
- **[Microsoft.Extensions.Hosting](https://docs.microsoft.com/en-us/dotnet/core/extensions/hosting)** - Background service hosting
- **[Microsoft.Extensions.Configuration](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration)** - Configuration management
- **[Microsoft.Extensions.Logging](https://docs.microsoft.com/en-us/dotnet/core/extensions/logging)** - Structured logging

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

### What this means:
- ✅ **Commercial use** - You can use this commercially
- ✅ **Modification** - You can modify the code
- ✅ **Distribution** - You can distribute the code
- ✅ **Private use** - You can use it privately
- ✅ **No liability** - Authors are not liable for damages
- ⚠️ **License and copyright notice** - Must include the license in copies


