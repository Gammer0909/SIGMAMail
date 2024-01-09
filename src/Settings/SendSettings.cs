using System;
using Spectre.Console.Cli;
using Gammer0909.SIGMAMail.Settings;

namespace Gammer0909.SIGMAMail.Settings;

/// <summary>
/// The settings for the send command.
/// </summary>
public class SendSettings : BaseSettings {

    [CommandArgument(0, "<SENDER>")]
    public string Sender { get; set; } = "";

    [CommandOption("-p|--password <PASSWORD>")]
    public string Password { get; set; } = "";

    [CommandArgument(1, "<RECIPIENT>")]
    public string Recipient { get; set; } = "";

    [CommandArgument(2, "<SUBJECT>")]
    public string Subject { get; set; } = "";

    [CommandArgument(3, "<BODY_FILE_PATH>")]
    public string BodyFilePath { get; set; } = "";

    [CommandOption("-A|--attachment <ATTACHMENT_FILE_PATH>")]
    public string AttachmentFilePath { get; set; } = "";

    [CommandOption("-c|--cc <CC_RECIPIENTS>")]
    public string[] CcRecipient { get; set; } = Array.Empty<string>();

    [CommandOption("-b|--bcc <BCC_RECIPIENTS>")]
    public string[] BccRecipient { get; set; } = Array.Empty<string>();

}