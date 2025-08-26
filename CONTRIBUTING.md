# Contributing to Defalt Discord Bot

First off, thank you for considering contributing to Defalt! 🎉

## Code of Conduct

This project and everyone participating in it is governed by our commitment to providing a welcoming and inclusive experience for everyone.

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check the existing issues to avoid duplicates. When you create a bug report, please include as many details as possible:

- **Use a clear and descriptive title**
- **Describe the exact steps to reproduce the problem**
- **Provide specific examples to demonstrate the steps**
- **Describe the behavior you observed and what behavior you expected**
- **Include logs if applicable**

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion, please include:

- **Use a clear and descriptive title**
- **Provide a step-by-step description of the suggested enhancement**
- **Provide specific examples to demonstrate the feature**
- **Explain why this enhancement would be useful**

### Pull Requests

1. Fork the repo and create your branch from `main`
2. If you've added code that should be tested, add tests
3. Ensure the build passes
4. Make sure your code follows the existing style
5. Issue that pull request!

## Development Process

### Prerequisites

- .NET 9.0 SDK
- A Discord bot token for testing
- A test Discord server

### Setting Up Development Environment

1. **Clone your fork**
   ```bash
   git clone https://github.com/YOUR_USERNAME/defalt.git
   cd defalt
   ```

2. **Set up configuration**
   ```bash
   cp Defalt/appsettings.template.json Defalt/appsettings.json
   # Edit appsettings.json with your test bot token and server IDs
   ```

3. **Build and run**
   ```bash
   cd Defalt
   dotnet build
   dotnet run
   ```

### Code Style

- Follow standard C# conventions
- Use meaningful variable and method names
- Add XML documentation for public methods
- Keep methods focused and single-purpose
- Use async/await properly for asynchronous operations

### Commit Messages

- Use the present tense ("Add feature" not "Added feature")
- Use the imperative mood ("Move cursor to..." not "Moves cursor to...")
- Limit the first line to 72 characters or less
- Reference issues and pull requests liberally after the first line

## Project Structure

```
Defalt/
├── Configuration/     # Configuration models
├── Services/         # Core bot services
│   ├── DiscordClientService.cs      # Main coordinator
│   ├── ReactionRoleService.cs       # Reaction role logic
│   ├── SlashCommandService.cs       # Command handling
│   └── BotHostedService.cs         # Lifecycle management
└── Program.cs        # Entry point
```

## Adding New Features

### Adding New Slash Commands

1. **Register the command** in `SlashCommandService.RegisterCommandsAsync()`
2. **Handle the command** in `SlashCommandService.HandleSlashCommandAsync()`
3. **Implement the handler method**
4. **Update documentation**

### Adding New Configuration Options

1. **Add properties to `ServerConfig`** in `Configuration/DiscordConfig.cs`
2. **Update `appsettings.template.json`**
3. **Use the configuration in your service**
4. **Update README with new configuration options**

## Testing

- Test your changes with a real Discord bot and server
- Verify that existing functionality still works
- Test edge cases and error conditions
- Check logs for any errors or warnings

## Questions?

Feel free to open an issue with the `question` label if you need help or clarification about anything!

Thank you for contributing! 🚀
