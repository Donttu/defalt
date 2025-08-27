# Defalt - A mysterious Discord Bot

[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/)
[![Discord.Net](https://img.shields.io/badge/Discord.Net-3.18.0-7289da)](https://github.com/discord-net/Discord.Net)
[![Build Status](https://github.com/Donttu/defalt/workflows/Build%20and%20Test/badge.svg)](https://github.com/Donttu/defalt/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![GitHub repo](https://img.shields.io/badge/GitHub-defalt-181717?logo=github)](https://github.com/Donttu/defalt)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)

A modern Discord bot built with Discord.Net and .NET 9.0, designed mainly for reaction-based role assignment across multiple servers to begin with.
All the features developed are based on the needs of me and my peers. The bot is not intended to be deployed by anyone else other than me, 
but for demonstration and practice purposes, I decided to share something here.

## Current Features

- **Reaction Role Assignment**: Assigns specified roles when users react to specific messages
- **Multi-Server Support**: Configure independent settings and features for each server the bot is in
- **Slash Commands**: Modern Discord slash command interface
- **Minecraft Whitelist Integration**: Add users to Minecraft server whitelists via web API
- **Secure Configuration**: Multiple token management options for safe deployment
- **Comprehensive Logging**: Detailed logging for monitoring and debugging

## Available Commands

- `/info` - Display bot information and server-specific configuration status (available globally)
- `/whitelist <username>` - Add a Minecraft username to the server whitelist (only available on servers with whitelist configured)

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
        "EnableWelcomeMessage": true,
        "WhitelistUrl": "https://your-minecraft-server.com/api/whitelist"
      }
    ]
  }
}
```

## Minecraft Whitelist Integration

The bot supports adding users to Minecraft server whitelists through a web API integration. Users can use the `/whitelist <username>` command to add their Minecraft username to the server's whitelist.

### Configuration

To enable whitelist functionality for a server, add the `WhitelistUrl` field to your server configuration:

```json
{
  "ServerId": 123456789012345678,
  "WhitelistUrl": "https://your-minecraft-server.com/api/whitelist"
}
```

The bot will append `/` and the username to this base URL. For example, if a user runs `/whitelist Steve`, the bot will make a GET request to:
```
https://your-minecraft-server.com/api/whitelist/Steve
```

**Important**: The `/whitelist` command will only be available on Discord servers that have the `WhitelistUrl` configured. Servers without this configuration will not see the command.

### Features

- **Username Validation**: Validates Minecraft username format (3-16 characters, alphanumeric and underscores only)
- **Error Handling**: Graceful handling of API failures and network issues
- **Server-Specific**: Each Discord server can have its own whitelist URL
- **Silent Operation**: All responses are ephemeral (only visible to the user who ran the command)
- **Logging**: Comprehensive logging of whitelist operations for monitoring

### Usage

Users can whitelist themselves using the slash command:
```
/whitelist Steve
```

The bot will:
1. Validate the username format
2. Make a request to the configured whitelist URL
3. Provide feedback on success or failure
```

## Troubleshooting

The developer does not provide any official support for troubleshooting. And there is no point in asking for one.

## Monitoring

The bot provides comprehensive logging:
- Server configuration status on startup
- Reaction events and role assignments
- Command executions
- Errors and warnings

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
