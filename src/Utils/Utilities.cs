using System;

namespace Gammer0909.SIGMAMail.Common;

public static class Utilities {

    public static string GetPassword() {
        var password = "";
        while (true) {
            var key = System.Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter) {
                break;
            } else if (key.Key == ConsoleKey.Backspace) {
                if (password.Length > 0) {
                    password = password.Remove(password.Length - 1);
                    Console.Write("\b \b");
                }
            } else {
                password += key.KeyChar;
                Console.Write("*");
            }
        }
        return password;
    }

    public static bool YesNo() {
            
        while (true) {
            var key = System.Console.ReadKey(true);
            if (key.Key == ConsoleKey.Y) {
                return true;
            } else if (key.Key == ConsoleKey.N) {
                return false;
            }
        }

    }

    public static bool IsAscii(char c) {
        return c <= 127;
    }

}