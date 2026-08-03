using SecretSanta.App.Models;

namespace SecretSanta.App.Data;

public interface IPersistence
{
    List<Friend> ReadFriendsFromFile();
    void SaveFriendsToFile(List<Friend> friendsList);
    List<Tuple<Friend, Friend>> ReadSecretSantaPairs();
    void SaveSecretSantaPairs(List<Tuple<Friend, Friend>> secretSantaPairs);
    void ClearFileContents();
}
