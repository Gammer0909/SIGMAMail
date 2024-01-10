using Spectre.Console.Cli;

namespace Gammer0909.SIGMAMail.Settings;

public class ReadSettings : BaseSettings {

    [CommandArgument(0, "<EMAIL>")]
    public string Email { get; set; } = "";

    [CommandOption("-p|--password <PASSWORD>")]
    public string Password { get; set; } = "";

    [CommandOption("-c|--count <COUNT>")]
    public int Count { get; set; } = 5;

    [CommandOption("-P|--use-pop3")]
    public bool UsePop3 { get; set; } = false;


}