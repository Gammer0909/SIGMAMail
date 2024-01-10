using Spectre.Console.Cli;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Search;
using MailKit;
using Spectre.Console;
using MimeKit;
using EmailValidation;
using Gammer0909.SIGMAMail.Settings;

namespace Gammer0909.SIGMAMail.Commands;

public class ReadCommand : Command<ReadSettings> {

    public override int Execute(CommandContext context, ReadSettings settings) {

        if (EmailValidator.Validate(settings.Email) == false) {
            Console.MarkupLine("[red]Invalid email address![/]");
            return 1;
        }

        var messages = GetMessages(settings);

        

        return 0;
    }

    private int RenderMessages(List<MimeMessage> messages, ReadSettings settings) {

        // Construct a le SelectionPrompt
        var selectionPrompt = new SelectionPrompt<string>();
        selectionPrompt.Title("Read Email");
        selectionPrompt.PageSize(messages.Count);

        var stringsToRender = new List<string>();

        // We want to format the strings like this
        // Subject (If more than 10 chars cut it off at 6 and add a ...) | Sender FName, LName

        // I would want to bold them if they're not read but idk how to get flags
        foreach (var msg in messages) {

            string subject = msg.Subject;

            if (subject.Length > 10) {
                subject = subject.Substring(0, 6) + "...";
            }

            string sender = msg.From.ToString();

            stringsToRender.Add($"{subject} | {sender}");

            // here's where I'd implement the bold logic but again, idk how to get flags

        }

        selectionPrompt.AddChoices(stringsToRender);

        // Render the prompt
        var result = Console.Prompt(selectionPrompt);

        // Get the index of the result
        int index = stringsToRender.IndexOf(result);

        // Get the message
        var message = messages[index];

        // Render the message
        Console.Clear();

        // Build the table
        var table = new Table();

        // I want it like this (minified, it'll be bigger)
        /*
        +-----------------+-----------------+
        | From - <SenderName (SenderEmail)> |
        +-----------------+-----------------+
        | Subject - <Subject>               |
        +-----------------+-----------------+
        | Body                              |
        | <Body>                            |
        | <Body>                            |
        | <Body>                            |
        +-----------------+-----------------+   
        */

        if (settings.ForceAscii) {
            table.Border = TableBorder.Ascii;
        } else {
            table.Border = TableBorder.Rounded;
        }

        table.AddRow($"From - {message.From}");
        table.AddRow($"Subject - {message.Subject}");
        table.AddRow(
            new Panel(message.TextBody)
                .Expand()
                .RoundedBorder()
        );

        Console.Write(table);

        // Ask if they want to reply
        var reply = Console.Confirm("Would you like to reply to this email?");
        if (reply) {
            // Reply to the email
            ReplyToEmail(message, settings);
            return 0;
        } else {
            // Ask if they want to delete the email
            var delete = Console.Confirm("Would you like to delete this email?");
            if (delete) {
                // Delete the email
                DeleteEmail(message, settings);
                return 0;
            }
            return 0;
        
        }
        return 0;

    }

    private void DeleteEmail(MimeMessage message, ReadSettings settings) {
        // Delete the message
        if (settings.UsePop3) {
            // Use pop3
            var popClient = GetPop3Client(settings);

            var messages = popClient.GetMessageUids();

            for (int i = 0; i < settings.Count; i++) {
                var msg = popClient.GetMessage(i);
                if (msg.MessageId == message.MessageId) {
                    popClient.DeleteMessage(i);
                }
            }

        } else {
            // Use imap
            var imapClient = GetImapClient(settings);

            var inbox = imapClient.Inbox;

            inbox.Open(FolderAccess.ReadWrite);

            var uids = inbox.Search(SearchQuery.All);

            for (int i = 0; i < settings.Count; i++) {
                var msg = inbox.GetMessage(i);
                if (msg.MessageId == message.MessageId) {
                    inbox.AddFlags(i, MessageFlags.Deleted, true);
                }
            }

        }
    }

    private void ReplyToEmail(MimeMessage message, ReadSettings settings) {
        // Reply to the message
        // Ask for the subject
        string subject = Console.Prompt(
            new TextPrompt<string>("Enter the subject of the email: ")
                .DefaultValue($"Re: {message.Subject}")
        );

        var sendSettings = new SendSettings {
            Sender = settings.Email,
            Recipient = message.From.ToString(),
            Subject = subject,
            BodyFilePath = "",
            ForceAscii = settings.ForceAscii
        };

        var client = SendCommand.GetClient(sendSettings);

        Console.Status()
            .Spinner(Spinner.Known.Line)
            .Start("Sending Email...", a => {


                a.Status("[green]Connecting to SMTP server...[/]");
                // Thread.Sleep(1000);

                a.Status("[green]Connected![/]");

                a.Status("[green]Sending email...[/]");
                // Thread.Sleep(1000);

                var msg = SendCommand.GetMsg(sendSettings);
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

    }

    private List<MimeMessage> GetMessages(ReadSettings settings) {

        var newestMessages = new List<MimeMessage>();

        if (settings.UsePop3) {

            // Use pop3
            var popClient = GetPop3Client(settings);

            var messages = popClient.GetMessageUids();

            for (int i = 0; i < settings.Count; i++) {
                var msg = popClient.GetMessage(i);
                newestMessages.Add(msg);
            }

        } else {

            // Use imap
            var imapClient = GetImapClient(settings);

            var inbox = imapClient.Inbox;

            inbox.Open(FolderAccess.ReadOnly);

            var uids = inbox.Search(SearchQuery.All);

            for (int i = 0; i < settings.Count; i++) {
                var msg = inbox.GetMessage(i);
                newestMessages.Add(msg);
            }

        }

        return newestMessages;
    }

    private Pop3Client GetPop3Client(ReadSettings settings) {

        var client = new Pop3Client();

        string host = ParseHost(settings);

        client.Connect(host, 995, true);

        // If no password is provided, prompt for it
        if (settings.Password == "") {
            // Ask the password
            Console.MarkupLine("Enter the password for [bold]{0}[/]", settings.Email);
            settings.Password = Console.Prompt(
                new TextPrompt<string>("")
                    .Secret()
            );
        }

        client.Authenticate(settings.Email, settings.Password);

        return client;

    }

    private ImapClient GetImapClient(ReadSettings settings) {

        var client = new ImapClient();

        string host = ParseHost(settings);

        client.Connect(host, 993, true);

        // If no password is provided, prompt for it
        if (settings.Password == "") {
            // Ask the password
            Console.MarkupLine("Enter the password for [bold]{0}[/]", settings.Email);
            settings.Password = Console.Prompt(
                new TextPrompt<string>("")
                    .Secret()
            );
        }

        client.Authenticate(settings.Email, settings.Password);

       return client;
    }

    private static string ParseHost(ReadSettings settings) {

        // From the domain of the sender
        string domain = settings.Email.Split('@')[1];

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