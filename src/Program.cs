global using Console = Spectre.Console.AnsiConsole;
using Gammer0909.SIGMAMail.Commands;
using Gammer0909.SIGMAMail.Settings;
using Spectre.Console.Cli;


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
                .WithDescription("Send an email through the command line.")
                .WithExample(new[] {"send", "-f", "body.md"});

        });

        return app.Run(args);

    }
}