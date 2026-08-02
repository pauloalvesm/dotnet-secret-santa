using SecretSanta.App.Models;
using SecretSanta.App.Utils;

namespace SecretSanta.Test.Utils;

public class UtilityTest
{
    [Theory(DisplayName = "ValidateName - When name has first name and last name should return true")]
    [InlineData("John Doe")]
    [InlineData("Alice Smith Junior")]
    [InlineData("Mary Jane Watson")]
    public void ValidateName_WhenNameHasFirstNameAndLastName_ShouldReturnTrue(string validName)
    {
        // Act
        var result = Utility.ValidateName(validName);

        // Assert
        Assert.True(result);
    }

    [Theory(DisplayName = "ValidateName - When name is null empty or whitespace should return false")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ValidateName_WhenNameIsNullOrEmptyOrWhitespace_ShouldReturnFalse(string invalidName)
    {
        // Act
        var result = Utility.ValidateName(invalidName);

        // Assert
        Assert.False(result);
    }

    [Theory(DisplayName = "ValidateName - When name does not contain space should return false")]
    [InlineData("John")]
    [InlineData("SingleWordName")]
    public void ValidateName_WhenNameDoesNotContainSpace_ShouldReturnFalse(string nameWithoutSpace)
    {
        // Act
        var result = Utility.ValidateName(nameWithoutSpace);

        // Assert
        Assert.False(result);
    }

    [Theory(DisplayName = "ValidateEmailFormat - When email is valid should return true")]
    [InlineData("john.doe@gmail.com")]
    [InlineData("user.name+tag@domain.co.uk")]
    [InlineData("friend@company.org")]
    public void ValidateEmailFormat_WhenEmailIsValid_ShouldReturnTrue(string validEmail)
    {
        // Act
        var result = Utility.ValidateEmailFormat(validEmail);

        // Assert
        Assert.True(result);
    }

    [Theory(DisplayName = "ValidateEmailFormat - When email is null empty or whitespace should return false")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ValidateEmailFormat_WhenEmailIsNullOrEmptyOrWhitespace_ShouldReturnFalse(string invalidEmail)
    {
        // Act
        var result = Utility.ValidateEmailFormat(invalidEmail);

        // Assert
        Assert.False(result);
    }

    [Theory(DisplayName = "ValidateEmailFormat - When email is invalid format should return false")]
    [InlineData("plainaddress")]
    [InlineData("@missingusername.com")]
    [InlineData("username@.com")]
    [InlineData("username@domain..com")]
    [InlineData("username@domain")]
    public void ValidateEmailFormat_WhenEmailIsInvalidFormat_ShouldReturnFalse(string invalidEmail)
    {
        // Act
        var result = Utility.ValidateEmailFormat(invalidEmail);

        // Assert
        Assert.False(result);
    }

    [Fact(DisplayName = "SortFriendsByName - When unsorted list provided should sort alphabetically by name")]
    public void SortFriendsByName_WhenUnsortedListProvided_ShouldSortAlphabeticallyByName()
    {
        // Arrange
        var friends = new List<Friend>
        {
            new Friend("Zack Fair", "zack@gmail.com"),
            new Friend("Alice Smith", "alice@gmail.com"),
            new Friend("Cloud Strife", "cloud@gmail.com")
        };

        // Act
        Utility.SortFriendsByName(friends);

        // Assert
        Assert.Equal(3, friends.Count);
        Assert.Equal("Alice Smith", friends[0].Name);
        Assert.Equal("Cloud Strife", friends[1].Name);
        Assert.Equal("Zack Fair", friends[2].Name);
    }

    [Fact(DisplayName = "SortFriendsByName - When list is empty should not throw exception")]
    public void SortFriendsByName_WhenListIsEmpty_ShouldNotThrowException()
    {
        // Arrange
        var emptyList = new List<Friend>();

        // Act & Assert
        var exception = Record.Exception(() => Utility.SortFriendsByName(emptyList));
        Assert.Null(exception);
        Assert.Empty(emptyList);
    }
}