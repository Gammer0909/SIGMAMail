using System;
using Spectre.Console.Cli;
using Gammer0909.SIGMAMail.Settings;

namespace Gammer0909.SIGMAMail.Settings;

public class SendSettings : BaseSettings {

    [CommandOption("-f|--file <FILE>")]
    public string BodyFile { get; set; } = "";

}