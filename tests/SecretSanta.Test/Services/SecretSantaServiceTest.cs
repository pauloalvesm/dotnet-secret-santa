using Moq;
using SecretSanta.App.Data;
using SecretSanta.App.Models;
using SecretSanta.App.Services;

namespace SecretSanta.Test.Services;

public class SecretSantaServiceTest
{
    private readonly Mock<IPersistence> _persistenceMock;

    public SecretSantaServiceTest()
    {
        _persistenceMock = new Mock<IPersistence>();
    }

    [Fact(DisplayName = "AddFriend - Should add friend to list and sort alphabetically by name")]
    public void AddFriend_ShouldAddFriendToListAndSortByName()
    {
        // Arrange
        var initialList = new List<Friend>
        {
            new Friend("Zack Fair", "zack@gmail.com")
        };
        var service = new SecretSantaService(initialList, _persistenceMock.Object);
        var newFriend = new Friend("Alice Smith", "alice@gmail.com");

        // Act
        service.AddFriend(newFriend);

        // Assert
        Assert.Equal(2, initialList.Count);
        Assert.Contains(newFriend, initialList);
        Assert.Equal("Alice Smith", initialList[0].Name);
        Assert.Equal("Zack Fair", initialList[1].Name);
    }

    [Fact(DisplayName = "ListFriends - Should call ReadFriendsFromFile and return friends list")]
    public void ListFriends_ShouldCallReadFriendsFromFileAndReturnList()
    {
        // Arrange
        var friends = new List<Friend>
        {
            new Friend("John Doe", "john@gmail.com"),
            new Friend("Jane Doe", "jane@gmail.com")
        };

        _persistenceMock.Setup(p => p.ReadFriendsFromFile()).Returns(friends);
        var service = new SecretSantaService(friends, _persistenceMock.Object);

        // Act
        var result = service.ListFriends();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Same(friends, result);
        _persistenceMock.Verify(p => p.ReadFriendsFromFile(), Times.Once);
    }

    [Fact(DisplayName = "GenerateSecretSanta - When less than two friends should return empty list and not save")]
    public void GenerateSecretSanta_WhenLessThanTwoFriends_ShouldReturnEmptyListAndNotSave()
    {
        // Arrange
        var singleFriendList = new List<Friend>
        {
            new Friend("John Doe", "john@gmail.com")
        };
        var service = new SecretSantaService(singleFriendList, _persistenceMock.Object);

        // Act
        var result = service.GenerateSecretSanta();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _persistenceMock.Verify(p => p.SaveSecretSantaPairs(It.IsAny<List<Tuple<Friend, Friend>>>()), Times.Never);
    }

    [Fact(DisplayName = "GenerateSecretSanta - When list is empty should return empty list and not save")]
    public void GenerateSecretSanta_WhenListIsEmpty_ShouldReturnEmptyListAndNotSave()
    {
        // Arrange
        var emptyList = new List<Friend>();
        var service = new SecretSantaService(emptyList, _persistenceMock.Object);

        // Act
        var result = service.GenerateSecretSanta();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _persistenceMock.Verify(p => p.SaveSecretSantaPairs(It.IsAny<List<Tuple<Friend, Friend>>>()), Times.Never);
    }

    [Theory(DisplayName = "GenerateSecretSanta - When even number of friends should generate pairs and save to persistence")]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(6)]
    public void GenerateSecretSanta_WhenEvenNumberOfFriends_ShouldGeneratePairsAndSave(int friendCount)
    {
        // Arrange
        var friends = new List<Friend>();
        for (int i = 1; i <= friendCount; i++)
        {
            friends.Add(new Friend($"Friend {i}", $"friend{i}@gmail.com"));
        }

        var service = new SecretSantaService(friends, _persistenceMock.Object);

        // Act
        var pairs = service.GenerateSecretSanta();

        // Assert
        Assert.NotNull(pairs);
        Assert.Equal(friendCount / 2, pairs.Count);

        foreach (var pair in pairs)
        {
            Assert.NotNull(pair.Item1);
            Assert.NotNull(pair.Item2);
            Assert.NotEqual(pair.Item1, pair.Item2);
        }

        _persistenceMock.Verify(p => p.SaveSecretSantaPairs(It.IsAny<List<Tuple<Friend, Friend>>>()), Times.Once);
    }
}