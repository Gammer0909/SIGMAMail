using Gammer0909.SIGMAMail.Settings;
using Spectre.Console.Cli;
using MailKit.Net.Smtp;
using MimeKit;
using Markdig;
using Spectre.Console;

// Using "Console" as "AnsiConsole"

namespace Gammer0909.SIGMAMail.Commands;


public class SendCommand : Command<SendSettings> {

    public override int Execute(CommandContext context, SendSettings settings) {

        
        if (settings.Debug) {
            Console.MarkupLine($"[yellow bold]Debug mode is enabled![/]");
            Console.MarkupLine($"[yellow bold]Sender: {settings.Sender}[/]");
            Console.MarkupLine($"[yellow bold]Recipient: {settings.Recipient}[/]");
            Console.MarkupLine($"[yellow bold]Subject: \"{settings.Subject}\"[/]");
            Console.MarkupLine($"[yellow bold]Body file path: {settings.BodyFilePath}[/]");
            Console.MarkupLine($"[yellow bold]Attachment file path: {settings.AttachmentFilePath}[/]");
            Console.MarkupLine($"[yellow bold]Cc recipients: {string.Join(", ", settings.CcRecipient)}[/]");
            Console.MarkupLine($"[yellow bold]Bcc recipients: {string.Join(", ", settings.BccRecipient)}[/]\n\n\n");

        }

        Console.Status()
            .Spinner(Spinner.Known.Line)
            .Start("Sending Email...", a => {


                a.Status("[green]Connecting to SMTP server...[/]");
                // Thread.Sleep(1000);

                var client = GetClient(settings);

                a.Status("[green]Connected![/]");

                a.Status("[green]Sending email...[/]");
                // Thread.Sleep(1000);

                var msg = GetMsg(settings);
                try {
                    client.Send(msg);
                    client.Disconnect(true);
                    client.Dispose();
                } catch (Exception e) {
                    if (settings.Debug)
                        Console.WriteException(e, ExceptionFormats.ShortenEverything);
                    else
                        Console.MarkupLine("[red bold]An error occured while sending the email.[/]");
                }

            });        


        Console.MarkupLine($"[green bold]Email successfully sent to {settings.Recipient}![/]");


        return 0;
    }


    public static SmtpClient GetClient(SendSettings settings) {

        var client = new SmtpClient();

        string host = ParseHost(settings);

        client.Connect(host, 587, false);

        if (settings.Password != "") {
            client.Authenticate(settings.Sender, settings.Password);
        } else {
            // Ask the password
            Console.MarkupLine("Enter the password for [bold]{0}[/]", settings.Sender);
            string password = Console.Prompt(
                new TextPrompt<string>("")
                    .Secret()
            );
            client.Authenticate(settings.Sender, password);
        }

        return client;
    }

    public static MimeMessage GetMsg(SendSettings settings) {

        var msg = new MimeMessage();

        msg.From.Add(MailboxAddress.Parse(settings.Sender));

        msg.To.Add(MailboxAddress.Parse(settings.Recipient));

        if (settings.CcRecipient.Length > 0) {
            foreach (string cc in settings.CcRecipient) {
                msg.Cc.Add(MailboxAddress.Parse(cc));
            }
        }

        if (settings.BccRecipient.Length > 0) {
            foreach (string bcc in settings.BccRecipient) {
                msg.Bcc.Add(MailboxAddress.Parse(bcc));
            }
        }

        msg.Subject = settings.Subject;

        if (!File.Exists(settings.BodyFilePath)) {
            Console.WriteException(new FileNotFoundException($"The file {settings.BodyFilePath} does not exist."));
            Environment.Exit(1);
        }

        string body = File.ReadAllText(settings.BodyFilePath);

        string htmlBody = ParseBody(settings.BodyFilePath);

        var builder = new BodyBuilder();

        builder.HtmlBody = htmlBody;

        if (settings.AttachmentFilePath != "") {
            if (!File.Exists(settings.AttachmentFilePath)) {
                Console.WriteException(new FileNotFoundException($"The file {settings.AttachmentFilePath} does not exist."));
                Environment.Exit(1);
            }
            builder.Attachments.Add(settings.AttachmentFilePath);
        }

        msg.Body = builder.ToMessageBody();

        return msg;
    } 

    private static string ParseBody(string bodyFilePath) {
        
        // Check file ending
        string fileEnding = Path.GetExtension(bodyFilePath);

        switch (fileEnding) {
            case ".md":
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                return Markdown.ToHtml(File.ReadAllText(bodyFilePath), pipeline);
            case ".html":
                return File.ReadAllText(bodyFilePath);
            default:
                return File.ReadAllText(bodyFilePath);
        }

    }

    private static string ParseHost(SendSettings settings) {

        // From the domain of the sender
        string domain = settings.Sender.Split('@')[1];

        switch (domain) {
            case "gmail.com":
                return "smtp.gmail.com";
            case "outlook.com":
                return "smtp.outlook.com";
            case "hotmail.com":
                return "smtp.live.com";
            case "yahoo.com":
                return "smtp.mail.yahoo.com";
            case "aol.com":
                return "smtp.aol.com";
            default:
                return $"smtp.{domain}";
        }


    }
}

