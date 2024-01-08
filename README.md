# SIGMAMail Spec

SIGMAMail is a WIP Console-based Email client, written in C#, using Spectre.Console, and Mailkit.

SIGMAMail is an acronym for **S**ecure **I**nterface for **G**lobal **M**essaging **A**pplications

This is a specification as of currently, but I'll update this as progress is made!

[X|X| | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | ] 1% Finished

## Features (Planned)

- [ ] TUI Based and CLI Based interfaces
- [ ] Built-in text editor (TUI Only)
- [ ] Writing Emails in Markdown (TUI Only, however you can upload one to the CLI)
- [ ] Uploding Attachments (TUI Only)
- [ ] Viewing image attachments as ASCII Images (TUI Only)
- [ ] Downloading Attachments
- [ ] Searching Emails (Subject, Sender, Recipient)
- [ ] Built-in HTML and Markdown Viewer (TUI Only)
- [ ] Basic Email Features (Read, send, reply, forward, etc.)
- [ ] IMAP Support
- [ ] POP3 Support
- [x] SMTP Support

More to come!

## Current (Implemented) Features usage

### Sending Emails

To send an email, you can use the `send` command, and then follow the prompts. You can also use the `send` command with the `-f` flag to specify a file to send. 
The file can be a `.txt` file, or a `.md` file. If it is a `.md` file, it will be converted to HTML before sending. (Optionally, you can upload a `.html` file)
Here's some sample usage:

```bash
smail send -f email.md
Enter your email address: test.acc@test.com
Enter your password: # (The characters here are hashtags, but bash shows those as comments so I can't put them here)
Enter the recipient's email address: <>
Enter the subject of the email: <>
```

Or, if you don't upload a file

```bash
smail send
Enter your email address: test.acc@test.com
Enter your password: # (The characters here are hashtags, but bash shows those as comments so I can't put them here)
Enter the recipient's email address:
Enter the subject of the email: <>
Enter the body of the email: <>
This is body text!\nYou have to use \n to make a new line!

```

This probably isn't very CLI-Like, but it was easiser to implement this way, so I'm going to keep it like this for now.