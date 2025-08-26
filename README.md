# Defalt - A mysterious Discord Bot

[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/)
[![Discord.Net](https://img.shields.io/badge/Discord.Net-3.18.0-7289da)](https://github.com/discord-net/Discord.Net)
[![Build Status](https://github.com/Donttu/defalt/workflows/Build%20and%20Test/badge.svg)](https://github.com/Donttu/defalt/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![GitHub repo](https://img.shields.io/badge/GitHub-defalt-181717?logo=github)](https://github.com/Donttu/defalt)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)

A modern Discord bot built with Discord.Net and .NET 9.0, designed mainly for reaction-based role assignment across multiple servers.
All the features developed are based on the needs of me and my peers. The bot is not intended to be deployed by anyone else other than me, 
but for demonstration and practice purposes, I decided to share something here.

## Current Features

- **Reaction Role Assignment**: Assigns specified roles when users react to specific messages
- **Multi-Server Support**: Configure independent settings and features for each server the bot is in
- **Slash Commands**: Modern Discord slash command interface
- **Secure Configuration**: Multiple token management options for safe deployment
- **Comprehensive Logging**: Detailed logging for monitoring and debugging
- **Container Ready**: Designed for containerized deployment

## Available Commands

- `/info` - Display bot information and server-specific configuration status

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
