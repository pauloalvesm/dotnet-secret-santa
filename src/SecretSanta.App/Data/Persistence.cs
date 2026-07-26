using SecretSanta.App.Models;
using System.Reflection;

namespace SecretSanta.App.Data;

public class Persistence
{
    private string friendsFilePath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "friends.csv");
    private string secretSantaFilePath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "secretsanta.csv");

    public List<Friend> ReadFriendsFromFile()
    {
        var friendsList = new List<Friend>();

        if (File.Exists(friendsFilePath))
        {
            using (StreamReader streamReader = new StreamReader(friendsFilePath))
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    string[] data = line.Split(';');
                    friendsList.Add(new Friend(data[0], data[1]));
                }
            }

            if (friendsList.Count == 0)
            {
                Console.WriteLine("Warning: The friends list is empty.");
            }
        }

        return friendsList;
    }

    public void SaveFriendsToFile(List<Friend> friendsList)
    {
        if (!Directory.Exists(Path.GetDirectoryName(friendsFilePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(friendsFilePath));
        }

        using (StreamWriter writer = new StreamWriter(friendsFilePath))
        {
            foreach (var friend in friendsList)
            {
                writer.WriteLine($"{friend.Name};{friend.Email}");
            }
        }

        Console.WriteLine("The friend was added to the file: friends");
    }

    public List<Tuple<Friend, Friend>> ReadSecretSantaPairs()
    {
        var secretSantaPairs = new List<Tuple<Friend, Friend>>();

        if (File.Exists(secretSantaFilePath))
        {
            using (StreamReader streamReader = new StreamReader(secretSantaFilePath))
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    string[] data = line.Split(';');

                    if (data.Length >= 4)
                    {
                        Friend leftFriend = new Friend(data[0], data[1]);
                        Friend rightFriend = new Friend(data[2], data[3]);

                        secretSantaPairs.Add(new Tuple<Friend, Friend>(leftFriend, rightFriend));
                    }
                    else
                    {
                        Console.WriteLine("Warning: File has insufficient data.");
                    }
                }
            }
        }

        foreach (var pair in secretSantaPairs)
        {
            Console.WriteLine($"{pair.Item1.Name} -> {pair.Item2.Name}");
        }

        return secretSantaPairs;
    }

    public void SaveSecretSantaPairs(List<Tuple<Friend, Friend>> secretSantaPairs)
    {
        if (!Directory.Exists(Path.GetDirectoryName(secretSantaFilePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(secretSantaFilePath));
        }

        using (StreamWriter streamWriter = new StreamWriter(secretSantaFilePath))
        {
            foreach (var pair in secretSantaPairs)
            {
                string line = $"{pair.Item1.Name};{pair.Item1.Email};{pair.Item2.Name};{pair.Item2.Email}";
                streamWriter.WriteLine(line);
            }
        }

        Console.WriteLine("Secret Santa pairs were added to the file: secretsanta");
    }

    public void ClearFileContents()
    {
        ClearFileContent(friendsFilePath);
        ClearFileContent(secretSantaFilePath);
    }

    private void ClearFileContent(string filePath)
    {
        if (File.Exists(filePath))
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(string.Empty);
            }
        }
    }
}
