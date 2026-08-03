using SecretSanta.App.Data;
using SecretSanta.App.Models;
using SecretSanta.App.Services;
using SecretSanta.App.Utils;

internal class Program
{
    private static void Main(string[] args)
    {
        IPersistence persistence = new Persistence();
        List<Friend> friends = persistence.ReadFriendsFromFile();

        SecretSantaService secretSantaService = new SecretSantaService(friends, persistence);

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\n****** Secret Santa Menu ******");
            Console.WriteLine("*   1 - Register friend       *");
            Console.WriteLine("*   2 - List friends          *");
            Console.WriteLine("*   3 - Generate Secret Santa *");
            Console.WriteLine("*   4 - Clear files           *");
            Console.WriteLine("*   5 - Exit                  *");
            Console.WriteLine("*******************************");

            Console.Write("\nOption: ");
            string option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    Console.Write("Enter the friend's name: ");
                    string name = Console.ReadLine();

                    while (!Utility.ValidateName(name))
                    {
                        Console.WriteLine("Enter a valid name:");
                        name = Console.ReadLine();
                    }

                    string email;
                    bool isEmailValid = false;

                    do
                    {
                        Console.Write("Enter the friend's email: ");
                        email = Console.ReadLine();

                        if (friends.Any(f => string.Equals(f.Email, email, StringComparison.OrdinalIgnoreCase)))
                        {
                            Console.WriteLine("Email already exists for a friend! Cannot add this friend.");
                            Console.WriteLine("Please check the email or choose another one.");
                        }
                        else if (!Utility.ValidateEmailFormat(email))
                        {
                            Console.WriteLine("Enter a valid email:");
                        }
                        else
                        {
                            isEmailValid = true;
                        }
                    }
                    while (!isEmailValid);

                    Friend newFriend = new Friend(name, email);
                    secretSantaService.AddFriend(newFriend);
                    persistence.SaveFriendsToFile(friends);
                    break;

                case "2":
                    Console.WriteLine("\nFriends List (sorted by name):\n");
                    List<Friend> friendsList = secretSantaService.ListFriends();
                    Utility.SortFriendsByName(friendsList);

                    foreach (var friend in friendsList)
                    {
                        Console.WriteLine(friend);
                    }
                    break;

                case "3":
                    List<Tuple<Friend, Friend>> secretSantaPairs = secretSantaService.GenerateSecretSanta();

                    if (secretSantaPairs != null)
                    {
                        Console.WriteLine("\nSecret Santa Pairs:\n");
                        persistence.ReadSecretSantaPairs();
                    }
                    break;

                case "4":
                    Console.WriteLine("\nClearing Lists!\n");
                    friends.Clear();
                    persistence.ClearFileContents();
                    break;

                case "5":
                    exit = true;
                    break;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
}