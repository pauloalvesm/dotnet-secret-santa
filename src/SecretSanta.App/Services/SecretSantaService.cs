using SecretSanta.App.Data;
using SecretSanta.App.Models;
using SecretSanta.App.Utils;

namespace SecretSanta.App.Services;

public class SecretSantaService
{
    private List<Friend> friendsList;
    Persistence persistence = new Persistence();

    public SecretSantaService(List<Friend> friends)
    {
        friendsList = friends;
    }

    public void AddFriend(Friend friend)
    {
        friendsList.Add(friend);
        Utility.SortFriendsByName(friendsList);
        Console.WriteLine("Friend successfully added!");
    }

    public List<Friend> ListFriends()
    {
        persistence.ReadFriendsFromFile();
        return friendsList;
    }

    public List<Tuple<Friend, Friend>> GenerateSecretSanta()
    {
        if (friendsList.Count < 2)
        {
            Console.WriteLine("\nThere are not enough friends to generate Secret Santa pairs.");
            return new List<Tuple<Friend, Friend>>();
        }
        else
        {
            List<Friend> shuffledFriends = friendsList.ToList();
            Random random = new Random();

            List<Tuple<Friend, Friend>> secretSantaPairs = new List<Tuple<Friend, Friend>>();

            while (shuffledFriends.Count > 0)
            {
                Friend friend = shuffledFriends[0];
                shuffledFriends.RemoveAt(0);

                int secretSantaIndex = random.Next(0, shuffledFriends.Count);
                Friend secretSantaFriend = shuffledFriends[secretSantaIndex];
                shuffledFriends.RemoveAt(secretSantaIndex);

                secretSantaPairs.Add(new Tuple<Friend, Friend>(friend, secretSantaFriend));
            }

            persistence.SaveSecretSantaPairs(secretSantaPairs);

            return secretSantaPairs;
        }
    }
}
