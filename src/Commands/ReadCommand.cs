using Spectre.Console.Cli;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Search;
using MailKit;
using Spectre.Console;
using MimeKit;
using EmailValidation;

namespace Gammer0909.SIGMAMail.Settings;

public class ReadCommand : Command<ReadSettings> {

    public override int Execute(CommandContext context, ReadSettings settings) {

        if (EmailValidator.Validate(settings.Email) == false) {
            Console.MarkupLine("[red]Invalid email address![/]");
            return 1;
        }

        var messages = GetMessages(settings);

        

        return 0;
    }

    private void RenderMessages(List<MimeMessage> messages) {

 

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