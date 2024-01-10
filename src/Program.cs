global using Console = Spectre.Console.AnsiConsole;
using Gammer0909.SIGMAMail.Commands;
using Gammer0909.SIGMAMail.Settings;
using Spectre.Console.Cli;
using Spectre.Console;


namespace Gammer0909.SIGMAMail;


public class Program {
    public static int Main(string[] args) {

        var app = new CommandApp();

        // Configuring the application
        app.Configure(config => {

            #if DEBUG
                config.PropagateExceptions();
                config.ValidateExamples();
            #endif

            config.AddCommand<SendCommand>("send")
                .WithDescription("Send an email through the command line.");
            config.AddCommand<ReadCommand>("read")
                .WithDescription("Read emails through the command line.");

            config.SetExceptionHandler(e => {
                if (e is CommandParseException) {
                    Console.MarkupLine("[red]Unknown command[/]");
                } else {
                    Console.WriteException(e, ExceptionFormats.ShortenEverything);
                }
                return 1;
            });

        });

        // Run that mans
        return app.Run(args);

    }

    private static void HandleException(Exception e) {

    }
}