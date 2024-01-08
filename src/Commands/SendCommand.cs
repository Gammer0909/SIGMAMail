using Gammer0909.SIGMAMail.Settings;
using Gammer0909.SIGMAMail.Common;
using Spectre.Console.Cli;
using MailKit.Net.Smtp;
using EmailValidation;

namespace Gammer0909.SIGMAMail.Commands;


public class SendCommand : Command<BaseSettings> {

    public override int Execute(CommandContext context, BaseSettings settings) {

        // I know i dont NEED the `SendCommand` there, but I think its more readable that way :)
        var server = SendCommand.GetSmtpClient(settings);

        // Prompt user for the recipient
        var recipient = Console.Ask<string>("Who do you want to send the email to?");
        if (!EmailValidator.Validate(recipient)) {
            Console.MarkupLine("[red]Invalid email!\nPress Any key to try again[/]");
            if (settings.Debug) {
                Console.MarkupLine($"[yellow]Debug: {recipient}[/]");
            }
            System.Console.ReadKey();
            Console.Clear();
            return Execute(context, settings);
        }

        // Prompt user for the subject
        var subject = Console.Ask<string>("What is the subject of the email?");
        if (settings.Debug) {
            Console.MarkupLine($"[yellow]Debug: {subject}[/]");
        }

        // Prompt user for the body
        Console.MarkupLine("Would you like to upload a file? [green]Y[/]/[red]N[/]");
        var upload = Utilities.YesNo();

        string body = "";

        if (upload) {
            // Prompt user for the file
            var file = Console.Ask<string>("What is the path to the file?");
            if (settings.Debug) {
                Console.MarkupLine($"[yellow]Debug: {file}[/]");
            }

            // Read the file
            body = System.IO.File.ReadAllText(file);
        } else {
            // Prompt user for the body
            body = Console.Ask<string>("Enter the body of the Email:\n");
            if (settings.Debug) {
                Console.MarkupLine($"[yellow]Debug: {body}[/]");
            }
        }


        // Prompt user for the confirmation
        Console.MarkupLine($"[bold]Are you sure you want to send this email to {recipient}?[/] [green]Y[/]/[red]N[/]");
        var confirm = Utilities.YesNo();

        


        return 0;
    }

    private static SmtpClient GetSmtpClient(BaseSettings settings) {
        var client = new SmtpClient();

        // Prompt user for the email
        var email = Console.Ask<string>("What is your email?");

        // Validate email
        if (!EmailValidator.Validate(email)) {
            Console.MarkupLine("[red]Invalid email![/]");
            if (settings.Debug) {
                Console.MarkupLine($"[yellow]Debug: {email}[/]");
            }
            System.Console.ReadKey();
            Console.Clear();
            return GetSmtpClient(settings);
        }

        // Prompt user for the password
        var password = Utilities.GetPassword();

        // Auth
        try {
            client.Authenticate(email, password);
        } catch (Exception e) { // Multiple exceptions can be thrown here, so we just catch all of them
            Console.MarkupLine($"[red bold]An error occured trying to authenticate that Email or Password: {e.Message}[/]\n[bold red]Please try again.[/]]");
            if (settings.Debug) {
                Console.MarkupLine($"[yellow]Debug: {e}[/]");
            }
            System.Console.ReadKey();
            Console.Clear();
            return GetSmtpClient(settings);
        }

        if (settings.Verbose) {
            Console.MarkupLine($"[green]Authenticated as {email}[/]");
        }

        if (settings.Debug) {
            Console.MarkupLine($"[yellow]Debug: {client}[/]");
        }

        return client;

    }



}

