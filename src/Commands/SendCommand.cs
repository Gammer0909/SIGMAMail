// -- SYSTEM --
using System;
using System.IO;
using System.Security;

// -- CONSOLE --
using Spectre.Console;
using Spectre.Console.Cli;

// -- PROJECT SPECIFIC --
using Gammer0909.SIGMAMail.Settings;

// -- FILES --
using YamlDotNet.Serialization;

// -- MAIL --
using EmailValidation;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;


namespace Gammer0909.SIGMAMail.Commands;

public class SendCommand : Command<SendSettings> {
    public override int Execute(CommandContext context, SendSettings settings) {
        
        // Check if the user uploaded a file, if not, we'll have to send them to a login page
        if (settings.File == null) {
            // TODO
        }


        return 0;
    }

    private SmtpClient Login() {

        var client = new SmtpClient();

        // Email
        var email = AnsiConsole.Prompt<string>(
            new TextPrompt<string>("Your Email: ")
                .PromptStyle("green")
                .ValidationErrorMessage("Invalid Email")
                .Validate(email => {

                    return EmailValidator.Validate(email);

                })
        );

        // Password
        var password = AnsiConsole.Prompt<SecureString>(
            new TextPrompt<SecureString>("Your Password: ")
                .PromptStyle("green")
                .Secret()
        );

        // Get the server (optional, if they don't know we'll find out)
        var server = AnsiConsole.Prompt<string>(
            new TextPrompt<string>("[[Optional]] Your email's SMTP Server: ")
                .AllowEmpty()
                .PromptStyle("green")
        );
        
        // Get the port (optional, if they don't know we'll find out)
        var port = AnsiConsole.Prompt<int>(
            new TextPrompt<int>("[[Optional]] Your email's SMTP Port: ")
                .AllowEmpty()
                .PromptStyle("green")
        );
        try {
            // Alr, now we need to connect to the server
            client.Connect(server, port, true);

            // Authenticate
            client.Authenticate(email, password.ToString());
        } catch (Exception e) {
            AnsiConsole.MarkupLine($"[red]An error occurred: {e.Message}[/]");
            Environment.Exit(1);
        }


        if (!client.IsConnected || !client.IsAuthenticated) {
            AnsiConsole.MarkupLine("[red]An error occurred, and the client was not connected.[/]");
            Environment.Exit(1);
        }

        // One last thing, ask to see if they want us to generate a login yaml file!
        var shouldGen = AnsiConsole.Confirm("Would you like to generate a login.yaml file, to make logging in easier? \n([red bold]THIS WILL STORE YOUR PASSWORD IN PLAIN TEXT, BE ADVISED[/])[[y/n]]");

        if (shouldGen) {
            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(new {
                Email = email,
                Password = password,
                Server = server,
                Port = port
            });

            File.WriteAllText("login.yaml", yaml);
            AnsiConsole.Write("[green]File successfully generated as login.yaml[/]");
        }

        return client;
    }

    private void SendEmail(string file) {
        // TODO
    }
    
}