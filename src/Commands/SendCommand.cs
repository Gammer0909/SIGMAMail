using Gammer0909.SIGMAMail.Settings;
using Spectre.Console.Cli;
using MailKit.Net.Smtp;
using MimeKit;
using EmailValidation;
using Markdig;
using Spectre.Console;

namespace Gammer0909.SIGMAMail.Commands;


public class SendCommand : Command<SendSettings> {

    public override int Execute(CommandContext context, SendSettings settings) {

        // Get the client
        var client = GetClient(settings);

        // Get the recipient
        string recipient = Console.Prompt(new TextPrompt<string>("Enter the recipient's email address: ")
            .Validate(email => EmailValidator.Validate(email) ? ValidationResult.Success() : ValidationResult.Error("[red bold]Invalid email address.[/]")));

        // Get the subject
        string subject = Console.Prompt(new TextPrompt<string>("Enter the subject: "));

        // Check if they have a text file for the body
        string body = "";        

        if (settings.BodyFile != "") {
            body = System.IO.File.ReadAllText(settings.BodyFile);
        } else {
            body = Console.Prompt(new TextPrompt<string>("Enter the body: "));
        }

        // Create the message   
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(GetName(client.email), client.email));

        message.To.Add(new MailboxAddress(GetName(recipient), recipient));

        message.Subject = subject;
        
        // But wait! They might have uploaded a file!
        // We need to check if it's a .MD, .TXT, or .HTML file
        // If it's a .MD file, we need to convert it to HTML (using Markdig)
        
        if (settings.BodyFile != "") {
            string extension = System.IO.Path.GetExtension(settings.BodyFile);

            if (extension == ".md") {
                body = Markdown.ToHtml(body);
            } else if (extension == ".txt") {
                body = body.Replace("\n", "<br>");
            }

        }

        message.Body = new TextPart("html") {
            Text = body
        };

        // Send the message
        client.Item1.Send(message);

        return 0;
    }


    private (SmtpClient, string email) GetClient(SendSettings settings) {

        var client = new SmtpClient();

        // Get the email
        string email = Console.Prompt(new TextPrompt<string>("Enter your email address: ")
            .Validate(email => EmailValidator.Validate(email) ? ValidationResult.Success() : ValidationResult.Error("[red bold]Invalid email address.[/]")));

        // Get the password
        string password = Console.Prompt(new TextPrompt<string>("Enter your password: ")
            .Secret('#'));

        // Get the host
        string host = GetHost(email);

        // Connect to the host
        client.Connect(host, 587, false);

        // auth client
        client.Authenticate(email, password);

        
        // Ship that bad boy back to the Execute method
        return (client, email);


    }

    private string GetName(string email) {
            
        string[] split = email.Split('@');
        string name = split[0];

        return name;

    }

    private string GetHost(string email) {

        string[] split = email.Split('@');
        string domain = split[1];

        switch (domain) {
            case "gmail.com":
                return "smtp.gmail.com";
            case "outlook.com":
                return "smtp-mail.outlook.com";
            case "yahoo.com":
                return "smtp.mail.yahoo.com";
            case "aol.com":
                return "smtp.aol.com"; // ????? Is that even possible?!?!
            case "icloud.com":
                return "smtp.mail.me.com";
            default:
                return "smtp." + domain;
        }

    }

}

