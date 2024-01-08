using Spectre.Console.Cli;

namespace Gammer0909.SIGMAMail.Settings;

/// <summary>
/// The base settings for all commands.
/// </summary>
public class BaseSettings : CommandSettings {

    [CommandOption("-v|--verbose")]
    public bool Verbose { get; set; } = false;

    [CommandOption("-d|--debug")]
    public bool Debug { get; set; } = false;

    [CommandOption("-n|--nocolor")]
    public bool NoColor { get; set; } = false;

    [CommandOption("-a|--ascii")]
    public bool ForceAscii { get; set; } = false;

}