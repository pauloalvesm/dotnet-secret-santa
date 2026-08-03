using SecretSanta.App.Data;
using SecretSanta.App.Models;
using System.Reflection;

namespace SecretSanta.Test.Data;

public class PersistenceTest : IDisposable
{
    private readonly Persistence _persistence;
    private readonly string _basePath;
    private readonly string _friendsFilePath;
    private readonly string _secretSantaFilePath;

    public PersistenceTest()
    {
        _persistence = new Persistence();
        _basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        _friendsFilePath = Path.Combine(_basePath, "friends.csv");
        _secretSantaFilePath = Path.Combine(_basePath, "secretsanta.csv");

        CleanUpFiles();
    }

    public void Dispose()
    {
        CleanUpFiles();
    }

    [Fact(DisplayName = "ReadFriendsFromFile - When file does not exist should return empty list")]
    public void ReadFriendsFromFile_WhenFileDoesNotExist_ShouldReturnEmptyList()
    {
        // Act
        var result = _persistence.ReadFriendsFromFile();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact(DisplayName = "ReadFriendsFromFile - When file is empty should return empty list")]
    public void ReadFriendsFromFile_WhenFileIsEmpty_ShouldReturnEmptyList()
    {
        // Arrange
        File.WriteAllText(_friendsFilePath, string.Empty);

        // Act
        var result = _persistence.ReadFriendsFromFile();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact(DisplayName = "ReadFriendsFromFile - When file has valid data should return friends list")]
    public void ReadFriendsFromFile_WhenFileHasValidData_ShouldReturnFriendsList()
    {
        // Arrange
        var content = "John Doe;john@gmail.com\nJane Doe;jane@gmail.com";
        File.WriteAllText(_friendsFilePath, content);

        // Act
        var result = _persistence.ReadFriendsFromFile();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("John Doe", result[0].Name);
        Assert.Equal("john@gmail.com", result[0].Email);
        Assert.Equal("Jane Doe", result[1].Name);
        Assert.Equal("jane@gmail.com", result[1].Email);
    }

    [Fact(DisplayName = "SaveFriendsToFile - When list has friends should write correctly to file")]
    public void SaveFriendsToFile_WhenListHasFriends_ShouldWriteCorrectlyToFile()
    {
        // Arrange
        var friends = new List<Friend>
        {
            new Friend("Alice Smith", "alice@gmail.com"),
            new Friend("Bob Builder", "bob@gmail.com")
        };

        // Act
        _persistence.SaveFriendsToFile(friends);

        // Assert
        Assert.True(File.Exists(_friendsFilePath));
        var lines = File.ReadAllLines(_friendsFilePath);
        Assert.Equal(2, lines.Length);
        Assert.Equal("Alice Smith;alice@gmail.com", lines[0]);
        Assert.Equal("Bob Builder;bob@gmail.com", lines[1]);
    }

    [Fact(DisplayName = "SaveFriendsToFile - When list is empty should create empty file")]
    public void SaveFriendsToFile_WhenListIsEmpty_ShouldCreateEmptyFile()
    {
        // Arrange
        var emptyList = new List<Friend>();

        // Act
        _persistence.SaveFriendsToFile(emptyList);

        // Assert
        Assert.True(File.Exists(_friendsFilePath));
        var lines = File.ReadAllLines(_friendsFilePath);
        Assert.Empty(lines);
    }

    [Fact(DisplayName = "ReadSecretSantaPairs - When file does not exist should return empty list")]
    public void ReadSecretSantaPairs_WhenFileDoesNotExist_ShouldReturnEmptyList()
    {
        // Act
        var result = _persistence.ReadSecretSantaPairs();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact(DisplayName = "ReadSecretSantaPairs - When file has sufficient data should return pairs list")]
    public void ReadSecretSantaPairs_WhenFileHasSufficientData_ShouldReturnPairsList()
    {
        // Arrange
        var content = "John;john@gmail.com;Jane;jane@gmail.com\nAlice;alice@gmail.com;Bob;bob@gmail.com";
        File.WriteAllText(_secretSantaFilePath, content);

        // Act
        var result = _persistence.ReadSecretSantaPairs();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("John", result[0].Item1.Name);
        Assert.Equal("john@gmail.com", result[0].Item1.Email);
        Assert.Equal("Jane", result[0].Item2.Name);
        Assert.Equal("jane@gmail.com", result[0].Item2.Email);
    }

    [Fact(DisplayName = "ReadSecretSantaPairs - When file has insufficient data should ignore invalid line and return valid pairs")]
    public void ReadSecretSantaPairs_WhenFileHasInsufficientData_ShouldIgnoreInvalidLineAndReturnValidPairs()
    {
        // Arrange
        var content = "InvalidLineDataWithoutDataDelimiter\nAlice;alice@gmail.com;Bob;bob@gmail.com";
        File.WriteAllText(_secretSantaFilePath, content);

        // Act
        var result = _persistence.ReadSecretSantaPairs();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Alice", result[0].Item1.Name);
        Assert.Equal("Bob", result[0].Item2.Name);
    }

    [Fact(DisplayName = "SaveSecretSantaPairs - When pairs exist should write correctly to file")]
    public void SaveSecretSantaPairs_WhenPairsExist_ShouldWriteCorrectlyToFile()
    {
        // Arrange
        var pairs = new List<Tuple<Friend, Friend>>
        {
            new Tuple<Friend, Friend>(
                new Friend("John Doe", "john@gmail.com"),
                new Friend("Jane Doe", "jane@gmail.com")
            )
        };

        // Act
        _persistence.SaveSecretSantaPairs(pairs);

        // Assert
        Assert.True(File.Exists(_secretSantaFilePath));
        var lines = File.ReadAllLines(_secretSantaFilePath);
        Assert.Single(lines);
        Assert.Equal("John Doe;john@gmail.com;Jane Doe;jane@gmail.com", lines[0]);
    }

    [Fact(DisplayName = "ClearFileContents - When files exist with content should truncate both files")]
    public void ClearFileContents_WhenFilesExistWithContent_ShouldTruncateBothFiles()
    {
        // Arrange
        File.WriteAllText(_friendsFilePath, "Some content");
        File.WriteAllText(_secretSantaFilePath, "Some pairs content");

        // Act
        _persistence.ClearFileContents();

        // Assert
        Assert.True(File.Exists(_friendsFilePath));
        Assert.True(File.Exists(_secretSantaFilePath));
        Assert.Equal(string.Empty, File.ReadAllText(_friendsFilePath));
        Assert.Equal(string.Empty, File.ReadAllText(_secretSantaFilePath));
    }

    [Fact(DisplayName = "ClearFileContents - When files do not exist should not throw exception")]
    public void ClearFileContents_WhenFilesDoNotExist_ShouldNotThrowException()
    {
        // Act & Assert
        var exception = Record.Exception(() => _persistence.ClearFileContents());
        Assert.Null(exception);
    }

    private void CleanUpFiles()
    {
        if (File.Exists(_friendsFilePath))
        {
            File.Delete(_friendsFilePath);
        }

        if (File.Exists(_secretSantaFilePath))
        {
            File.Delete(_secretSantaFilePath);
        }
    }
}