using Spectre.Console.Cli;

namespace Gammer0909.SIGMAMail.Settings;

/// <summary>
/// Base settings for all commands (The root command)
/// </summary>
public class BaseSettings : CommandSettings {
    
    [CommandOption("-d|--debug")]
    public bool Debug { get; set; }

    [CommandOption("-n|--no-color")]
    public bool NoColor { get; set; }

    [CommandOption("-a|--force-ascii")]
    public bool ForceAscii { get; set; }

}