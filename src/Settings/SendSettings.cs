using System;
using Spectre.Console.Cli;
using Gammer0909.SIGMAMail.Settings;

namespace Gammer0909.SIGMAMail.Settings;

public class SendSettings : BaseSettings {

    /// <summary>
    /// The Sign-In File for the user (So they don't need to sign in every time)
    /// </summary>
    [CommandOption("-f|--file")]
    public string? File { get; set; }

}