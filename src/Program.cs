using System;
using Spectre.Console.Cli;
using Spectre.Console;


namespace Gammer0909.SIGMAMail;


public class Program {
    public static void Main(string[] args) {

        var app = new CommandApp();
        app.Configure(config => {
            config.AddCommand<Commands.SendCommand>("send")
                .WithDescription("Send an email")
                .WithExample(new[] { "send --file login.yaml" });
        });

        var result = app.Run(args);
        if (result != 0) {
            AnsiConsole.MarkupLine("[red]An error occurred[/]");
        }


    }
}