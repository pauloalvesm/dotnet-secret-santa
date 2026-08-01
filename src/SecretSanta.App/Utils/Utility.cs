using SecretSanta.App.Models;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace SecretSanta.App.Utils;

public static class Utility
{
    private static readonly Regex EmailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static bool ValidateName(string providedName) 
    {
        if (string.IsNullOrWhiteSpace(providedName)) 
        {
            Console.WriteLine("Name cannot be empty. Please enter a valid name.");
            return false;
        }

        if (!providedName.Contains(" ")) 
        {
            Console.WriteLine("The name must contain at least one space for the last name. Please enter a full name.");
            return false;
        }

        return true;
    }

    public static bool ValidateEmailFormat(string providedEmail)
    {
        if (string.IsNullOrWhiteSpace(providedEmail))
        {
            return false;
        }

        string trimmedEmail = providedEmail.Trim();

        if (trimmedEmail.Contains(".."))
        {
            return false;
        }

        if (!EmailRegex.IsMatch(trimmedEmail))
        {
            return false;
        }

        try
        {
            var addr = new MailAddress(trimmedEmail);
            return addr.Address == trimmedEmail;
        }
        catch
        {
            return false;
        }
    }

    public static void SortFriendsByName(List<Friend> friends) 
    {
        friends.Sort((friend1, friend2) => friend1.Name.CompareTo(friend2.Name));
    }
}
